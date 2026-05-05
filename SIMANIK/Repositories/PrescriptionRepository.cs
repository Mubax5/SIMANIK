using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class PrescriptionRepository
    {
        public void CreatePrescriptionDetail(
            int examinationId,
            int medicineId,
            int quantity,
            string instructionNote,
            MySqlConnection connection,
            MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO prescription_details
                        (ExaminationId, MedicineId, Quantity, InstructionNote)
                    VALUES
                        (@examinationId, @medicineId, @quantity, @instructionNote);";

                command.Parameters.AddWithValue("@examinationId", examinationId);
                command.Parameters.AddWithValue("@medicineId", medicineId);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@instructionNote", string.IsNullOrWhiteSpace(instructionNote) ? (object)DBNull.Value : instructionNote.Trim());
                command.ExecuteNonQuery();
            }
        }

        public List<PrescriptionDetailItem> GetPrescriptionDetailsByExaminationId(int examinationId)
        {
            List<PrescriptionDetailItem> items = new List<PrescriptionDetailItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        pd.PrescriptionDetailId,
                        pd.ExaminationId,
                        pd.MedicineId,
                        m.MedicineName,
                        COALESCE(m.MedicineType, '') AS MedicineType,
                        pd.Quantity,
                        COALESCE(m.Unit, '') AS Unit,
                        COALESCE(pd.InstructionNote, '') AS InstructionNote
                    FROM prescription_details pd
                    INNER JOIN medicines m ON m.MedicineId = pd.MedicineId
                    WHERE pd.ExaminationId = @examinationId
                    ORDER BY m.MedicineName;";

                command.Parameters.AddWithValue("@examinationId", examinationId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new PrescriptionDetailItem
                        {
                            PrescriptionDetailId = GetInt(reader, "PrescriptionDetailId"),
                            ExaminationId = GetInt(reader, "ExaminationId"),
                            MedicineId = GetInt(reader, "MedicineId"),
                            MedicineName = GetString(reader, "MedicineName"),
                            MedicineType = GetString(reader, "MedicineType"),
                            Quantity = GetInt(reader, "Quantity"),
                            Unit = GetString(reader, "Unit"),
                            InstructionNote = GetString(reader, "InstructionNote")
                        });
                    }
                }
            }

            return items;
        }

        public int ReduceMedicineStock(int medicineId, int quantity, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE medicines
                    SET Stock = Stock - @quantity
                    WHERE MedicineId = @medicineId
                      AND IsActive = 1
                      AND Stock >= @quantity;";

                command.Parameters.AddWithValue("@medicineId", medicineId);
                command.Parameters.AddWithValue("@quantity", quantity);
                return command.ExecuteNonQuery();
            }
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
    }
}
