using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace SIMANIK.Helpers
{
    public static class DatabaseHelper
    {
        private const string ConnectionStringName = "SimanikDb";

        public static string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];

                if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    throw new InvalidOperationException("Connection string SimanikDb belum dikonfigurasi di App.config.");
                }

                return settings.ConnectionString;
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = GetConnection();
            connection.Open();
            return connection;
        }
    }
}
