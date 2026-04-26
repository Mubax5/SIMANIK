using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;

namespace SIMANIK.Helpers
{
    public class DatabaseConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public static class ConfigHelper
    {
        private const string ConfigFileName = "konfig.txt";
        private const string MissingConfigMessage =
            "File konfig.txt tidak ditemukan. Salin konfig.example.txt menjadi konfig.txt lalu sesuaikan koneksi database.";

        public static DatabaseConfig LoadDatabaseConfig()
        {
            string configPath = FindConfigFilePath();

            if (configPath == null)
            {
                throw new FileNotFoundException(MissingConfigMessage, ConfigFileName);
            }

            Dictionary<string, string> values = ReadConfigValues(configPath);

            return new DatabaseConfig
            {
                Host = GetRequiredValue(values, "DB_HOST"),
                Port = ParsePort(GetRequiredValue(values, "DB_PORT")),
                DatabaseName = GetRequiredValue(values, "DB_NAME"),
                Username = GetRequiredValue(values, "DB_USER"),
                Password = GetOptionalValue(values, "DB_PASSWORD")
            };
        }

        public static string BuildConnectionString()
        {
            DatabaseConfig config = LoadDatabaseConfig();

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = config.Host,
                Port = (uint)config.Port,
                Database = config.DatabaseName,
                UserID = config.Username,
                Password = config.Password ?? string.Empty,
                SslMode = MySqlSslMode.Disabled
            };

            return builder.ConnectionString;
        }

        private static Dictionary<string, string> ReadConfigValues(string configPath)
        {
            Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] lines = File.ReadAllLines(configPath);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                int separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                string key = line.Substring(0, separatorIndex).Trim();
                string value = line.Substring(separatorIndex + 1).Trim();
                values[key] = value;
            }

            return values;
        }

        private static string FindConfigFilePath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string directPath = Path.Combine(baseDirectory, ConfigFileName);

            if (File.Exists(directPath))
            {
                return directPath;
            }

            DirectoryInfo directory = new DirectoryInfo(baseDirectory);

            while (directory != null)
            {
                string candidatePath = Path.Combine(directory.FullName, ConfigFileName);

                if (File.Exists(candidatePath))
                {
                    return candidatePath;
                }

                directory = directory.Parent;
            }

            return null;
        }

        private static string GetRequiredValue(Dictionary<string, string> values, string key)
        {
            string value;

            if (!values.TryGetValue(key, out value) || string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Konfigurasi " + key + " wajib diisi di konfig.txt.");
            }

            return value;
        }

        private static string GetOptionalValue(Dictionary<string, string> values, string key)
        {
            string value;
            return values.TryGetValue(key, out value) ? value : string.Empty;
        }

        private static int ParsePort(string value)
        {
            int port;

            if (!int.TryParse(value, out port) || port <= 0 || port > 65535)
            {
                throw new InvalidOperationException("Konfigurasi DB_PORT tidak valid di konfig.txt.");
            }

            return port;
        }
    }
}
