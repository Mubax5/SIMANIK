using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class MedicalRecordRepository
    {
        public MedicalRecordViewItem GetByPatientUserId(int userId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = BaseMedicalRecordViewSelect() + @"
                    WHERE p.UserId = @userId
                    LIMIT 1;";

                command.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapView(reader) : null;
                }
            }
        }

        public List<MedicalRecordViewItem> Search(string keyword)
        {
            List<MedicalRecordViewItem> items = new List<MedicalRecordViewItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                string cleanKeyword = string.IsNullOrWhiteSpace(keyword) ? string.Empty : keyword.Trim();
                command.CommandText = BaseMedicalRecordViewSelect() + @"
                    WHERE (@keyword = ''
                           OR p.FullName LIKE @keywordLike
                           OR p.PatientNumber LIKE @keywordLike
                           OR p.PhoneNumber LIKE @keywordLike
                           OR COALESCE(mr.BloodType, '') LIKE @keywordLike
                           OR COALESCE(mr.AllergyNotes, '') LIKE @keywordLike
                           OR COALESCE(mr.ChronicDiseaseNotes, '') LIKE @keywordLike)
                    ORDER BY p.FullName;";

                command.Parameters.AddWithValue("@keyword", cleanKeyword);
                command.Parameters.AddWithValue("@keywordLike", "%" + cleanKeyword + "%");

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(MapView(reader));
                    }
                }
            }

            return items;
        }

        public MedicalRecordItem GetByPatientId(int patientId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return GetByPatientId(patientId, connection, null);
            }
        }

        public MedicalRecordItem GetByPatientId(int patientId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT RecordId, PatientId, LastVisitDate, BloodType, AllergyNotes, ChronicDiseaseNotes
                    FROM medical_records
                    WHERE PatientId = @patientId
                    LIMIT 1;";

                command.Parameters.AddWithValue("@patientId", patientId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? Map(reader) : null;
                }
            }
        }

        public void CreateIfNotExists(int patientId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                CreateIfNotExists(patientId, connection, null);
            }
        }

        public void CreateIfNotExists(int patientId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO medical_records
                        (PatientId, LastVisitDate, BloodType, AllergyNotes, ChronicDiseaseNotes)
                    VALUES
                        (@patientId, NULL, NULL, '', '')
                    ON DUPLICATE KEY UPDATE PatientId = PatientId;";

                command.Parameters.AddWithValue("@patientId", patientId);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateLastVisitDate(int patientId, DateTime date)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                UpdateLastVisitDate(patientId, date, connection, null);
            }
        }

        public void UpdateLastVisitDate(int patientId, DateTime date, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE medical_records
                    SET LastVisitDate = @lastVisitDate
                    WHERE PatientId = @patientId;";

                command.Parameters.AddWithValue("@patientId", patientId);
                command.Parameters.AddWithValue("@lastVisitDate", date);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateMedicalRecord(MedicalRecordItem record)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE medical_records
                    SET BloodType = @bloodType,
                        AllergyNotes = @allergyNotes,
                        ChronicDiseaseNotes = @chronicDiseaseNotes
                    WHERE PatientId = @patientId;";

                command.Parameters.AddWithValue("@patientId", record.PatientId);
                command.Parameters.AddWithValue("@bloodType", string.IsNullOrWhiteSpace(record.BloodType) ? (object)DBNull.Value : record.BloodType.Trim());
                command.Parameters.AddWithValue("@allergyNotes", string.IsNullOrWhiteSpace(record.AllergyNotes) ? (object)DBNull.Value : record.AllergyNotes.Trim());
                command.Parameters.AddWithValue("@chronicDiseaseNotes", string.IsNullOrWhiteSpace(record.ChronicDiseaseNotes) ? (object)DBNull.Value : record.ChronicDiseaseNotes.Trim());
                command.ExecuteNonQuery();
            }
        }

        private static MedicalRecordItem Map(MySqlDataReader reader)
        {
            return new MedicalRecordItem
            {
                RecordId = GetInt(reader, "RecordId"),
                PatientId = GetInt(reader, "PatientId"),
                LastVisitDate = GetNullableDate(reader, "LastVisitDate"),
                BloodType = GetString(reader, "BloodType"),
                AllergyNotes = GetString(reader, "AllergyNotes"),
                ChronicDiseaseNotes = GetString(reader, "ChronicDiseaseNotes")
            };
        }

        private static MedicalRecordViewItem MapView(MySqlDataReader reader)
        {
            return new MedicalRecordViewItem
            {
                RecordId = GetInt(reader, "RecordId"),
                PatientId = GetInt(reader, "PatientId"),
                LastVisitDate = GetNullableDate(reader, "LastVisitDate"),
                BloodType = GetString(reader, "BloodType"),
                AllergyNotes = GetString(reader, "AllergyNotes"),
                ChronicDiseaseNotes = GetString(reader, "ChronicDiseaseNotes"),
                PatientName = GetString(reader, "PatientName"),
                PatientNumber = GetString(reader, "PatientNumber"),
                Age = GetInt(reader, "Age"),
                Gender = GetString(reader, "Gender"),
                PhoneNumber = GetString(reader, "PhoneNumber")
            };
        }

        private static string BaseMedicalRecordViewSelect()
        {
            return @"
                SELECT
                    COALESCE(mr.RecordId, 0) AS RecordId,
                    p.PatientId,
                    mr.LastVisitDate,
                    COALESCE(mr.BloodType, '') AS BloodType,
                    COALESCE(mr.AllergyNotes, '') AS AllergyNotes,
                    COALESCE(mr.ChronicDiseaseNotes, '') AS ChronicDiseaseNotes,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    TIMESTAMPDIFF(YEAR, p.BirthDate, CURDATE()) AS Age,
                    p.Gender,
                    COALESCE(p.PhoneNumber, '') AS PhoneNumber
                FROM patients p
                LEFT JOIN medical_records mr ON mr.PatientId = p.PatientId";
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
