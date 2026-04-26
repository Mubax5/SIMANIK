using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class PasienRepository
    {
        public string GenerateNextNoRekamMedis(MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT no_rekam_medis
                    FROM patients
                    WHERE no_rekam_medis LIKE 'RM%'
                    ORDER BY CAST(SUBSTRING(no_rekam_medis, 3) AS UNSIGNED) DESC
                    LIMIT 1;";

                object result = command.ExecuteScalar();
                int nextNumber = 1;

                if (result != null && result != DBNull.Value)
                {
                    string lastNoRekamMedis = Convert.ToString(result);
                    int lastNumber;

                    if (!string.IsNullOrWhiteSpace(lastNoRekamMedis)
                        && lastNoRekamMedis.Length > 2
                        && int.TryParse(lastNoRekamMedis.Substring(2), out lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }

                return "RM" + nextNumber.ToString("D6");
            }
        }

        public int Insert(Pasien pasien, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO patients
                        (user_id, no_rekam_medis, nik, nama_lengkap, jenis_kelamin, tanggal_lahir, no_telepon, email, alamat, created_at)
                    VALUES
                        (@user_id, @no_rekam_medis, @nik, @nama_lengkap, @jenis_kelamin, @tanggal_lahir, @no_telepon, @email, @alamat, @created_at);";

                command.Parameters.AddWithValue("@user_id", pasien.UserId);
                command.Parameters.AddWithValue("@no_rekam_medis", pasien.NoRekamMedis);
                command.Parameters.AddWithValue("@nik", (object)pasien.Nik ?? DBNull.Value);
                command.Parameters.AddWithValue("@nama_lengkap", pasien.NamaLengkap);
                command.Parameters.AddWithValue("@jenis_kelamin", (int)pasien.JenisKelamin);
                command.Parameters.AddWithValue("@tanggal_lahir", (object)pasien.TanggalLahir ?? DBNull.Value);
                command.Parameters.AddWithValue("@no_telepon", pasien.NoTelepon);
                command.Parameters.AddWithValue("@email", (object)pasien.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@alamat", (object)pasien.Alamat ?? DBNull.Value);
                command.Parameters.AddWithValue("@created_at", pasien.CreatedAt);
                command.ExecuteNonQuery();
            }

            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT LAST_INSERT_ID();";
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void InsertEmptyMedicalRecordIfTableExists(
            int pasienId,
            string noRekamMedis,
            MySqlConnection connection,
            MySqlTransaction transaction)
        {
            const string tableName = "medical_records";

            if (!TableExists(tableName, connection, transaction))
            {
                return;
            }

            HashSet<string> columns = GetTableColumns(tableName, connection, transaction);
            string pasienColumn = null;

            if (columns.Contains("patient_id"))
            {
                pasienColumn = "patient_id";
            }
            else if (columns.Contains("pasien_id"))
            {
                pasienColumn = "pasien_id";
            }

            if (pasienColumn == null)
            {
                return;
            }

            List<string> insertColumns = new List<string>();
            List<string> insertValues = new List<string>();

            AddColumnValue(insertColumns, insertValues, pasienColumn, "@pasien_id");

            if (columns.Contains("no_rekam_medis"))
            {
                AddColumnValue(insertColumns, insertValues, "no_rekam_medis", "@no_rekam_medis");
            }
            else if (columns.Contains("nomor_rekam_medis"))
            {
                AddColumnValue(insertColumns, insertValues, "nomor_rekam_medis", "@no_rekam_medis");
            }

            AddOptionalNowColumn(columns, insertColumns, insertValues, "tanggal_rekam_medis");
            AddOptionalNowColumn(columns, insertColumns, insertValues, "tanggal_pemeriksaan");
            AddOptionalNowColumn(columns, insertColumns, insertValues, "record_date");
            AddOptionalNowColumn(columns, insertColumns, insertValues, "created_at");
            AddOptionalNowColumn(columns, insertColumns, insertValues, "updated_at");

            AddOptionalEmptyTextColumn(columns, insertColumns, insertValues, "anamnesis");
            AddOptionalEmptyTextColumn(columns, insertColumns, insertValues, "diagnosis");
            AddOptionalEmptyTextColumn(columns, insertColumns, insertValues, "tindakan");
            AddOptionalEmptyTextColumn(columns, insertColumns, insertValues, "resep");
            AddOptionalEmptyTextColumn(columns, insertColumns, insertValues, "catatan");

            if (columns.Contains("status"))
            {
                AddColumnValue(insertColumns, insertValues, "status", "@status");
            }

            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO `" + tableName + "` (" + string.Join(", ", insertColumns) + ") VALUES (" + string.Join(", ", insertValues) + ");";
                command.Parameters.AddWithValue("@pasien_id", pasienId);
                command.Parameters.AddWithValue("@no_rekam_medis", noRekamMedis);
                command.Parameters.AddWithValue("@empty_text", string.Empty);
                command.Parameters.AddWithValue("@status", 1);
                command.ExecuteNonQuery();
            }
        }

        private static bool TableExists(string tableName, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COUNT(1)
                    FROM information_schema.tables
                    WHERE table_schema = DATABASE()
                      AND table_name = @table_name;";

                command.Parameters.AddWithValue("@table_name", tableName);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static HashSet<string> GetTableColumns(string tableName, MySqlConnection connection, MySqlTransaction transaction)
        {
            HashSet<string> columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT column_name
                    FROM information_schema.columns
                    WHERE table_schema = DATABASE()
                      AND table_name = @table_name;";

                command.Parameters.AddWithValue("@table_name", tableName);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(Convert.ToString(reader["column_name"]));
                    }
                }
            }

            return columns;
        }

        private static void AddOptionalNowColumn(HashSet<string> columns, List<string> insertColumns, List<string> insertValues, string columnName)
        {
            if (columns.Contains(columnName))
            {
                AddColumnValue(insertColumns, insertValues, columnName, "NOW()");
            }
        }

        private static void AddOptionalEmptyTextColumn(HashSet<string> columns, List<string> insertColumns, List<string> insertValues, string columnName)
        {
            if (columns.Contains(columnName))
            {
                AddColumnValue(insertColumns, insertValues, columnName, "@empty_text");
            }
        }

        private static void AddColumnValue(List<string> insertColumns, List<string> insertValues, string columnName, string valueExpression)
        {
            insertColumns.Add("`" + columnName.Replace("`", "``") + "`");
            insertValues.Add(valueExpression);
        }
    }
}
