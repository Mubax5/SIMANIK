using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class ReportRepository
    {
        private const int LowStockThreshold = 10;

        public List<ReportItem> GetReservationsPerDay(DateTime start, DateTime end)
        {
            return Query(@"
                SELECT
                    s.ScheduleDate AS ItemDate,
                    DATE_FORMAT(s.ScheduleDate, '%d/%m/%Y') AS Label,
                    '' AS DoctorName,
                    '' AS PatientName,
                    '' AS DiseaseName,
                    '' AS MedicineName,
                    COUNT(1) AS Total,
                    0 AS Stock,
                    '' AS Unit,
                    'Reservasi berdasarkan tanggal jadwal' AS ExtraInfo
                FROM reservations r
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                WHERE s.ScheduleDate BETWEEN @startDate AND @endDate
                GROUP BY s.ScheduleDate
                ORDER BY s.ScheduleDate;", delegate(MySqlParameterCollection p)
            {
                AddDateParameters(p, start, end);
            });
        }

        public List<ReportItem> GetReservationsPerDoctor(DateTime start, DateTime end)
        {
            return Query(@"
                SELECT
                    NULL AS ItemDate,
                    d.FullName AS Label,
                    d.FullName AS DoctorName,
                    '' AS PatientName,
                    '' AS DiseaseName,
                    '' AS MedicineName,
                    COUNT(r.ReservationId) AS Total,
                    0 AS Stock,
                    '' AS Unit,
                    d.Specialization AS ExtraInfo
                FROM reservations r
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE d.IsActive = 1
                  AND s.ScheduleDate BETWEEN @startDate AND @endDate
                GROUP BY d.DoctorId, d.FullName, d.Specialization
                ORDER BY Total DESC, d.FullName;", delegate(MySqlParameterCollection p)
            {
                AddDateParameters(p, start, end);
            });
        }

        public List<ReportItem> GetCompletedVisitsPerDay(DateTime start, DateTime end)
        {
            return Query(@"
                SELECT
                    DATE(v.CheckInTime) AS ItemDate,
                    DATE_FORMAT(DATE(v.CheckInTime), '%d/%m/%Y') AS Label,
                    '' AS DoctorName,
                    '' AS PatientName,
                    '' AS DiseaseName,
                    '' AS MedicineName,
                    COUNT(1) AS Total,
                    0 AS Stock,
                    '' AS Unit,
                    'Kunjungan status Selesai' AS ExtraInfo
                FROM visits v
                WHERE v.VisitStatus = 'Selesai'
                  AND DATE(v.CheckInTime) BETWEEN @startDate AND @endDate
                GROUP BY DATE(v.CheckInTime)
                ORDER BY DATE(v.CheckInTime);", delegate(MySqlParameterCollection p)
            {
                AddDateParameters(p, start, end);
            });
        }

        public List<ReportItem> GetPatientCountPerDoctor(DateTime start, DateTime end)
        {
            return Query(@"
                SELECT
                    NULL AS ItemDate,
                    d.FullName AS Label,
                    d.FullName AS DoctorName,
                    '' AS PatientName,
                    '' AS DiseaseName,
                    '' AS MedicineName,
                    COUNT(DISTINCT r.PatientId) AS Total,
                    0 AS Stock,
                    '' AS Unit,
                    d.Specialization AS ExtraInfo
                FROM reservations r
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                WHERE d.IsActive = 1
                  AND s.ScheduleDate BETWEEN @startDate AND @endDate
                GROUP BY d.DoctorId, d.FullName, d.Specialization
                ORDER BY Total DESC, d.FullName;", delegate(MySqlParameterCollection p)
            {
                AddDateParameters(p, start, end);
            });
        }

        public List<ReportItem> GetMostFrequentDiseases(DateTime start, DateTime end)
        {
            return Query(@"
                SELECT
                    NULL AS ItemDate,
                    dis.DiseaseName AS Label,
                    '' AS DoctorName,
                    '' AS PatientName,
                    dis.DiseaseName,
                    '' AS MedicineName,
                    COUNT(e.ExaminationId) AS Total,
                    0 AS Stock,
                    '' AS Unit,
                    CONCAT('Kode: ', dis.DiseaseCode) AS ExtraInfo
                FROM examinations e
                INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                WHERE dis.IsActive = 1
                  AND DATE(e.ExaminationDate) BETWEEN @startDate AND @endDate
                GROUP BY dis.DiseaseId, dis.DiseaseCode, dis.DiseaseName
                ORDER BY Total DESC, dis.DiseaseName
                LIMIT 20;", delegate(MySqlParameterCollection p)
            {
                AddDateParameters(p, start, end);
            });
        }

        public List<ReportItem> GetMostUsedMedicines(DateTime start, DateTime end)
        {
            return Query(@"
                SELECT
                    NULL AS ItemDate,
                    m.MedicineName AS Label,
                    '' AS DoctorName,
                    '' AS PatientName,
                    '' AS DiseaseName,
                    m.MedicineName,
                    COALESCE(SUM(pd.Quantity), 0) AS Total,
                    m.Stock,
                    COALESCE(m.Unit, '') AS Unit,
                    COALESCE(m.MedicineType, '') AS ExtraInfo
                FROM prescription_details pd
                INNER JOIN medicines m ON m.MedicineId = pd.MedicineId
                INNER JOIN examinations e ON e.ExaminationId = pd.ExaminationId
                WHERE m.IsActive = 1
                  AND DATE(e.ExaminationDate) BETWEEN @startDate AND @endDate
                GROUP BY m.MedicineId, m.MedicineName, m.MedicineType, m.Stock, m.Unit
                ORDER BY Total DESC, m.MedicineName
                LIMIT 20;", delegate(MySqlParameterCollection p)
            {
                AddDateParameters(p, start, end);
            });
        }

        public List<ReportItem> GetLowStockMedicines()
        {
            return Query(@"
                SELECT
                    NULL AS ItemDate,
                    m.MedicineName AS Label,
                    '' AS DoctorName,
                    '' AS PatientName,
                    '' AS DiseaseName,
                    m.MedicineName,
                    0 AS Total,
                    m.Stock,
                    COALESCE(m.Unit, '') AS Unit,
                    COALESCE(m.MedicineType, '') AS ExtraInfo
                FROM medicines m
                WHERE m.IsActive = 1
                  AND m.Stock <= @threshold
                ORDER BY m.Stock ASC, m.MedicineName;", delegate(MySqlParameterCollection p)
            {
                p.AddWithValue("@threshold", LowStockThreshold);
            });
        }

        public List<ReportItem> GetExaminationsPerPeriod(DateTime start, DateTime end)
        {
            return Query(@"
                SELECT
                    DATE(e.ExaminationDate) AS ItemDate,
                    DATE_FORMAT(DATE(e.ExaminationDate), '%d/%m/%Y') AS Label,
                    d.FullName AS DoctorName,
                    '' AS PatientName,
                    '' AS DiseaseName,
                    '' AS MedicineName,
                    COUNT(e.ExaminationId) AS Total,
                    0 AS Stock,
                    '' AS Unit,
                    'Pemeriksaan selesai' AS ExtraInfo
                FROM examinations e
                INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                WHERE DATE(e.ExaminationDate) BETWEEN @startDate AND @endDate
                GROUP BY DATE(e.ExaminationDate), d.DoctorId, d.FullName
                ORDER BY DATE(e.ExaminationDate), d.FullName;", delegate(MySqlParameterCollection p)
            {
                AddDateParameters(p, start, end);
            });
        }

        private static List<ReportItem> Query(string commandText, Action<MySqlParameterCollection> addParameters)
        {
            List<ReportItem> items = new List<ReportItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                addParameters(command.Parameters);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ReportItem
                        {
                            Date = GetNullableDate(reader, "ItemDate"),
                            Label = GetString(reader, "Label"),
                            DoctorName = GetString(reader, "DoctorName"),
                            PatientName = GetString(reader, "PatientName"),
                            DiseaseName = GetString(reader, "DiseaseName"),
                            MedicineName = GetString(reader, "MedicineName"),
                            Total = GetInt(reader, "Total"),
                            Stock = GetInt(reader, "Stock"),
                            Unit = GetString(reader, "Unit"),
                            ExtraInfo = GetString(reader, "ExtraInfo")
                        });
                    }
                }
            }

            return items;
        }

        private static void AddDateParameters(MySqlParameterCollection parameters, DateTime start, DateTime end)
        {
            parameters.AddWithValue("@startDate", start.Date);
            parameters.AddWithValue("@endDate", end.Date);
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
