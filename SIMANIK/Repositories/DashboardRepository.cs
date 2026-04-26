using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class DashboardRepository
    {
        private const int LowStockThreshold = 10;

        public AdminDashboardSummary GetAdminSummary()
        {
            AdminDashboardSummary summary = new AdminDashboardSummary
            {
                TotalPatients = ExecuteScalarInt("SELECT COUNT(1) FROM patients;", null),
                TotalActiveDoctors = ExecuteScalarInt("SELECT COUNT(1) FROM doctors WHERE IsActive = 1;", null),
                ReservationsToday = ExecuteScalarInt(@"
                    SELECT COUNT(1)
                    FROM reservations r
                    INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                    WHERE s.ScheduleDate = CURDATE();", null),
                PendingReservations = ExecuteScalarInt("SELECT COUNT(1) FROM reservations WHERE ReservationStatus = 'Menunggu Verifikasi';", null),
                CheckedInPatientsToday = ExecuteScalarInt("SELECT COUNT(1) FROM visits WHERE DATE(CheckInTime) = CURDATE();", null),
                CompletedExaminationsToday = ExecuteScalarInt("SELECT COUNT(1) FROM examinations WHERE DATE(ExaminationDate) = CURDATE();", null),
                LowStockMedicines = ExecuteScalarInt("SELECT COUNT(1) FROM medicines WHERE Stock <= @threshold AND IsActive = 1;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@threshold", LowStockThreshold);
                }),
                TopDiseaseThisMonth = ExecuteScalarString(@"
                    SELECT d.DiseaseName
                    FROM examinations e
                    INNER JOIN diseases d ON d.DiseaseId = e.DiseaseId
                    WHERE e.ExaminationDate >= DATE_FORMAT(CURDATE(), '%Y-%m-01')
                    GROUP BY d.DiseaseId, d.DiseaseName
                    ORDER BY COUNT(1) DESC, d.DiseaseName
                    LIMIT 1;", null)
            };

            if (string.IsNullOrWhiteSpace(summary.TopDiseaseThisMonth))
            {
                summary.TopDiseaseThisMonth = "Belum ada data";
            }

            return summary;
        }

        public List<ChartDataPoint> GetReservationsLast7Days()
        {
            return QueryChart(@"
                SELECT DATE_FORMAT(s.ScheduleDate, '%d/%m') AS Label, COUNT(1) AS Value
                FROM reservations r
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                WHERE s.ScheduleDate BETWEEN DATE_SUB(CURDATE(), INTERVAL 6 DAY) AND CURDATE()
                GROUP BY s.ScheduleDate
                ORDER BY s.ScheduleDate;", null);
        }

        public List<ChartDataPoint> GetReservationStatusDistribution()
        {
            return QueryChart(@"
                SELECT ReservationStatus AS Label, COUNT(1) AS Value
                FROM reservations
                GROUP BY ReservationStatus
                ORDER BY ReservationStatus;", null);
        }

        public List<ChartDataPoint> GetPatientCountByDoctor()
        {
            return QueryChart(@"
                SELECT d.FullName AS Label, COUNT(DISTINCT r.PatientId) AS Value
                FROM doctors d
                LEFT JOIN doctor_schedules s ON s.DoctorId = d.DoctorId
                LEFT JOIN reservations r ON r.ScheduleId = s.ScheduleId
                WHERE d.IsActive = 1
                GROUP BY d.DoctorId, d.FullName
                ORDER BY Value DESC, d.FullName
                LIMIT 10;", null);
        }

        public List<ChartDataPoint> GetTopDiseasesThisMonth()
        {
            return QueryChart(@"
                SELECT d.DiseaseName AS Label, COUNT(1) AS Value
                FROM examinations e
                INNER JOIN diseases d ON d.DiseaseId = e.DiseaseId
                WHERE e.ExaminationDate >= DATE_FORMAT(CURDATE(), '%Y-%m-01')
                GROUP BY d.DiseaseId, d.DiseaseName
                ORDER BY Value DESC, d.DiseaseName
                LIMIT 10;", null);
        }

        public List<RecentReservationItem> GetRecentReservations()
        {
            return QueryList(@"
                SELECT s.ScheduleDate AS ItemDate,
                       p.FullName AS PatientName,
                       d.FullName AS DoctorName,
                       r.Complaint,
                       r.ReservationStatus AS Status,
                       CONCAT(DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i')) AS ExtraInfo
                FROM reservations r
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                ORDER BY r.CreatedAt DESC
                LIMIT 10;", null, MapRecentReservation);
        }

        public List<QueueItem> GetTodayQueues()
        {
            return QueryList(@"
                SELECT COALESCE(v.CheckInTime, s.ScheduleDate) AS ItemDate,
                       v.QueueNumber,
                       p.FullName AS PatientName,
                       d.FullName AS DoctorName,
                       r.Complaint,
                       v.VisitStatus AS Status
                FROM visits v
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE DATE(v.CheckInTime) = CURDATE() OR s.ScheduleDate = CURDATE()
                ORDER BY v.QueueNumber, v.CheckInTime
                LIMIT 20;", null, MapQueue);
        }

        public List<LowStockMedicineItem> GetLowStockMedicines()
        {
            return QueryList(@"
                SELECT MedicineName, MedicineType, Stock, Unit, IsActive
                FROM medicines
                WHERE Stock <= @threshold
                ORDER BY Stock ASC, MedicineName
                LIMIT 20;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@threshold", LowStockThreshold);
                }, MapLowStockMedicine);
        }

        public List<DashboardSearchResultItem> SearchAdminDashboardData(DashboardSearchCriteria criteria)
        {
            List<DashboardSearchResultItem> results = new List<DashboardSearchResultItem>();

            if (ShouldSearch(criteria, "Semua", "Pasien"))
            {
                results.AddRange(SearchAdminPatients(criteria));
            }

            if (ShouldSearch(criteria, "Semua", "Dokter"))
            {
                results.AddRange(SearchAdminDoctors(criteria));
            }

            if (ShouldSearch(criteria, "Semua", "Reservasi"))
            {
                results.AddRange(SearchAdminReservations(criteria));
            }

            if (ShouldSearch(criteria, "Semua", "Antrian"))
            {
                results.AddRange(SearchAdminQueues(criteria));
            }

            if (ShouldSearch(criteria, "Semua", "Obat"))
            {
                results.AddRange(SearchAdminMedicines(criteria));
            }

            if (ShouldSearch(criteria, "Semua", "Penyakit"))
            {
                results.AddRange(SearchAdminDiseases(criteria));
            }

            return results;
        }

        public DoctorDashboardSummary GetDoctorSummary(int userId)
        {
            int doctorId = GetDoctorIdByUserId(userId);

            if (doctorId <= 0)
            {
                return new DoctorDashboardSummary();
            }

            return new DoctorDashboardSummary
            {
                TodaySchedules = ExecuteScalarInt("SELECT COUNT(1) FROM doctor_schedules WHERE DoctorId = @doctorId AND ScheduleDate = CURDATE() AND IsActive = 1;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                }),
                WaitingPatients = ExecuteScalarInt(@"
                    SELECT COUNT(1)
                    FROM visits v
                    INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                    INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                    WHERE s.DoctorId = @doctorId AND s.ScheduleDate = CURDATE() AND v.VisitStatus = 'Menunggu';", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                }),
                InProgressPatients = ExecuteScalarInt(@"
                    SELECT COUNT(1)
                    FROM visits v
                    INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                    INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                    WHERE s.DoctorId = @doctorId AND s.ScheduleDate = CURDATE() AND v.VisitStatus = 'Sedang Diperiksa';", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                }),
                CompletedExaminationsToday = ExecuteScalarInt("SELECT COUNT(1) FROM examinations WHERE DoctorId = @doctorId AND DATE(ExaminationDate) = CURDATE();", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                })
            };
        }

        public List<QueueItem> GetDoctorTodayQueue(int userId)
        {
            int doctorId = GetDoctorIdByUserId(userId);

            if (doctorId <= 0)
            {
                return new List<QueueItem>();
            }

            return QueryList(@"
                SELECT COALESCE(v.CheckInTime, s.ScheduleDate) AS ItemDate,
                       v.QueueNumber,
                       p.FullName AS PatientName,
                       d.FullName AS DoctorName,
                       r.Complaint,
                       v.VisitStatus AS Status
                FROM visits v
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE s.DoctorId = @doctorId AND s.ScheduleDate = CURDATE()
                ORDER BY v.QueueNumber, v.CheckInTime
                LIMIT 20;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                }, MapQueue);
        }

        public List<ChartDataPoint> GetDoctorQueueStatusDistribution(int userId)
        {
            int doctorId = GetDoctorIdByUserId(userId);

            if (doctorId <= 0)
            {
                return new List<ChartDataPoint>();
            }

            return QueryChart(@"
                SELECT v.VisitStatus AS Label, COUNT(1) AS Value
                FROM visits v
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                WHERE s.DoctorId = @doctorId AND s.ScheduleDate = CURDATE()
                GROUP BY v.VisitStatus
                ORDER BY v.VisitStatus;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                });
        }

        public List<ChartDataPoint> GetDoctorExaminationsLast7Days(int userId)
        {
            int doctorId = GetDoctorIdByUserId(userId);

            if (doctorId <= 0)
            {
                return new List<ChartDataPoint>();
            }

            return QueryChart(@"
                SELECT DATE_FORMAT(ExaminationDate, '%d/%m') AS Label, COUNT(1) AS Value
                FROM examinations
                WHERE DoctorId = @doctorId
                  AND DATE(ExaminationDate) BETWEEN DATE_SUB(CURDATE(), INTERVAL 6 DAY) AND CURDATE()
                GROUP BY DATE(ExaminationDate)
                ORDER BY DATE(ExaminationDate);", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                });
        }

        public List<ChartDataPoint> GetDoctorTopDiseases(int userId)
        {
            int doctorId = GetDoctorIdByUserId(userId);

            if (doctorId <= 0)
            {
                return new List<ChartDataPoint>();
            }

            return QueryChart(@"
                SELECT d.DiseaseName AS Label, COUNT(1) AS Value
                FROM examinations e
                INNER JOIN diseases d ON d.DiseaseId = e.DiseaseId
                WHERE e.DoctorId = @doctorId
                GROUP BY d.DiseaseId, d.DiseaseName
                ORDER BY Value DESC, d.DiseaseName
                LIMIT 10;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                });
        }

        public List<RecentExaminationItem> GetDoctorRecentExaminations(int userId)
        {
            int doctorId = GetDoctorIdByUserId(userId);

            if (doctorId <= 0)
            {
                return new List<RecentExaminationItem>();
            }

            return QueryList(@"
                SELECT e.ExaminationDate AS ItemDate,
                       p.FullName AS PatientName,
                       d.FullName AS DoctorName,
                       dis.DiseaseName,
                       e.CurrentComplaint AS Complaint,
                       e.TreatmentNotes AS Treatment
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                WHERE e.DoctorId = @doctorId
                ORDER BY e.ExaminationDate DESC
                LIMIT 10;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                }, MapRecentExamination);
        }

        public List<DashboardSearchResultItem> SearchDoctorDashboardData(int userId, DashboardSearchCriteria criteria)
        {
            int doctorId = GetDoctorIdByUserId(userId);

            if (doctorId <= 0)
            {
                return new List<DashboardSearchResultItem>();
            }

            if (IsType(criteria, "Antrian Hari Ini"))
            {
                return SearchDoctorQueues(doctorId, criteria);
            }

            if (IsType(criteria, "Riwayat Pemeriksaan"))
            {
                return SearchDoctorExaminations(doctorId, criteria);
            }

            if (IsType(criteria, "Jadwal Praktik"))
            {
                return SearchDoctorSchedules(doctorId, criteria);
            }

            if (IsType(criteria, "Diagnosa/Penyakit"))
            {
                return SearchDoctorDiseases(doctorId, criteria);
            }

            return new List<DashboardSearchResultItem>();
        }

        public PatientDashboardSummary GetPatientSummary(int userId)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new PatientDashboardSummary
                {
                    PatientName = "Belum ada data",
                    PatientNumber = "-",
                    NextReservation = "Belum ada data",
                    LastReservationStatus = "Belum ada data"
                };
            }

            PatientDashboardSummary summary = QuerySingle(@"
                SELECT FullName, PatientNumber, TIMESTAMPDIFF(YEAR, BirthDate, CURDATE()) AS Age
                FROM patients
                WHERE PatientId = @patientId
                LIMIT 1;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                }, delegate(MySqlDataReader reader)
                {
                    return new PatientDashboardSummary
                    {
                        PatientName = GetString(reader, "FullName"),
                        PatientNumber = GetString(reader, "PatientNumber"),
                        Age = GetInt(reader, "Age"),
                        NextReservation = "Belum ada data",
                        LastReservationStatus = "Belum ada data"
                    };
                }) ?? new PatientDashboardSummary();

            string nextReservation = ExecuteScalarString(@"
                SELECT CONCAT(DATE_FORMAT(s.ScheduleDate, '%d/%m/%Y'), ' - ', d.FullName, ' (', r.ReservationStatus, ')')
                FROM reservations r
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE r.PatientId = @patientId AND s.ScheduleDate >= CURDATE()
                ORDER BY s.ScheduleDate, s.StartTime
                LIMIT 1;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });

            string lastStatus = ExecuteScalarString(@"
                SELECT r.ReservationStatus
                FROM reservations r
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                WHERE r.PatientId = @patientId
                ORDER BY s.ScheduleDate DESC, r.CreatedAt DESC
                LIMIT 1;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });

            summary.NextReservation = string.IsNullOrWhiteSpace(nextReservation) ? "Belum ada data" : nextReservation;
            summary.LastReservationStatus = string.IsNullOrWhiteSpace(lastStatus) ? "Belum ada data" : lastStatus;

            return summary;
        }

        public List<ChartDataPoint> GetPatientReservationStatusDistribution(int userId)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new List<ChartDataPoint>();
            }

            return QueryChart(@"
                SELECT ReservationStatus AS Label, COUNT(1) AS Value
                FROM reservations
                WHERE PatientId = @patientId
                GROUP BY ReservationStatus
                ORDER BY ReservationStatus;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });
        }

        public List<ChartDataPoint> GetPatientVisitsPerMonth(int userId)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new List<ChartDataPoint>();
            }

            return QueryChart(@"
                SELECT DATE_FORMAT(v.CheckInTime, '%Y-%m') AS Label, COUNT(1) AS Value
                FROM visits v
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                WHERE r.PatientId = @patientId
                GROUP BY DATE_FORMAT(v.CheckInTime, '%Y-%m')
                ORDER BY Label
                LIMIT 12;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });
        }

        public List<ChartDataPoint> GetPatientDiseaseHistory(int userId)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new List<ChartDataPoint>();
            }

            return QueryChart(@"
                SELECT d.DiseaseName AS Label, COUNT(1) AS Value
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN diseases d ON d.DiseaseId = e.DiseaseId
                WHERE r.PatientId = @patientId
                GROUP BY d.DiseaseId, d.DiseaseName
                ORDER BY Value DESC, d.DiseaseName
                LIMIT 10;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });
        }

        public List<RecentReservationItem> GetPatientRecentReservations(int userId)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new List<RecentReservationItem>();
            }

            return QueryList(@"
                SELECT s.ScheduleDate AS ItemDate,
                       p.FullName AS PatientName,
                       d.FullName AS DoctorName,
                       r.Complaint,
                       r.ReservationStatus AS Status,
                       CONCAT(DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i')) AS ExtraInfo
                FROM reservations r
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE r.PatientId = @patientId
                ORDER BY s.ScheduleDate DESC, r.CreatedAt DESC
                LIMIT 10;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                }, MapRecentReservation);
        }

        public List<RecentExaminationItem> GetPatientRecentExaminations(int userId)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new List<RecentExaminationItem>();
            }

            return QueryList(@"
                SELECT e.ExaminationDate AS ItemDate,
                       p.FullName AS PatientName,
                       d.FullName AS DoctorName,
                       dis.DiseaseName,
                       e.CurrentComplaint AS Complaint,
                       e.TreatmentNotes AS Treatment
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                WHERE r.PatientId = @patientId
                ORDER BY e.ExaminationDate DESC
                LIMIT 10;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                }, MapRecentExamination);
        }

        public List<LowStockMedicineItem> GetPatientRecentMedicines(int userId)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new List<LowStockMedicineItem>();
            }

            return QueryList(@"
                SELECT m.MedicineName, m.MedicineType, pd.Quantity AS Stock, m.Unit, m.IsActive
                FROM prescription_details pd
                INNER JOIN medicines m ON m.MedicineId = pd.MedicineId
                INNER JOIN examinations e ON e.ExaminationId = pd.ExaminationId
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                WHERE r.PatientId = @patientId
                ORDER BY e.ExaminationDate DESC
                LIMIT 10;", delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                }, MapLowStockMedicine);
        }

        public List<DashboardSearchResultItem> SearchPatientDashboardData(int userId, DashboardSearchCriteria criteria)
        {
            int patientId = GetPatientIdByUserId(userId);

            if (patientId <= 0)
            {
                return new List<DashboardSearchResultItem>();
            }

            if (IsType(criteria, "Reservasi Saya"))
            {
                return SearchPatientReservations(patientId, criteria);
            }

            if (IsType(criteria, "Riwayat Pemeriksaan"))
            {
                return SearchPatientExaminations(patientId, criteria);
            }

            if (IsType(criteria, "Obat Saya"))
            {
                return SearchPatientMedicines(patientId, criteria);
            }

            if (IsType(criteria, "Diagnosa Saya"))
            {
                return SearchPatientDiseases(patientId, criteria);
            }

            if (IsType(criteria, "Jadwal Dokter"))
            {
                return SearchPatientDoctorSchedules(criteria);
            }

            return new List<DashboardSearchResultItem>();
        }

        private List<DashboardSearchResultItem> SearchAdminPatients(DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Pasien' AS Category,
                       p.FullName AS Title,
                       CONCAT('No pasien: ', p.PatientNumber) AS Description,
                       'Aktif' AS Status,
                       NULL AS ItemDate,
                       CONCAT('Telepon: ', COALESCE(p.PhoneNumber, '-')) AS ExtraInfo
                FROM patients p
                WHERE (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR COALESCE(p.PhoneNumber, '') LIKE @keywordLike)
                ORDER BY p.FullName
                LIMIT 50;", criteria, null);
        }

        private List<DashboardSearchResultItem> SearchAdminDoctors(DashboardSearchCriteria criteria)
        {
            string statusFilter = GetActiveStatusCondition(criteria, "d.IsActive");

            return QuerySearch(@"
                SELECT 'Dokter' AS Category,
                       d.FullName AS Title,
                       d.Specialization AS Description,
                       CASE WHEN d.IsActive = 1 THEN 'Aktif' ELSE 'Nonaktif' END AS Status,
                       NULL AS ItemDate,
                       CONCAT('Telepon: ', COALESCE(d.PhoneNumber, '-')) AS ExtraInfo
                FROM doctors d
                WHERE (@keyword = '' OR d.FullName LIKE @keywordLike OR d.Specialization LIKE @keywordLike)
                  " + statusFilter + @"
                ORDER BY d.FullName
                LIMIT 50;", criteria, null);
        }

        private List<DashboardSearchResultItem> SearchAdminReservations(DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Reservasi' AS Category,
                       p.FullName AS Title,
                       CONCAT('Dokter: ', d.FullName, ' | Keluhan: ', r.Complaint) AS Description,
                       r.ReservationStatus AS Status,
                       s.ScheduleDate AS ItemDate,
                       CONCAT('Jam: ', DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i')) AS ExtraInfo
                FROM reservations r
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE (@keyword = '' OR p.FullName LIKE @keywordLike OR d.FullName LIKE @keywordLike OR r.Complaint LIKE @keywordLike)
                  AND (@status = 'Semua' OR r.ReservationStatus = @status)
                  AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                  AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                ORDER BY s.ScheduleDate DESC, r.CreatedAt DESC
                LIMIT 50;", criteria, null);
        }

        private List<DashboardSearchResultItem> SearchAdminQueues(DashboardSearchCriteria criteria)
        {
            string statusFilter = string.Empty;

            if (criteria.Status == "Selesai")
            {
                statusFilter = "AND v.VisitStatus = 'Selesai'";
            }
            else if (criteria.Status == "Check-in")
            {
                statusFilter = "AND r.ReservationStatus = 'Check-in'";
            }

            return QuerySearch(@"
                SELECT 'Antrian' AS Category,
                       CONCAT('Antrian ', v.QueueNumber) AS Title,
                       CONCAT(p.FullName, ' | Dokter: ', d.FullName, ' | Keluhan: ', r.Complaint) AS Description,
                       v.VisitStatus AS Status,
                       DATE(v.CheckInTime) AS ItemDate,
                       CONCAT('Check-in: ', DATE_FORMAT(v.CheckInTime, '%H:%i')) AS ExtraInfo
                FROM visits v
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE (@keyword = '' OR p.FullName LIKE @keywordLike OR CAST(v.QueueNumber AS CHAR) LIKE @keywordLike OR v.VisitStatus LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(v.CheckInTime) >= @startDate)
                  AND (@endDate IS NULL OR DATE(v.CheckInTime) <= @endDate)
                  " + statusFilter + @"
                ORDER BY v.CheckInTime DESC, v.QueueNumber
                LIMIT 50;", criteria, null);
        }

        private List<DashboardSearchResultItem> SearchAdminMedicines(DashboardSearchCriteria criteria)
        {
            string statusFilter = string.Empty;

            if (criteria.Status == "Stok Rendah")
            {
                statusFilter = "AND m.Stock <= @threshold";
            }
            else if (criteria.Status == "Aktif")
            {
                statusFilter = "AND m.IsActive = 1";
            }
            else if (criteria.Status == "Nonaktif")
            {
                statusFilter = "AND m.IsActive = 0";
            }

            return QuerySearch(@"
                SELECT 'Obat' AS Category,
                       m.MedicineName AS Title,
                       COALESCE(m.MedicineType, '-') AS Description,
                       CASE WHEN m.Stock <= @threshold THEN 'Stok Rendah' WHEN m.IsActive = 1 THEN 'Aktif' ELSE 'Nonaktif' END AS Status,
                       NULL AS ItemDate,
                       CONCAT('Stok: ', m.Stock, ' ', COALESCE(m.Unit, '')) AS ExtraInfo
                FROM medicines m
                WHERE (@keyword = '' OR m.MedicineName LIKE @keywordLike OR COALESCE(m.MedicineType, '') LIKE @keywordLike)
                  " + statusFilter + @"
                ORDER BY m.Stock ASC, m.MedicineName
                LIMIT 50;", criteria, AddThresholdParameter);
        }

        private List<DashboardSearchResultItem> SearchAdminDiseases(DashboardSearchCriteria criteria)
        {
            string statusFilter = GetActiveStatusCondition(criteria, "d.IsActive");

            return QuerySearch(@"
                SELECT 'Penyakit' AS Category,
                       d.DiseaseName AS Title,
                       CONCAT('Kode: ', d.DiseaseCode) AS Description,
                       CASE WHEN d.IsActive = 1 THEN 'Aktif' ELSE 'Nonaktif' END AS Status,
                       NULL AS ItemDate,
                       COALESCE(d.Description, '-') AS ExtraInfo
                FROM diseases d
                WHERE (@keyword = '' OR d.DiseaseName LIKE @keywordLike OR d.DiseaseCode LIKE @keywordLike)
                  " + statusFilter + @"
                ORDER BY d.DiseaseName
                LIMIT 50;", criteria, null);
        }

        private List<DashboardSearchResultItem> SearchDoctorQueues(int doctorId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Antrian' AS Category,
                       CONCAT('Antrian ', v.QueueNumber) AS Title,
                       CONCAT(p.FullName, ' | Keluhan: ', r.Complaint) AS Description,
                       v.VisitStatus AS Status,
                       DATE(v.CheckInTime) AS ItemDate,
                       CONCAT('Check-in: ', DATE_FORMAT(v.CheckInTime, '%H:%i')) AS ExtraInfo
                FROM visits v
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                WHERE s.DoctorId = @doctorId
                  AND (@keyword = '' OR p.FullName LIKE @keywordLike OR CAST(v.QueueNumber AS CHAR) LIKE @keywordLike OR r.Complaint LIKE @keywordLike OR v.VisitStatus LIKE @keywordLike)
                  AND (@status = 'Semua' OR v.VisitStatus = @status)
                  AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                  AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                ORDER BY v.CheckInTime DESC
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                });
        }

        private List<DashboardSearchResultItem> SearchDoctorExaminations(int doctorId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Riwayat Pemeriksaan' AS Category,
                       p.FullName AS Title,
                       CONCAT('Diagnosa: ', dis.DiseaseName, ' | Keluhan: ', e.CurrentComplaint) AS Description,
                       'Selesai' AS Status,
                       DATE(e.ExaminationDate) AS ItemDate,
                       COALESCE(e.TreatmentNotes, '-') AS ExtraInfo
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                WHERE e.DoctorId = @doctorId
                  AND (@keyword = '' OR p.FullName LIKE @keywordLike OR dis.DiseaseName LIKE @keywordLike OR e.CurrentComplaint LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike OR COALESCE(e.TreatmentNotes, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                });
        }

        private List<DashboardSearchResultItem> SearchDoctorSchedules(int doctorId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Jadwal Praktik' AS Category,
                       DATE_FORMAT(s.ScheduleDate, '%d/%m/%Y') AS Title,
                       CONCAT(DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i')) AS Description,
                       CASE WHEN s.IsActive = 1 THEN 'Aktif' ELSE 'Nonaktif' END AS Status,
                       s.ScheduleDate AS ItemDate,
                       CONCAT('Kuota: ', s.Quota) AS ExtraInfo
                FROM doctor_schedules s
                WHERE s.DoctorId = @doctorId
                  AND (@keyword = '' OR DATE_FORMAT(s.ScheduleDate, '%d/%m/%Y') LIKE @keywordLike)
                  AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                  AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                ORDER BY s.ScheduleDate DESC, s.StartTime
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                });
        }

        private List<DashboardSearchResultItem> SearchDoctorDiseases(int doctorId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Diagnosa/Penyakit' AS Category,
                       dis.DiseaseName AS Title,
                       CONCAT('Kode: ', dis.DiseaseCode) AS Description,
                       'Tertangani' AS Status,
                       MAX(DATE(e.ExaminationDate)) AS ItemDate,
                       CONCAT('Jumlah kasus: ', COUNT(1)) AS ExtraInfo
                FROM examinations e
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                WHERE e.DoctorId = @doctorId
                  AND (@keyword = '' OR dis.DiseaseName LIKE @keywordLike OR dis.DiseaseCode LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                GROUP BY dis.DiseaseId, dis.DiseaseName, dis.DiseaseCode
                ORDER BY COUNT(1) DESC, dis.DiseaseName
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@doctorId", doctorId);
                });
        }

        private List<DashboardSearchResultItem> SearchPatientReservations(int patientId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Reservasi Saya' AS Category,
                       d.FullName AS Title,
                       r.Complaint AS Description,
                       r.ReservationStatus AS Status,
                       s.ScheduleDate AS ItemDate,
                       CONCAT('Jam: ', DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i')) AS ExtraInfo
                FROM reservations r
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE r.PatientId = @patientId
                  AND (@keyword = '' OR d.FullName LIKE @keywordLike OR d.Specialization LIKE @keywordLike OR r.Complaint LIKE @keywordLike)
                  AND (@status = 'Semua' OR r.ReservationStatus = @status)
                  AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                  AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                ORDER BY s.ScheduleDate DESC, r.CreatedAt DESC
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });
        }

        private List<DashboardSearchResultItem> SearchPatientExaminations(int patientId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Riwayat Pemeriksaan' AS Category,
                       dis.DiseaseName AS Title,
                       CONCAT('Dokter: ', d.FullName, ' | Keluhan: ', e.CurrentComplaint) AS Description,
                       'Selesai' AS Status,
                       DATE(e.ExaminationDate) AS ItemDate,
                       COALESCE(e.TreatmentNotes, '-') AS ExtraInfo
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                WHERE r.PatientId = @patientId
                  AND (@keyword = '' OR dis.DiseaseName LIKE @keywordLike OR e.CurrentComplaint LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike OR COALESCE(e.TreatmentNotes, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });
        }

        private List<DashboardSearchResultItem> SearchPatientMedicines(int patientId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Obat Saya' AS Category,
                       m.MedicineName AS Title,
                       COALESCE(m.MedicineType, '-') AS Description,
                       'Diberikan' AS Status,
                       DATE(e.ExaminationDate) AS ItemDate,
                       CONCAT('Jumlah: ', pd.Quantity, ' ', COALESCE(m.Unit, ''), ' | ', COALESCE(pd.InstructionNote, m.DefaultInstruction, '-')) AS ExtraInfo
                FROM prescription_details pd
                INNER JOIN medicines m ON m.MedicineId = pd.MedicineId
                INNER JOIN examinations e ON e.ExaminationId = pd.ExaminationId
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                WHERE r.PatientId = @patientId
                  AND (@keyword = '' OR m.MedicineName LIKE @keywordLike OR COALESCE(m.MedicineType, '') LIKE @keywordLike OR COALESCE(pd.InstructionNote, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });
        }

        private List<DashboardSearchResultItem> SearchPatientDiseases(int patientId, DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Diagnosa Saya' AS Category,
                       dis.DiseaseName AS Title,
                       CONCAT('Kode: ', dis.DiseaseCode) AS Description,
                       'Tercatat' AS Status,
                       MAX(DATE(e.ExaminationDate)) AS ItemDate,
                       CONCAT('Jumlah catatan: ', COUNT(1)) AS ExtraInfo
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                WHERE r.PatientId = @patientId
                  AND (@keyword = '' OR dis.DiseaseName LIKE @keywordLike OR dis.DiseaseCode LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                GROUP BY dis.DiseaseId, dis.DiseaseName, dis.DiseaseCode
                ORDER BY COUNT(1) DESC, dis.DiseaseName
                LIMIT 50;", criteria, delegate(MySqlParameterCollection p)
                {
                    p.AddWithValue("@patientId", patientId);
                });
        }

        private List<DashboardSearchResultItem> SearchPatientDoctorSchedules(DashboardSearchCriteria criteria)
        {
            return QuerySearch(@"
                SELECT 'Jadwal Dokter' AS Category,
                       d.FullName AS Title,
                       CONCAT(d.Specialization, ' | ', DATE_FORMAT(s.ScheduleDate, '%d/%m/%Y')) AS Description,
                       CASE WHEN s.IsActive = 1 THEN 'Aktif' ELSE 'Nonaktif' END AS Status,
                       s.ScheduleDate AS ItemDate,
                       CONCAT(DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i'), ' | Kuota: ', s.Quota) AS ExtraInfo
                FROM doctor_schedules s
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE d.IsActive = 1
                  AND s.IsActive = 1
                  AND (@keyword = '' OR d.FullName LIKE @keywordLike OR d.Specialization LIKE @keywordLike)
                  AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                  AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                ORDER BY s.ScheduleDate, s.StartTime
                LIMIT 50;", criteria, null);
        }

        private static bool ShouldSearch(DashboardSearchCriteria criteria, string allType, string targetType)
        {
            return IsType(criteria, allType) || IsType(criteria, targetType);
        }

        private static bool IsType(DashboardSearchCriteria criteria, string value)
        {
            return string.Equals(criteria.SearchType, value, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetActiveStatusCondition(DashboardSearchCriteria criteria, string columnName)
        {
            if (criteria.Status == "Aktif")
            {
                return "AND " + columnName + " = 1";
            }

            if (criteria.Status == "Nonaktif")
            {
                return "AND " + columnName + " = 0";
            }

            return string.Empty;
        }

        private int GetDoctorIdByUserId(int userId)
        {
            return ExecuteScalarInt("SELECT DoctorId FROM doctors WHERE UserId = @userId LIMIT 1;", delegate(MySqlParameterCollection p)
            {
                p.AddWithValue("@userId", userId);
            });
        }

        private int GetPatientIdByUserId(int userId)
        {
            return ExecuteScalarInt("SELECT PatientId FROM patients WHERE UserId = @userId LIMIT 1;", delegate(MySqlParameterCollection p)
            {
                p.AddWithValue("@userId", userId);
            });
        }

        private static RecentReservationItem MapRecentReservation(MySqlDataReader reader)
        {
            return new RecentReservationItem
            {
                Date = GetNullableDate(reader, "ItemDate"),
                PatientName = GetString(reader, "PatientName"),
                DoctorName = GetString(reader, "DoctorName"),
                Complaint = GetString(reader, "Complaint"),
                Status = GetString(reader, "Status"),
                ExtraInfo = GetString(reader, "ExtraInfo")
            };
        }

        private static QueueItem MapQueue(MySqlDataReader reader)
        {
            return new QueueItem
            {
                Date = GetNullableDate(reader, "ItemDate"),
                QueueNumber = GetString(reader, "QueueNumber"),
                PatientName = GetString(reader, "PatientName"),
                DoctorName = GetString(reader, "DoctorName"),
                Complaint = GetString(reader, "Complaint"),
                Status = GetString(reader, "Status")
            };
        }

        private static LowStockMedicineItem MapLowStockMedicine(MySqlDataReader reader)
        {
            return new LowStockMedicineItem
            {
                MedicineName = GetString(reader, "MedicineName"),
                MedicineType = GetString(reader, "MedicineType"),
                Stock = GetInt(reader, "Stock"),
                Unit = GetString(reader, "Unit"),
                Status = GetBoolean(reader, "IsActive") ? "Aktif" : "Nonaktif"
            };
        }

        private static RecentExaminationItem MapRecentExamination(MySqlDataReader reader)
        {
            return new RecentExaminationItem
            {
                Date = GetNullableDate(reader, "ItemDate"),
                PatientName = GetString(reader, "PatientName"),
                DoctorName = GetString(reader, "DoctorName"),
                DiseaseName = GetString(reader, "DiseaseName"),
                Complaint = GetString(reader, "Complaint"),
                Treatment = GetString(reader, "Treatment")
            };
        }

        private static ChartDataPoint MapChart(MySqlDataReader reader)
        {
            return new ChartDataPoint
            {
                Label = GetString(reader, "Label"),
                Value = GetDouble(reader, "Value")
            };
        }

        private static DashboardSearchResultItem MapSearch(MySqlDataReader reader)
        {
            return new DashboardSearchResultItem
            {
                Category = GetString(reader, "Category"),
                Title = GetString(reader, "Title"),
                Description = GetString(reader, "Description"),
                Status = GetString(reader, "Status"),
                Date = GetNullableDate(reader, "ItemDate"),
                ExtraInfo = GetString(reader, "ExtraInfo")
            };
        }

        private static List<ChartDataPoint> QueryChart(string commandText, Action<MySqlParameterCollection> addParameters)
        {
            return QueryList(commandText, addParameters, MapChart);
        }

        private static List<DashboardSearchResultItem> QuerySearch(string commandText, DashboardSearchCriteria criteria, Action<MySqlParameterCollection> addParameters)
        {
            return QueryList(commandText, delegate(MySqlParameterCollection p)
            {
                AddSearchParameters(p, criteria);

                if (addParameters != null)
                {
                    addParameters(p);
                }
            }, MapSearch);
        }

        private static T QuerySingle<T>(string commandText, Action<MySqlParameterCollection> addParameters, Func<MySqlDataReader, T> map)
            where T : class
        {
            List<T> items = QueryList(commandText, addParameters, map);
            return items.Count == 0 ? null : items[0];
        }

        private static List<T> QueryList<T>(string commandText, Action<MySqlParameterCollection> addParameters, Func<MySqlDataReader, T> map)
        {
            List<T> items = new List<T>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                if (addParameters != null)
                {
                    addParameters(command.Parameters);
                }

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(map(reader));
                    }
                }
            }

            return items;
        }

        private static int ExecuteScalarInt(string commandText, Action<MySqlParameterCollection> addParameters)
        {
            object value = ExecuteScalar(commandText, addParameters);

            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(value);
        }

        private static string ExecuteScalarString(string commandText, Action<MySqlParameterCollection> addParameters)
        {
            object value = ExecuteScalar(commandText, addParameters);
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }

        private static object ExecuteScalar(string commandText, Action<MySqlParameterCollection> addParameters)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                if (addParameters != null)
                {
                    addParameters(command.Parameters);
                }

                return command.ExecuteScalar();
            }
        }

        private static void AddSearchParameters(MySqlParameterCollection parameters, DashboardSearchCriteria criteria)
        {
            string keyword = criteria == null || string.IsNullOrWhiteSpace(criteria.Keyword) ? string.Empty : criteria.Keyword.Trim();
            string status = criteria == null || string.IsNullOrWhiteSpace(criteria.Status) ? "Semua" : criteria.Status.Trim();

            parameters.AddWithValue("@keyword", keyword);
            parameters.AddWithValue("@keywordLike", "%" + keyword + "%");
            parameters.AddWithValue("@status", status);
            parameters.AddWithValue("@startDate", criteria != null && criteria.StartDate.HasValue ? (object)criteria.StartDate.Value.Date : DBNull.Value);
            parameters.AddWithValue("@endDate", criteria != null && criteria.EndDate.HasValue ? (object)criteria.EndDate.Value.Date : DBNull.Value);
        }

        private static void AddThresholdParameter(MySqlParameterCollection parameters)
        {
            parameters.AddWithValue("@threshold", LowStockThreshold);
        }

        private static string GetString(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }

        private static int GetInt(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private static double GetDouble(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value == null || value == DBNull.Value ? 0 : Convert.ToDouble(value);
        }

        private static bool GetBoolean(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private static DateTime? GetNullableDate(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value == null || value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(value);
        }
    }
}
