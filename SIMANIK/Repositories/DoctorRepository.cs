using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class DoctorRepository
    {
        public List<DoctorItem> Search(string keyword, string status)
        {
            List<DoctorItem> items = new List<DoctorItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT d.DoctorId, d.UserId, u.Username, d.FullName, d.Specialization, d.PhoneNumber, d.IsActive
                    FROM doctors d
                    INNER JOIN users u ON u.UserId = d.UserId
                    WHERE (@keyword = '' OR d.FullName LIKE @keywordLike OR d.Specialization LIKE @keywordLike)
                      AND (@status = 'Semua'
                           OR (@status = 'Aktif' AND d.IsActive = 1)
                           OR (@status = 'Nonaktif' AND d.IsActive = 0))
                    ORDER BY d.FullName;";

                command.Parameters.AddWithValue("@keyword", Normalize(keyword));
                command.Parameters.AddWithValue("@keywordLike", "%" + Normalize(keyword) + "%");
                command.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(status) ? "Semua" : status.Trim());

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(MapDoctor(reader));
                    }
                }
            }

            return items;
        }

        public List<LookupItem> GetActiveDoctorOptions()
        {
            List<LookupItem> items = new List<LookupItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DoctorId, FullName
                    FROM doctors
                    WHERE IsActive = 1
                    ORDER BY FullName;";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LookupItem
                        {
                            Id = Convert.ToInt32(reader["DoctorId"]),
                            Text = Convert.ToString(reader["FullName"])
                        });
                    }
                }
            }

            return items;
        }

        public bool IsUserLinkedToDoctor(int userId, int excludedDoctorId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COUNT(1)
                    FROM doctors
                    WHERE UserId = @userId
                      AND DoctorId <> @excludedDoctorId;";

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@excludedDoctorId", excludedDoctorId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public bool IsDoctorUser(int userId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM users WHERE UserId = @userId AND Role = 'Dokter' AND IsActive = 1;";
                command.Parameters.AddWithValue("@userId", userId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public void Insert(DoctorItem doctor)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO doctors (UserId, FullName, Specialization, PhoneNumber, IsActive)
                    VALUES (@userId, @fullName, @specialization, @phoneNumber, @isActive);";

                AddParameters(command, doctor);
                command.ExecuteNonQuery();
            }
        }

        public void Update(DoctorItem doctor)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE doctors
                    SET UserId = @userId,
                        FullName = @fullName,
                        Specialization = @specialization,
                        PhoneNumber = @phoneNumber,
                        IsActive = @isActive
                    WHERE DoctorId = @doctorId;";

                AddParameters(command, doctor);
                command.Parameters.AddWithValue("@doctorId", doctor.DoctorId);
                command.ExecuteNonQuery();
            }
        }

        public void SetActive(int doctorId, bool isActive)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE doctors SET IsActive = @isActive WHERE DoctorId = @doctorId;";
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@doctorId", doctorId);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int doctorId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM doctors WHERE DoctorId = @doctorId;";
                command.Parameters.AddWithValue("@doctorId", doctorId);
                command.ExecuteNonQuery();
            }
        }

        public void Deactivate(int doctorId)
        {
            SetActive(doctorId, false);
        }

        public bool HasRelations(int doctorId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        (SELECT COUNT(1) FROM doctor_schedules WHERE DoctorId = @doctorId) +
                        (SELECT COUNT(1) FROM examinations WHERE DoctorId = @doctorId);";

                command.Parameters.AddWithValue("@doctorId", doctorId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static void AddParameters(MySqlCommand command, DoctorItem doctor)
        {
            command.Parameters.AddWithValue("@userId", doctor.UserId);
            command.Parameters.AddWithValue("@fullName", doctor.FullName);
            command.Parameters.AddWithValue("@specialization", doctor.Specialization);
            command.Parameters.AddWithValue("@phoneNumber", string.IsNullOrWhiteSpace(doctor.PhoneNumber) ? (object)DBNull.Value : doctor.PhoneNumber.Trim());
            command.Parameters.AddWithValue("@isActive", doctor.IsActive);
        }

        private static DoctorItem MapDoctor(MySqlDataReader reader)
        {
            return new DoctorItem
            {
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                Username = Convert.ToString(reader["Username"]),
                FullName = Convert.ToString(reader["FullName"]),
                Specialization = Convert.ToString(reader["Specialization"]),
                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? string.Empty : Convert.ToString(reader["PhoneNumber"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
