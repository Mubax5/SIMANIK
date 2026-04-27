using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;

namespace SIMANIK.Repositories
{
    public class ReservationRepository
    {
        public List<LookupItem> GetDoctorsActive()
        {
            List<LookupItem> items = new List<LookupItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DoctorId, FullName, Specialization
                    FROM doctors
                    WHERE IsActive = 1
                    ORDER BY FullName;";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LookupItem
                        {
                            Id = Convert.ToInt32(reader["DoctorId"]),
                            Text = Convert.ToString(reader["FullName"]) + " - " + Convert.ToString(reader["Specialization"])
                        });
                    }
                }
            }

            return items;
        }

        public List<ReservationScheduleItem> GetSchedulesByDoctor(int doctorId)
        {
            List<ReservationScheduleItem> items = new List<ReservationScheduleItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        s.ScheduleId,
                        s.DoctorId,
                        d.FullName AS DoctorName,
                        s.ScheduleDate,
                        s.StartTime,
                        s.EndTime,
                        s.Quota,
                        (
                            SELECT COUNT(1)
                            FROM reservations r
                            WHERE r.ScheduleId = s.ScheduleId
                              AND r.ReservationStatus IN ('Menunggu Verifikasi', 'Dikonfirmasi', 'Check-in')
                        ) AS UsedQuota
                    FROM doctor_schedules s
                    INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                    WHERE s.DoctorId = @doctorId
                      AND s.IsActive = 1
                      AND d.IsActive = 1
                      AND s.ScheduleDate >= CURDATE()
                    ORDER BY s.ScheduleDate, s.StartTime;";

                command.Parameters.AddWithValue("@doctorId", doctorId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(MapSchedule(reader));
                    }
                }
            }

            return items;
        }

        public ReservationScheduleItem GetScheduleById(int scheduleId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        s.ScheduleId,
                        s.DoctorId,
                        d.FullName AS DoctorName,
                        s.ScheduleDate,
                        s.StartTime,
                        s.EndTime,
                        s.Quota,
                        (
                            SELECT COUNT(1)
                            FROM reservations r
                            WHERE r.ScheduleId = s.ScheduleId
                              AND r.ReservationStatus IN ('Menunggu Verifikasi', 'Dikonfirmasi', 'Check-in')
                        ) AS UsedQuota
                    FROM doctor_schedules s
                    INNER JOIN doctors d ON d.DoctorId = s.DoctorId
                    WHERE s.ScheduleId = @scheduleId
                      AND s.IsActive = 1
                      AND d.IsActive = 1
                      AND s.ScheduleDate >= CURDATE()
                    LIMIT 1;";

                command.Parameters.AddWithValue("@scheduleId", scheduleId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapSchedule(reader);
                }
            }
        }

        public PatientLookupItem GetPatientByUserId(int userId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT PatientId, UserId, PatientNumber, FullName
                    FROM patients
                    WHERE UserId = @userId
                    LIMIT 1;";

                command.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapPatient(reader);
                }
            }
        }

        public List<ReservationListItem> GetReservationsByPatientUserId(int userId)
        {
            List<ReservationListItem> items = new List<ReservationListItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = BaseReservationSelect() + @"
                    WHERE p.UserId = @userId
                    ORDER BY r.CreatedAt DESC, s.ScheduleDate DESC, s.StartTime DESC;";

                command.Parameters.AddWithValue("@userId", userId);
                FillReservations(command, items);
            }

            return items;
        }

        public int CountActiveReservationsBySchedule(int scheduleId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COUNT(1)
                    FROM reservations
                    WHERE ScheduleId = @scheduleId
                      AND ReservationStatus IN ('Menunggu Verifikasi', 'Dikonfirmasi', 'Check-in');";

                command.Parameters.AddWithValue("@scheduleId", scheduleId);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public ReservationListItem GetReservationById(int reservationId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return GetReservationById(reservationId, connection, null);
            }
        }

        public ReservationListItem GetReservationById(int reservationId, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = BaseReservationSelect() + @"
                    WHERE r.ReservationId = @reservationId
                    LIMIT 1;";

                command.Parameters.AddWithValue("@reservationId", reservationId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapReservation(reader);
                }
            }
        }

        public void CreateReservation(int patientId, int scheduleId, string complaint)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO reservations
                        (PatientId, ScheduleId, Complaint, ReservationStatus, RejectionReason, CreatedAt)
                    VALUES
                        (@patientId, @scheduleId, @complaint, @status, NULL, NOW());";

                command.Parameters.AddWithValue("@patientId", patientId);
                command.Parameters.AddWithValue("@scheduleId", scheduleId);
                command.Parameters.AddWithValue("@complaint", complaint);
                command.Parameters.AddWithValue("@status", ReservationStatusText.Pending);
                command.ExecuteNonQuery();
            }
        }

        public int CancelReservationByPatient(int reservationId, int patientId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE reservations
                    SET ReservationStatus = @cancelled,
                        RejectionReason = NULL
                    WHERE ReservationId = @reservationId
                      AND PatientId = @patientId
                      AND ReservationStatus NOT IN ('Check-in', 'Selesai');";

                command.Parameters.AddWithValue("@cancelled", ReservationStatusText.CancelledByPatient);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                command.Parameters.AddWithValue("@patientId", patientId);
                return command.ExecuteNonQuery();
            }
        }

        public List<ReservationListItem> GetPendingReservations()
        {
            return GetReservationsByStatus(ReservationStatusText.Pending);
        }

        public int ConfirmReservation(int reservationId)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE reservations
                    SET ReservationStatus = @confirmed,
                        RejectionReason = NULL
                    WHERE ReservationId = @reservationId
                      AND ReservationStatus = @pending;";

                command.Parameters.AddWithValue("@confirmed", ReservationStatusText.Confirmed);
                command.Parameters.AddWithValue("@pending", ReservationStatusText.Pending);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                return command.ExecuteNonQuery();
            }
        }

        public int RejectReservation(int reservationId, string reason)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE reservations
                    SET ReservationStatus = @rejected,
                        RejectionReason = @reason
                    WHERE ReservationId = @reservationId
                      AND ReservationStatus = @pending;";

                command.Parameters.AddWithValue("@rejected", ReservationStatusText.Rejected);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@pending", ReservationStatusText.Pending);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                return command.ExecuteNonQuery();
            }
        }

        public List<ReservationListItem> GetConfirmedReservations()
        {
            return GetReservationsByStatus(ReservationStatusText.Confirmed);
        }

        public int UpdateReservationStatus(int reservationId, string status)
        {
            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            {
                return UpdateReservationStatus(reservationId, status, connection, null);
            }
        }

        public int UpdateReservationStatus(int reservationId, string status, MySqlConnection connection, MySqlTransaction transaction)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE reservations
                    SET ReservationStatus = @status
                    WHERE ReservationId = @reservationId;";

                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                return command.ExecuteNonQuery();
            }
        }

        private List<ReservationListItem> GetReservationsByStatus(string status)
        {
            List<ReservationListItem> items = new List<ReservationListItem>();

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = BaseReservationSelect() + @"
                    WHERE r.ReservationStatus = @status
                    ORDER BY s.ScheduleDate, s.StartTime, r.CreatedAt;";

                command.Parameters.AddWithValue("@status", status);
                FillReservations(command, items);
            }

            return items;
        }

        private static void FillReservations(MySqlCommand command, List<ReservationListItem> items)
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(MapReservation(reader));
                }
            }
        }

        private static string BaseReservationSelect()
        {
            return @"
                SELECT
                    r.ReservationId,
                    r.PatientId,
                    r.ScheduleId,
                    p.FullName AS PatientName,
                    p.PatientNumber,
                    d.FullName AS DoctorName,
                    d.Specialization,
                    s.ScheduleDate,
                    s.StartTime,
                    s.EndTime,
                    r.Complaint,
                    r.ReservationStatus,
                    r.RejectionReason,
                    r.CreatedAt
                FROM reservations r
                INNER JOIN patients p ON p.PatientId = r.PatientId
                INNER JOIN doctor_schedules s ON s.ScheduleId = r.ScheduleId
                INNER JOIN doctors d ON d.DoctorId = s.DoctorId";
        }

        private static ReservationScheduleItem MapSchedule(MySqlDataReader reader)
        {
            return new ReservationScheduleItem
            {
                ScheduleId = Convert.ToInt32(reader["ScheduleId"]),
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                DoctorName = Convert.ToString(reader["DoctorName"]),
                ScheduleDate = Convert.ToDateTime(reader["ScheduleDate"]),
                StartTime = (TimeSpan)reader["StartTime"],
                EndTime = (TimeSpan)reader["EndTime"],
                Quota = Convert.ToInt32(reader["Quota"]),
                UsedQuota = Convert.ToInt32(reader["UsedQuota"])
            };
        }

        private static PatientLookupItem MapPatient(MySqlDataReader reader)
        {
            return new PatientLookupItem
            {
                PatientId = Convert.ToInt32(reader["PatientId"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                PatientNumber = Convert.ToString(reader["PatientNumber"]),
                FullName = Convert.ToString(reader["FullName"])
            };
        }

        private static ReservationListItem MapReservation(MySqlDataReader reader)
        {
            return new ReservationListItem
            {
                ReservationId = Convert.ToInt32(reader["ReservationId"]),
                PatientId = Convert.ToInt32(reader["PatientId"]),
                ScheduleId = Convert.ToInt32(reader["ScheduleId"]),
                PatientName = Convert.ToString(reader["PatientName"]),
                PatientNumber = Convert.ToString(reader["PatientNumber"]),
                DoctorName = Convert.ToString(reader["DoctorName"]),
                Specialization = Convert.ToString(reader["Specialization"]),
                ScheduleDate = Convert.ToDateTime(reader["ScheduleDate"]),
                StartTime = (TimeSpan)reader["StartTime"],
                EndTime = (TimeSpan)reader["EndTime"],
                Complaint = Convert.ToString(reader["Complaint"]),
                ReservationStatus = Convert.ToString(reader["ReservationStatus"]),
                RejectionReason = reader["RejectionReason"] == DBNull.Value ? string.Empty : Convert.ToString(reader["RejectionReason"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }
    }
}
