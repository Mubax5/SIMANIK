using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class MedicineRepository
    {
        public List<MedicineItem> Search(string keyword, string status)
        {
            List<MedicineItem> items = new List<MedicineItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT MedicineId, MedicineName, MedicineType, Stock, Unit, DefaultInstruction, IsActive
                    FROM medicines
                    WHERE (@keyword = '' OR MedicineName LIKE @keywordLike OR COALESCE(MedicineType, '') LIKE @keywordLike)
                      AND (@status = 'Semua'
                           OR (@status = 'Aktif' AND IsActive = 1)
                           OR (@status = 'Nonaktif' AND IsActive = 0)
                           OR (@status = 'Stok Rendah' AND Stock <= 10))
                    ORDER BY MedicineName;";

                command.Parameters.AddWithValue("@keyword", Normalize(keyword));
                command.Parameters.AddWithValue("@keywordLike", "%" + Normalize(keyword) + "%");
                command.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(status) ? "Semua" : status.Trim());

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MedicineItem
                        {
                            MedicineId = Convert.ToInt32(reader["MedicineId"]),
                            MedicineName = Convert.ToString(reader["MedicineName"]),
                            MedicineType = reader["MedicineType"] == DBNull.Value ? string.Empty : Convert.ToString(reader["MedicineType"]),
                            Stock = Convert.ToInt32(reader["Stock"]),
                            Unit = reader["Unit"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Unit"]),
                            DefaultInstruction = reader["DefaultInstruction"] == DBNull.Value ? string.Empty : Convert.ToString(reader["DefaultInstruction"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }

            return items;
        }

        public void Insert(MedicineItem medicine)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO medicines (MedicineName, MedicineType, Stock, Unit, DefaultInstruction, IsActive)
                    VALUES (@name, @type, @stock, @unit, @instruction, @isActive);";

                AddParameters(command, medicine);
                command.ExecuteNonQuery();
            }
        }

        public void Update(MedicineItem medicine)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE medicines
                    SET MedicineName = @name,
                        MedicineType = @type,
                        Stock = @stock,
                        Unit = @unit,
                        DefaultInstruction = @instruction,
                        IsActive = @isActive
                    WHERE MedicineId = @medicineId;";

                AddParameters(command, medicine);
                command.Parameters.AddWithValue("@medicineId", medicine.MedicineId);
                command.ExecuteNonQuery();
            }
        }

        public void SetActive(int medicineId, bool isActive)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE medicines SET IsActive = @isActive WHERE MedicineId = @medicineId;";
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@medicineId", medicineId);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int medicineId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM medicines WHERE MedicineId = @medicineId;";
                command.Parameters.AddWithValue("@medicineId", medicineId);
                command.ExecuteNonQuery();
            }
        }

        public void Deactivate(int medicineId)
        {
            SetActive(medicineId, false);
        }

        public bool HasRelations(int medicineId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM prescription_details WHERE MedicineId = @medicineId;";
                command.Parameters.AddWithValue("@medicineId", medicineId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static void AddParameters(MySqlCommand command, MedicineItem medicine)
        {
            command.Parameters.AddWithValue("@name", medicine.MedicineName);
            command.Parameters.AddWithValue("@type", string.IsNullOrWhiteSpace(medicine.MedicineType) ? (object)DBNull.Value : medicine.MedicineType.Trim());
            command.Parameters.AddWithValue("@stock", medicine.Stock);
            command.Parameters.AddWithValue("@unit", string.IsNullOrWhiteSpace(medicine.Unit) ? (object)DBNull.Value : medicine.Unit.Trim());
            command.Parameters.AddWithValue("@instruction", string.IsNullOrWhiteSpace(medicine.DefaultInstruction) ? (object)DBNull.Value : medicine.DefaultInstruction.Trim());
            command.Parameters.AddWithValue("@isActive", medicine.IsActive);
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
