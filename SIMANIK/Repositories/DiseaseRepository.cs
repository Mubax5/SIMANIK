using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class DiseaseRepository
    {
        public List<DiseaseItem> Search(string keyword, string status)
        {
            List<DiseaseItem> items = new List<DiseaseItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DiseaseId, DiseaseCode, DiseaseName, Description, IsActive
                    FROM diseases
                    WHERE (@keyword = '' OR DiseaseCode LIKE @keywordLike OR DiseaseName LIKE @keywordLike)
                      AND (@status = 'Semua'
                           OR (@status = 'Aktif' AND IsActive = 1)
                           OR (@status = 'Nonaktif' AND IsActive = 0))
                    ORDER BY DiseaseCode;";

                command.Parameters.AddWithValue("@keyword", Normalize(keyword));
                command.Parameters.AddWithValue("@keywordLike", "%" + Normalize(keyword) + "%");
                command.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(status) ? "Semua" : status.Trim());

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new DiseaseItem
                        {
                            DiseaseId = Convert.ToInt32(reader["DiseaseId"]),
                            DiseaseCode = Convert.ToString(reader["DiseaseCode"]),
                            DiseaseName = Convert.ToString(reader["DiseaseName"]),
                            Description = reader["Description"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Description"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }

            return items;
        }

        public bool IsCodeExists(string diseaseCode, int excludedDiseaseId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COUNT(1)
                    FROM diseases
                    WHERE DiseaseCode = @diseaseCode
                      AND DiseaseId <> @excludedDiseaseId;";

                command.Parameters.AddWithValue("@diseaseCode", diseaseCode);
                command.Parameters.AddWithValue("@excludedDiseaseId", excludedDiseaseId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public void Insert(DiseaseItem disease)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO diseases (DiseaseCode, DiseaseName, Description, IsActive)
                    VALUES (@code, @name, @description, @isActive);";

                AddParameters(command, disease);
                command.ExecuteNonQuery();
            }
        }

        public void Update(DiseaseItem disease)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE diseases
                    SET DiseaseCode = @code,
                        DiseaseName = @name,
                        Description = @description,
                        IsActive = @isActive
                    WHERE DiseaseId = @diseaseId;";

                AddParameters(command, disease);
                command.Parameters.AddWithValue("@diseaseId", disease.DiseaseId);
                command.ExecuteNonQuery();
            }
        }

        public void SetActive(int diseaseId, bool isActive)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE diseases SET IsActive = @isActive WHERE DiseaseId = @diseaseId;";
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@diseaseId", diseaseId);
                command.ExecuteNonQuery();
            }
        }

        private static void AddParameters(MySqlCommand command, DiseaseItem disease)
        {
            command.Parameters.AddWithValue("@code", disease.DiseaseCode);
            command.Parameters.AddWithValue("@name", disease.DiseaseName);
            command.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(disease.Description) ? (object)DBNull.Value : disease.Description.Trim());
            command.Parameters.AddWithValue("@isActive", disease.IsActive);
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
