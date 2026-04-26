using System;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class UserRepository
    {
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
