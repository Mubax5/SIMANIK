using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class VisitRepository
    {
        public int GetNextQueueNumber(DateTime date)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return GetNextQueueNumber(date, connection, null);
            }
        }

        public int GetNextQueueNumber(DateTime date, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(MAX(QueueNumber), 0) + 1
                    FROM visits
                    WHERE DATE(CheckInTime) = @date;";

                command.Parameters.AddWithValue("@date", date.Date);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void CreateVisit(int reservationId, int queueNumber)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                CreateVisit(reservationId, queueNumber, connection, null);
            }
        }

        public void CreateVisit(int reservationId, int queueNumber, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO visits
                        (ReservationId, QueueNumber, CheckInTime, VisitStatus)
                    VALUES
                        (@reservationId, @queueNumber, NOW(), @visitStatus);";

                command.Parameters.AddWithValue("@reservationId", reservationId);
                command.Parameters.AddWithValue("@queueNumber", queueNumber);
                command.Parameters.AddWithValue("@visitStatus", VisitStatusText.Waiting);
                command.ExecuteNonQuery();
            }
        }

        public bool ReservationHasVisit(int reservationId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return ReservationHasVisit(reservationId, connection, null);
            }
        }

        public bool ReservationHasVisit(int reservationId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(1) FROM visits WHERE ReservationId = @reservationId;";
                command.Parameters.AddWithValue("@reservationId", reservationId);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public List<QueueListItem> GetTodayQueuesForAdmin(string status, string keyword)
        {
            List<QueueListItem> items = new List<QueueListItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = BaseQueueSelect() + @"
                    WHERE DATE(v.CheckInTime) = CURDATE()
                      AND (@status = 'Semua' OR v.VisitStatus = @status)
                      AND (@keyword = ''
                           OR p.FullName LIKE @keywordLike
                           OR p.PatientNumber LIKE @keywordLike
                           OR CAST(v.QueueNumber AS CHAR) LIKE @keywordLike)
                    ORDER BY v.QueueNumber;";

                AddQueueFilterParameters(command, status, keyword);
                FillQueues(command, items);
            }

            return items;
        }

        public List<QueueListItem> GetTodayQueuesForDoctor(int userId, string status, string keyword)
        {
            List<QueueListItem> items = new List<QueueListItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = BaseQueueSelect() + @"
                    WHERE DATE(v.CheckInTime) = CURDATE()
                      AND d.UserId = @userId
                      AND (@status = 'Semua' OR v.VisitStatus = @status)
                      AND (@keyword = ''
                           OR p.FullName LIKE @keywordLike
                           OR p.PatientNumber LIKE @keywordLike
                           OR CAST(v.QueueNumber AS CHAR) LIKE @keywordLike)
                    ORDER BY v.QueueNumber;";

                command.Parameters.AddWithValue("@userId", userId);
                AddQueueFilterParameters(command, status, keyword);
                FillQueues(command, items);
            }

            return items;
        }

        private static string BaseQueueSelect()
        {
            return @"
                SELECT
                    v.VisitId,
                    v.ReservationId,
                    v.QueueNumber,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    d.FullName AS DoctorName,
                    s.ScheduleDate,
                    s.StartTime,
                    s.EndTime,
                    r.Complaint,
                    v.VisitStatus,
                    v.CheckInTime
                FROM visits v
                INNER JOIN reservations r ON r.ReservationId = v.ReservationId
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId";
        }

        private static void AddQueueFilterParameters(MySqlCommand command, string status, string keyword)
        {
            string cleanKeyword = string.IsNullOrWhiteSpace(keyword) ? string.Empty : keyword.Trim();
            command.Parameters.AddWithValue("@status", string.IsNullOrWhiteSpace(status) ? "Semua" : status.Trim());
            command.Parameters.AddWithValue("@keyword", cleanKeyword);
            command.Parameters.AddWithValue("@keywordLike", "%" + cleanKeyword + "%");
        }

        private static void FillQueues(MySqlCommand command, List<QueueListItem> items)
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new QueueListItem
                    {
                        VisitId = Convert.ToInt32(reader["VisitId"]),
                        ReservationId = Convert.ToInt32(reader["ReservationId"]),
                        QueueNumber = Convert.ToInt32(reader["QueueNumber"]),
                        PatientName = Convert.ToString(reader["PatientName"]),
                        PatientNumber = Convert.ToString(reader["PatientNumber"]),
                        DoctorName = Convert.ToString(reader["DoctorName"]),
                        ScheduleDate = Convert.ToDateTime(reader["ScheduleDate"]),
                        StartTime = (TimeSpan)reader["StartTime"],
                        EndTime = (TimeSpan)reader["EndTime"],
                        Complaint = Convert.ToString(reader["Complaint"]),
                        VisitStatus = Convert.ToString(reader["VisitStatus"]),
                        CheckInTime = Convert.ToDateTime(reader["CheckInTime"])
                    });
                }
            }
        }
    }
}
