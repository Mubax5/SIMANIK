using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class ExaminationRepository
    {
        public List<DoctorQueueExaminationItem> GetDoctorQueueForExamination(int userId)
        {
            List<DoctorQueueExaminationItem> items = new List<DoctorQueueExaminationItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        v.VisitId,
                        v.ReservationId,
                        v.QueueNumber,
                        p.FullName AS PatientName,
                        p.PatientNumber,
                        d.FullName AS DoctorName,
                        s.ScheduleDate,
                        s.StartTime,
                        s.EndTime,
                        r.Complaint,
                        v.VisitStatus,
                        v.CheckInTime
                    FROM visits v
                    INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                    INNER JOIN patients p ON p.PatientId = r.PatientId
                    INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                    INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                    WHERE DATE(v.CheckInTime) = CURDATE()
                      AND d.UserId = @userId
                      AND v.VisitStatus IN ('Menunggu', 'Sedang Diperiksa')
                    ORDER BY v.QueueNumber, v.CheckInTime;";

                command.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new DoctorQueueExaminationItem
                        {
                            VisitId = GetInt(reader, "VisitId"),
                            ReservationId = GetInt(reader, "ReservationId"),
                            QueueNumber = GetInt(reader, "QueueNumber"),
                            PatientName = GetString(reader, "PatientName"),
                            PatientNumber = GetString(reader, "PatientNumber"),
                            DoctorName = GetString(reader, "DoctorName"),
                            ScheduleDate = GetDate(reader, "ScheduleDate"),
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"],
                            Complaint = GetString(reader, "Complaint"),
                            VisitStatus = GetString(reader, "VisitStatus"),
                            CheckInTime = GetDate(reader, "CheckInTime")
                        });
                    }
                }
            }

            return items;
        }

        public PatientExaminationDetail GetPatientDetailByVisitId(int visitId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return GetPatientDetailByVisitId(visitId, connection, null);
            }
        }

        public PatientExaminationDetail GetPatientDetailByVisitId(int visitId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT
                        v.VisitId,
                        v.ReservationId,
                        v.VisitStatus,
                        v.CheckInTime,
                        r.ReservationStatus,
                        r.PatientId,
                        r.Complaint,
                        p.FullName AS PatientName,
                        p.PatientNumber,
                        TIMESTAMPDIFF(YEAR, p.BirthDate, CURDATE()) AS Age,
                        p.Gender,
                        p.PhoneNumber,
                        d.DoctorId,
                        d.UserId AS DoctorUserId
                    FROM visits v
                    INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                    INNER JOIN patients p ON p.PatientId = r.PatientId
                    INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                    INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                    WHERE v.VisitId = @visitId
                    LIMIT 1;";

                command.Parameters.AddWithValue("@visitId", visitId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new PatientExaminationDetail
                    {
                        VisitId = GetInt(reader, "VisitId"),
                        ReservationId = GetInt(reader, "ReservationId"),
                        PatientId = GetInt(reader, "PatientId"),
                        DoctorId = GetInt(reader, "DoctorId"),
                        DoctorUserId = GetInt(reader, "DoctorUserId"),
                        VisitStatus = GetString(reader, "VisitStatus"),
                        ReservationStatus = GetString(reader, "ReservationStatus"),
                        PatientName = GetString(reader, "PatientName"),
                        PatientNumber = GetString(reader, "PatientNumber"),
                        Age = GetInt(reader, "Age"),
                        Gender = GetString(reader, "Gender"),
                        PhoneNumber = GetString(reader, "PhoneNumber"),
                        InitialComplaint = GetString(reader, "Complaint"),
                        CheckInTime = GetDate(reader, "CheckInTime")
                    };
                }
            }
        }

        public List<PatientExaminationHistoryItem> GetPatientHistoryByVisitId(int visitId)
        {
            List<PatientExaminationHistoryItem> items = new List<PatientExaminationHistoryItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        e.ExaminationDate,
                        d.FullName AS DoctorName,
                        dis.DiseaseName,
                        e.CurrentComplaint,
                        COALESCE(e.DiagnosisNotes, '') AS DiagnosisNotes,
                        COALESCE(e.TreatmentNotes, '') AS TreatmentNotes,
                        COALESCE(GROUP_CONCAT(CONCAT(m.MedicineName, ' x', pd.Quantity, ' ', COALESCE(m.Unit, '')) SEPARATOR ', '), '') AS Medicines
                    FROM examinations e
                    INNER JOIN visits v ON v.VisitId = e.VisitId
                    INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                    INNER JOIN doctors d ON d.DoctorId = e.DoctorId
                    INNER JOIN diseases dis ON dis.DiseaseId = e.DiseaseId
                    LEFT JOIN prescription_details pd ON pd.ExaminationId = e.ExaminationId
                    LEFT JOIN medicines m ON m.MedicineId = pd.MedicineId
                    WHERE r.PatientId = (
                        SELECT r2.PatientId
                        FROM visits v2
                        INNER JOIN reservations r2 ON r2.ReservationId = v2.ReservationId
                        WHERE v2.VisitId = @visitId
                        LIMIT 1
                    )
                      AND v.VisitId <> @visitId
                    GROUP BY e.ExaminationId, e.ExaminationDate, d.FullName, dis.DiseaseName, e.CurrentComplaint, e.DiagnosisNotes, e.TreatmentNotes
                    ORDER BY e.ExaminationDate DESC;";

                command.Parameters.AddWithValue("@visitId", visitId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new PatientExaminationHistoryItem
                        {
                            ExaminationDate = GetDate(reader, "ExaminationDate"),
                            DoctorName = GetString(reader, "DoctorName"),
                            DiseaseName = GetString(reader, "DiseaseName"),
                            CurrentComplaint = GetString(reader, "CurrentComplaint"),
                            DiagnosisNotes = GetString(reader, "DiagnosisNotes"),
                            TreatmentNotes = GetString(reader, "TreatmentNotes"),
                            Medicines = GetString(reader, "Medicines")
                        });
                    }
                }
            }

            return items;
        }

        public List<DiseaseItem> GetActiveDiseases()
        {
            List<DiseaseItem> items = new List<DiseaseItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DiseaseId, DiseaseCode, DiseaseName, Description, IsActive
                    FROM diseases
                    WHERE IsActive = 1
                    ORDER BY DiseaseName;";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(MapDisease(reader));
                    }
                }
            }

            return items;
        }

        public List<MedicineItem> GetActiveMedicines()
        {
            List<MedicineItem> items = new List<MedicineItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT MedicineId, MedicineName, MedicineType, Stock, Unit, DefaultInstruction, IsActive
                    FROM medicines
                    WHERE IsActive = 1
                    ORDER BY MedicineName;";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(MapMedicine(reader));
                    }
                }
            }

            return items;
        }

        public MedicineItem GetMedicineById(int medicineId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return GetMedicineById(medicineId, connection, null, false);
            }
        }

        public MedicineItem GetMedicineById(int medicineId, MySqlConnection connection, MySqlTransaction transaction, bool lockRow)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT MedicineId, MedicineName, MedicineType, Stock, Unit, DefaultInstruction, IsActive
                    FROM medicines
                    WHERE MedicineId = @medicineId
                    LIMIT 1" + (lockRow ? " FOR UPDATE;" : ";");

                command.Parameters.AddWithValue("@medicineId", medicineId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapMedicine(reader) : null;
                }
            }
        }

        public DiseaseItem GetDiseaseById(int diseaseId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT DiseaseId, DiseaseCode, DiseaseName, Description, IsActive
                    FROM diseases
                    WHERE DiseaseId = @diseaseId
                    LIMIT 1;";

                command.Parameters.AddWithValue("@diseaseId", diseaseId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapDisease(reader) : null;
                }
            }
        }

        public bool VisitHasExamination(int visitId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return VisitHasExamination(visitId, connection, null);
            }
        }

        public bool VisitHasExamination(int visitId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(1) FROM examinations WHERE VisitId = @visitId;";
                command.Parameters.AddWithValue("@visitId", visitId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public int StartExamination(int visitId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return StartExamination(visitId, connection, null);
            }
        }

        public int StartExamination(int visitId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE visits
                    SET VisitStatus = @inProgress
                    WHERE VisitId = @visitId
                      AND VisitStatus = @waiting;";

                command.Parameters.AddWithValue("@inProgress", VisitStatusText.InProgress);
                command.Parameters.AddWithValue("@waiting", VisitStatusText.Waiting);
                command.Parameters.AddWithValue("@visitId", visitId);
                return command.ExecuteNonQuery();
            }
        }

        public int CreateExamination(
            int visitId,
            int doctorId,
            int diseaseId,
            string currentComplaint,
            string diagnosisNotes,
            string treatmentNotes,
            MySqlConnection connection,
            MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO examinations
                        (VisitId, DoctorId, DiseaseId, ExaminationDate, CurrentComplaint, DiagnosisNotes, TreatmentNotes)
                    VALUES
                        (@visitId, @doctorId, @diseaseId, NOW(), @currentComplaint, @diagnosisNotes, @treatmentNotes);";

                command.Parameters.AddWithValue("@visitId", visitId);
                command.Parameters.AddWithValue("@doctorId", doctorId);
                command.Parameters.AddWithValue("@diseaseId", diseaseId);
                command.Parameters.AddWithValue("@currentComplaint", currentComplaint);
                command.Parameters.AddWithValue("@diagnosisNotes", ToDbText(diagnosisNotes));
                command.Parameters.AddWithValue("@treatmentNotes", ToDbText(treatmentNotes));
                command.ExecuteNonQuery();
            }

            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT LAST_INSERT_ID();";
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int UpdateVisitStatus(int visitId, string status)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return UpdateVisitStatus(visitId, status, connection, null);
            }
        }

        public int UpdateVisitStatus(int visitId, string status, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE visits
                    SET VisitStatus = @status
                    WHERE VisitId = @visitId;";

                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@visitId", visitId);
                return command.ExecuteNonQuery();
            }
        }

        public int UpdateReservationStatusByVisitId(int visitId, string status)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return UpdateReservationStatusByVisitId(visitId, status, connection, null);
            }
        }

        public int UpdateReservationStatusByVisitId(int visitId, string status, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE reservations r
                    INNER JOIN visits v ON v.ReservationId = r.ReservationId
                    SET r.ReservationStatus = @status
                    WHERE v.VisitId = @visitId;";

                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@visitId", visitId);
                return command.ExecuteNonQuery();
            }
        }

        private static DiseaseItem MapDisease(MySqlDataReader reader)
        {
            return new DiseaseItem
            {
                DiseaseId = GetInt(reader, "DiseaseId"),
                DiseaseCode = GetString(reader, "DiseaseCode"),
                DiseaseName = GetString(reader, "DiseaseName"),
                Description = GetString(reader, "Description"),
                IsActive = GetBoolean(reader, "IsActive")
            };
        }

        private static MedicineItem MapMedicine(MySqlDataReader reader)
        {
            return new MedicineItem
            {
                MedicineId = GetInt(reader, "MedicineId"),
                MedicineName = GetString(reader, "MedicineName"),
                MedicineType = GetString(reader, "MedicineType"),
                Stock = GetInt(reader, "Stock"),
                Unit = GetString(reader, "Unit"),
                DefaultInstruction = GetString(reader, "DefaultInstruction"),
                IsActive = GetBoolean(reader, "IsActive")
            };
        }

        private static object ToDbText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
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

        private static bool GetBoolean(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private static DateTime GetDate(MySqlDataReader reader, string columnName)
        {
            object value = reader[columnName];
            return value == null || value == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(value);
        }
    }
}
