using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class UserRepository
    {
        public List<UserListItem> Search(string keyword, string role, string status)
        {
            List<UserListItem> items = new List<UserListItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT UserId, Username, Role, IsActive, CreatedAt
                    FROM users
                    WHERE (@keyword = '' OR Username LIKE @keywordLike)
                      AND (@role = 'Semua' OR Role = @role)
                      AND (@status = 'Semua'
                           OR (@status = 'Aktif' AND IsActive = 1)
                           OR (@status = 'Nonaktif' AND IsActive = 0))
                    ORDER BY CreatedAt DESC, Username;";

                command.Parameters.AddWithValue("@keyword", Normalize(keyword));
                command.Parameters.AddWithValue("@keywordLike", "%" + Normalize(keyword) + "%");
                command.Parameters.AddWithValue("@role", string.IsNullOrWhiteSpace(role) ? "Semua" : role.Trim());
                command.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(status) ? "Semua" : status.Trim());

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new UserListItem
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = Convert.ToString(reader["Username"]),
                            Role = Convert.ToString(reader["Role"]),
                            Status = ParseBoolean(reader["IsActive"]) ? "Aktif" : "Nonaktif",
                            CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }

            return items;
        }

        public User FindByUsernameAndPassword(string username, string password)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT UserId, Username, Password, Role, IsActive, CreatedAt
                    FROM users
                    WHERE Username = @username
                      AND (
                            Password = @password
                            OR LOWER(Password) = LOWER(SHA2(@password, 256))
                          )
                    LIMIT 1;";

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapUser(reader);
                }
            }
        }

        public bool IsUsernameExists(string username)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return IsUsernameExists(username, connection, null);
            }
        }

        public bool IsUsernameExists(string username, int excludedUserId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COUNT(1)
                    FROM users
                    WHERE Username = @username
                      AND UserId <> @excludedUserId;";

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@excludedUserId", excludedUserId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public bool IsUsernameExists(string username, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COUNT(1)
                    FROM users
                    WHERE Username = @username;";

                command.Parameters.AddWithValue("@username", username);

                object result = command.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public int Insert(User user, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO users
                        (Username, Password, Role, IsActive, CreatedAt)
                    VALUES
                        (@username, @password, @role, @is_active, @created_at);";

                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@password", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role.ToString());
                command.Parameters.AddWithValue("@is_active", user.IsActive);
                command.Parameters.AddWithValue("@created_at", user.CreatedAt);
                command.ExecuteNonQuery();
            }

            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT LAST_INSERT_ID();";
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int Insert(User user)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return Insert(user, connection, null);
            }
        }

        public void Update(User user, bool updatePassword)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = updatePassword
                    ? @"
                        UPDATE users
                        SET Username = @username,
                            Password = @password,
                            Role = @role,
                            IsActive = @isActive
                        WHERE UserId = @userId;"
                    : @"
                        UPDATE users
                        SET Username = @username,
                            Role = @role,
                            IsActive = @isActive
                        WHERE UserId = @userId;";

                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@role", user.Role.ToString());
                command.Parameters.AddWithValue("@isActive", user.IsActive);
                command.Parameters.AddWithValue("@userId", user.Id);

                if (updatePassword)
                {
                    command.Parameters.AddWithValue("@password", user.PasswordHash);
                }

                command.ExecuteNonQuery();
            }
        }

        public void SetActive(int userId, bool isActive)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE users SET IsActive = @isActive WHERE UserId = @userId;";
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@userId", userId);
                command.ExecuteNonQuery();
            }
        }

        public List<LookupItem> GetDoctorUserOptions(int selectedUserId)
        {
            List<LookupItem> items = new List<LookupItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT u.UserId, u.Username
                    FROM users u
                    LEFT JOIN doctors d ON d.UserId = u.UserId
                    WHERE u.Role = 'Dokter'
                      AND u.IsActive = 1
                      AND (d.DoctorId IS NULL OR u.UserId = @selectedUserId)
                    ORDER BY u.Username;";

                command.Parameters.AddWithValue("@selectedUserId", selectedUserId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LookupItem
                        {
                            Id = Convert.ToInt32(reader["UserId"]),
                            Text = Convert.ToString(reader["Username"])
                        });
                    }
                }
            }

            return items;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static User MapUser(MySqlDataReader reader)
        {
            string username = Convert.ToString(reader["Username"]);

            return new User
            {
                Id = Convert.ToInt32(reader["UserId"]),
                Username = username,
                PasswordHash = Convert.ToString(reader["Password"]),
                NamaLengkap = username,
                Role = ParseRole(reader["Role"]),
                IsActive = ParseBoolean(reader["IsActive"]),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedAt"])
            };
        }

        private static UserRole ParseRole(object value)
        {
            string text = Convert.ToString(value);
            int roleNumber;

            if (int.TryParse(text, out roleNumber) && Enum.IsDefined(typeof(UserRole), roleNumber))
            {
                return (UserRole)roleNumber;
            }

            UserRole role;
            if (Enum.TryParse(text, true, out role))
            {
                return role;
            }

            throw new InvalidOperationException("Role user tidak valid: " + text);
        }

        private static bool ParseBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            if (value is bool)
            {
                return (bool)value;
            }

            string text = Convert.ToString(value);
            if (text == "1")
            {
                return true;
            }

            if (text == "0")
            {
                return false;
            }

            bool parsed;
            return bool.TryParse(text, out parsed) && parsed;
        }
    }
}
