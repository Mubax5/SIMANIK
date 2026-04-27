using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class ScheduleRepository
    {
        public List<DoctorScheduleItem> Search(int doctorId, DateTime? date, string status)
        {
            List<DoctorScheduleItem> items = new List<DoctorScheduleItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT s.ScheduleId, s.DoctorId, d.FullName AS DoctorName, s.ScheduleDate, s.StartTime, s.EndTime, s.Quota, s.IsActive
                    FROM doctor_schedules s
                    INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                    WHERE (@doctorId = 0 OR s.DoctorId = @doctorId)
                      AND (@scheduleDate IS NULL OR s.ScheduleDate = @scheduleDate)
                      AND (@status = 'Semua'
                           OR (@status = 'Aktif' AND s.IsActive = 1)
                           OR (@status = 'Nonaktif' AND s.IsActive = 0))
                    ORDER BY s.ScheduleDate DESC, s.StartTime;";

                command.Parameters.AddWithValue("@doctorId", doctorId);
                command.Parameters.AddWithValue("@scheduleDate", date.HasValue ? (object)date.Value.Date : DBNull.Value);
                command.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(status) ? "Semua" : status.Trim());

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new DoctorScheduleItem
                        {
                            ScheduleId = Convert.ToInt32(reader["ScheduleId"]),
                            DoctorId = Convert.ToInt32(reader["DoctorId"]),
                            DoctorName = Convert.ToString(reader["DoctorName"]),
                            ScheduleDate = Convert.ToDateTime(reader["ScheduleDate"]),
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"],
                            Quota = Convert.ToInt32(reader["Quota"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }

            return items;
        }

        public void Insert(DoctorScheduleItem schedule)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO doctor_schedules (DoctorId, ScheduleDate, StartTime, EndTime, Quota, IsActive)
                    VALUES (@doctorId, @scheduleDate, @startTime, @endTime, @quota, @isActive);";

                AddParameters(command, schedule);
                command.ExecuteNonQuery();
            }
        }

        public void Update(DoctorScheduleItem schedule)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE doctor_schedules
                    SET DoctorId = @doctorId,
                        ScheduleDate = @scheduleDate,
                        StartTime = @startTime,
                        EndTime = @endTime,
                        Quota = @quota,
                        IsActive = @isActive
                    WHERE ScheduleId = @scheduleId;";

                AddParameters(command, schedule);
                command.Parameters.AddWithValue("@scheduleId", schedule.ScheduleId);
                command.ExecuteNonQuery();
            }
        }

        public void SetActive(int scheduleId, bool isActive)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE doctor_schedules SET IsActive = @isActive WHERE ScheduleId = @scheduleId;";
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@scheduleId", scheduleId);
                command.ExecuteNonQuery();
            }
        }

        private static void AddParameters(MySqlCommand command, DoctorScheduleItem schedule)
        {
            command.Parameters.AddWithValue("@doctorId", schedule.DoctorId);
            command.Parameters.AddWithValue("@scheduleDate", schedule.ScheduleDate.Date);
            command.Parameters.AddWithValue("@startTime", schedule.StartTime);
            command.Parameters.AddWithValue("@endTime", schedule.EndTime);
            command.Parameters.AddWithValue("@quota", schedule.Quota);
            command.Parameters.AddWithValue("@isActive", schedule.IsActive);
        }
    }
}
