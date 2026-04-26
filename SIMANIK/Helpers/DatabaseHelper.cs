using System;
using MySql.Data.MySqlClient;

namespace SIMANIK.Helpers
{
    public static class DatabaseHelper
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigHelper.BuildConnectionString();
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
