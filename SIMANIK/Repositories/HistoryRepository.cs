using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class HistoryRepository
    {
        public List<HistoryItem> GetPatientReservationHistory(int userId, string keyword, DateTime? startDate, DateTime? endDate)
        {
            return QueryHistory(@"
                SELECT
                    'Reservasi' AS Category,
                    s.ScheduleDate AS ItemDate,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    d.FullName AS DoctorName,
                    '' AS DiseaseName,
                    '' AS MedicineName,
                    0 AS Quantity,
                    '' AS InstructionNote,
                    r.Complaint,
                    '' AS DiagnosisNotes,
                    '' AS TreatmentNotes,
                    r.ReservationStatus AS Status,
                    CONCAT('Jadwal: ', DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i')) AS ExtraInfo,
                    CONCAT('Gol darah: ', COALESCE(mr.BloodType, '-'), ' | Alergi: ', COALESCE(mr.AllergyNotes, '-'), ' | Kronis: ', COALESCE(mr.ChronicDiseaseNotes, '-')) AS MedicalRecordSummary
                FROM reservations r
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                LEFT JOIN medical_records mr ON mr.PatientId = p.PatientId
                WHERE p.UserId = @userId
                  AND (@keyword = '' OR d.FullName LIKE @keywordLike OR r.Complaint LIKE @keywordLike OR r.ReservationStatus LIKE @keywordLike)
                  AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                  AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                ORDER BY s.ScheduleDate DESC, r.CreatedAt DESC;", delegate(MySqlParameterCollection p)
            {
                AddCommonParameters(p, keyword, startDate, endDate);
                p.AddWithValue("@userId", userId);
            });
        }

        public List<HistoryItem> GetPatientExaminationHistory(int userId, string keyword, DateTime? startDate, DateTime? endDate)
        {
            return QueryHistory(BaseExaminationHistorySelect() + @"
                WHERE p.UserId = @userId
                  AND (@keyword = '' OR d.FullName LIKE @keywordLike OR dis.DiseaseName LIKE @keywordLike OR e.CurrentComplaint LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike OR COALESCE(e.TreatmentNotes, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC;", delegate(MySqlParameterCollection p)
            {
                AddCommonParameters(p, keyword, startDate, endDate);
                p.AddWithValue("@userId", userId);
            });
        }

        public List<HistoryItem> GetPatientMedicineHistory(int userId, string keyword, DateTime? startDate, DateTime? endDate)
        {
            return QueryHistory(BaseMedicineHistorySelect() + @"
                WHERE p.UserId = @userId
                  AND (@keyword = '' OR d.FullName LIKE @keywordLike OR m.MedicineName LIKE @keywordLike OR COALESCE(pd.InstructionNote, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC, m.MedicineName;", delegate(MySqlParameterCollection p)
            {
                AddCommonParameters(p, keyword, startDate, endDate);
                p.AddWithValue("@userId", userId);
            });
        }

        public List<HistoryItem> GetPatientDiagnosisHistory(int userId, string keyword, DateTime? startDate, DateTime? endDate)
        {
            return QueryHistory(BaseDiagnosisHistorySelect() + @"
                WHERE p.UserId = @userId
                  AND (@keyword = '' OR d.FullName LIKE @keywordLike OR dis.DiseaseName LIKE @keywordLike OR dis.DiseaseCode LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC, dis.DiseaseName;", delegate(MySqlParameterCollection p)
            {
                AddCommonParameters(p, keyword, startDate, endDate);
                p.AddWithValue("@userId", userId);
            });
        }

        public List<HistoryItem> GetDoctorPatientHistory(int userId, string historyType, string keyword, DateTime? startDate, DateTime? endDate)
        {
            int doctorId = GetDoctorIdByUserId(userId);
            if (doctorId <= 0)
            {
                return new List<HistoryItem>();
            }

            if (IsType(historyType, "Reservasi"))
            {
                return GetDoctorReservationHistory(doctorId, keyword, startDate, endDate);
            }

            if (IsType(historyType, "Obat"))
            {
                return QueryHistory(BaseMedicineHistorySelect() + @"
                    WHERE e.DoctorId = @doctorId
                      AND (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR m.MedicineName LIKE @keywordLike OR COALESCE(pd.InstructionNote, '') LIKE @keywordLike)
                      AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                      AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                    ORDER BY e.ExaminationDate DESC, p.FullName, m.MedicineName;", delegate(MySqlParameterCollection p)
                {
                    AddCommonParameters(p, keyword, startDate, endDate);
                    p.AddWithValue("@doctorId", doctorId);
                });
            }

            if (IsType(historyType, "Diagnosa"))
            {
                return QueryHistory(BaseDiagnosisHistorySelect() + @"
                    WHERE e.DoctorId = @doctorId
                      AND (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR dis.DiseaseName LIKE @keywordLike OR dis.DiseaseCode LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike)
                      AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                      AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                    ORDER BY e.ExaminationDate DESC, p.FullName, dis.DiseaseName;", delegate(MySqlParameterCollection p)
                {
                    AddCommonParameters(p, keyword, startDate, endDate);
                    p.AddWithValue("@doctorId", doctorId);
                });
            }

            return QueryHistory(BaseExaminationHistorySelect() + @"
                WHERE e.DoctorId = @doctorId
                  AND (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR dis.DiseaseName LIKE @keywordLike OR e.CurrentComplaint LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike OR COALESCE(e.TreatmentNotes, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC, p.FullName;", delegate(MySqlParameterCollection p)
            {
                AddCommonParameters(p, keyword, startDate, endDate);
                p.AddWithValue("@doctorId", doctorId);
            });
        }

        public List<HistoryItem> GetAdminHistory(string historyType, string keyword, DateTime? startDate, DateTime? endDate)
        {
            if (IsType(historyType, "Reservasi"))
            {
                return QueryHistory(AdminReservationHistorySelect() + @"
                    WHERE (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR d.FullName LIKE @keywordLike OR r.Complaint LIKE @keywordLike OR r.ReservationStatus LIKE @keywordLike)
                      AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                      AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                    ORDER BY s.ScheduleDate DESC, r.CreatedAt DESC;", delegate(MySqlParameterCollection p)
                {
                    AddCommonParameters(p, keyword, startDate, endDate);
                });
            }

            if (IsType(historyType, "Obat"))
            {
                return QueryHistory(BaseMedicineHistorySelect() + @"
                    WHERE (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR d.FullName LIKE @keywordLike OR m.MedicineName LIKE @keywordLike OR COALESCE(pd.InstructionNote, '') LIKE @keywordLike)
                      AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                      AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                    ORDER BY e.ExaminationDate DESC, p.FullName, m.MedicineName;", delegate(MySqlParameterCollection p)
                {
                    AddCommonParameters(p, keyword, startDate, endDate);
                });
            }

            if (IsType(historyType, "Diagnosa"))
            {
                return QueryHistory(BaseDiagnosisHistorySelect() + @"
                    WHERE (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR d.FullName LIKE @keywordLike OR dis.DiseaseName LIKE @keywordLike OR dis.DiseaseCode LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike)
                      AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                      AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                    ORDER BY e.ExaminationDate DESC, p.FullName, dis.DiseaseName;", delegate(MySqlParameterCollection p)
                {
                    AddCommonParameters(p, keyword, startDate, endDate);
                });
            }

            return QueryHistory(BaseExaminationHistorySelect() + @"
                WHERE (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR d.FullName LIKE @keywordLike OR dis.DiseaseName LIKE @keywordLike OR e.CurrentComplaint LIKE @keywordLike OR COALESCE(e.DiagnosisNotes, '') LIKE @keywordLike OR COALESCE(e.TreatmentNotes, '') LIKE @keywordLike)
                  AND (@startDate IS NULL OR DATE(e.ExaminationDate) >= @startDate)
                  AND (@endDate IS NULL OR DATE(e.ExaminationDate) <= @endDate)
                ORDER BY e.ExaminationDate DESC, p.FullName;", delegate(MySqlParameterCollection p)
            {
                AddCommonParameters(p, keyword, startDate, endDate);
            });
        }

        private List<HistoryItem> GetDoctorReservationHistory(int doctorId, string keyword, DateTime? startDate, DateTime? endDate)
        {
            return QueryHistory(AdminReservationHistorySelect() + @"
                WHERE s.DoctorId = @doctorId
                  AND (@keyword = '' OR p.FullName LIKE @keywordLike OR p.PatientNumber LIKE @keywordLike OR r.Complaint LIKE @keywordLike OR r.ReservationStatus LIKE @keywordLike)
                  AND (@startDate IS NULL OR s.ScheduleDate >= @startDate)
                  AND (@endDate IS NULL OR s.ScheduleDate <= @endDate)
                ORDER BY s.ScheduleDate DESC, r.CreatedAt DESC;", delegate(MySqlParameterCollection p)
            {
                AddCommonParameters(p, keyword, startDate, endDate);
                p.AddWithValue("@doctorId", doctorId);
            });
        }

        private static string AdminReservationHistorySelect()
        {
            return @"
                SELECT
                    'Reservasi' AS Category,
                    s.ScheduleDate AS ItemDate,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    d.FullName AS DoctorName,
                    '' AS DiseaseName,
                    '' AS MedicineName,
                    0 AS Quantity,
                    '' AS InstructionNote,
                    r.Complaint,
                    '' AS DiagnosisNotes,
                    '' AS TreatmentNotes,
                    r.ReservationStatus AS Status,
                    CONCAT('Jadwal: ', DATE_FORMAT(s.StartTime, '%H:%i'), ' - ', DATE_FORMAT(s.EndTime, '%H:%i')) AS ExtraInfo,
                    CONCAT('Gol darah: ', COALESCE(mr.BloodType, '-'), ' | Alergi: ', COALESCE(mr.AllergyNotes, '-'), ' | Kronis: ', COALESCE(mr.ChronicDiseaseNotes, '-')) AS MedicalRecordSummary
                FROM reservations r
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                LEFT JOIN medical_records mr ON mr.PatientId = p.PatientId";
        }

        private static string BaseExaminationHistorySelect()
        {
            return @"
                SELECT
                    'Pemeriksaan' AS Category,
                    e.ExaminationDate AS ItemDate,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    d.FullName AS DoctorName,
                    dis.DiseaseName,
                    '' AS MedicineName,
                    0 AS Quantity,
                    '' AS InstructionNote,
                    e.CurrentComplaint AS Complaint,
                    COALESCE(e.DiagnosisNotes, '') AS DiagnosisNotes,
                    COALESCE(e.TreatmentNotes, '') AS TreatmentNotes,
                    v.VisitStatus AS Status,
                    CONCAT('Reservasi: ', r.ReservationStatus) AS ExtraInfo,
                    CONCAT('Gol darah: ', COALESCE(mr.BloodType, '-'), ' | Alergi: ', COALESCE(mr.AllergyNotes, '-'), ' | Kronis: ', COALESCE(mr.ChronicDiseaseNotes, '-')) AS MedicalRecordSummary
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                LEFT JOIN medical_records mr ON mr.PatientId = p.PatientId";
        }

        private static string BaseMedicineHistorySelect()
        {
            return @"
                SELECT
                    'Obat' AS Category,
                    e.ExaminationDate AS ItemDate,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    d.FullName AS DoctorName,
                    dis.DiseaseName,
                    m.MedicineName,
                    pd.Quantity,
                    COALESCE(pd.InstructionNote, m.DefaultInstruction, '') AS InstructionNote,
                    e.CurrentComplaint AS Complaint,
                    COALESCE(e.DiagnosisNotes, '') AS DiagnosisNotes,
                    COALESCE(e.TreatmentNotes, '') AS TreatmentNotes,
                    'Diberikan' AS Status,
                    CONCAT('Satuan: ', COALESCE(m.Unit, '-')) AS ExtraInfo,
                    CONCAT('Gol darah: ', COALESCE(mr.BloodType, '-'), ' | Alergi: ', COALESCE(mr.AllergyNotes, '-'), ' | Kronis: ', COALESCE(mr.ChronicDiseaseNotes, '-')) AS MedicalRecordSummary
                FROM prescription_details pd
                INNER JOIN medicines m ON m.MedicineId = pd.MedicineId
                INNER JOIN examinations e ON e.ExaminationId = pd.ExaminationId
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                LEFT JOIN medical_records mr ON mr.PatientId = p.PatientId";
        }

        private static string BaseDiagnosisHistorySelect()
        {
            return @"
                SELECT
                    'Diagnosa' AS Category,
                    e.ExaminationDate AS ItemDate,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    d.FullName AS DoctorName,
                    dis.DiseaseName,
                    '' AS MedicineName,
                    0 AS Quantity,
                    '' AS InstructionNote,
                    e.CurrentComplaint AS Complaint,
                    COALESCE(e.DiagnosisNotes, '') AS DiagnosisNotes,
                    COALESCE(e.TreatmentNotes, '') AS TreatmentNotes,
                    'Tercatat' AS Status,
                    CONCAT('Kode: ', dis.DiseaseCode) AS ExtraInfo,
                    CONCAT('Gol darah: ', COALESCE(mr.BloodType, '-'), ' | Alergi: ', COALESCE(mr.AllergyNotes, '-'), ' | Kronis: ', COALESCE(mr.ChronicDiseaseNotes, '-')) AS MedicalRecordSummary
                FROM examinations e
                INNER JOIN visits v ON v.VisitId = e.VisitId
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                LEFT JOIN medical_records mr ON mr.PatientId = p.PatientId";
        }

        private int GetDoctorIdByUserId(int userId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT DoctorId FROM doctors WHERE UserId = @userId LIMIT 1;";
                command.Parameters.AddWithValue("@userId", userId);

                object value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
            }
        }

        private static List<HistoryItem> QueryHistory(string commandText, Action<MySqlParameterCollection> addParameters)
        {
            List<HistoryItem> items = new List<HistoryItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                addParameters(command.Parameters);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new HistoryItem
                        {
                            Category = GetString(reader, "Category"),
                            Date = GetNullableDate(reader, "ItemDate"),
                            PatientName = GetString(reader, "PatientName"),
                            PatientNumber = GetString(reader, "PatientNumber"),
                            DoctorName = GetString(reader, "DoctorName"),
                            DiseaseName = GetString(reader, "DiseaseName"),
                            MedicineName = GetString(reader, "MedicineName"),
                            Quantity = GetInt(reader, "Quantity"),
                            InstructionNote = GetString(reader, "InstructionNote"),
                            Complaint = GetString(reader, "Complaint"),
                            DiagnosisNotes = GetString(reader, "DiagnosisNotes"),
                            TreatmentNotes = GetString(reader, "TreatmentNotes"),
                            Status = GetString(reader, "Status"),
                            ExtraInfo = GetString(reader, "ExtraInfo"),
                            MedicalRecordSummary = GetString(reader, "MedicalRecordSummary")
                        });
                    }
                }
            }

            return items;
        }

        private static void AddCommonParameters(MySqlParameterCollection parameters, string keyword, DateTime? startDate, DateTime? endDate)
        {
            string cleanKeyword = string.IsNullOrWhiteSpace(keyword) ? string.Empty : keyword.Trim();
            parameters.AddWithValue("@keyword", cleanKeyword);
            parameters.AddWithValue("@keywordLike", "%" + cleanKeyword + "%");
            parameters.AddWithValue("@startDate", startDate.HasValue ? (object)startDate.Value.Date : DBNull.Value);
            parameters.AddWithValue("@endDate", endDate.HasValue ? (object)endDate.Value.Date : DBNull.Value);
        }

        private static bool IsType(string historyType, string value)
        {
            return string.Equals(historyType, value, StringComparison.OrdinalIgnoreCase);
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

        private static DateTime? GetNullableDate(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value == null || value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(value);
        }
    }
}
