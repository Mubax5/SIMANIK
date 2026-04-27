using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class VisitService
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly VisitRepository _visitRepository;

        public VisitService()
        {
            _reservationRepository = new ReservationRepository();
            _visitRepository = new VisitRepository();
        }

        public ServiceResult CheckInReservation(int reservationId)
        {
            if (!SessionHelper.HasRole(UserRole.Admin))
            {
                return ServiceResult.Fail("Hanya Admin yang boleh melakukan check-in pasien.");
            }

            if (reservationId <= 0)
            {
                return ServiceResult.Fail("Pilih reservasi yang ingin di-check-in.");
            }

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    ReservationListItem reservation = _reservationRepository.GetReservationById(reservationId, connection, transaction);
                    if (reservation == null)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Reservasi tidak ditemukan.");
                    }

                    if (reservation.ReservationStatus != ReservationStatusText.Confirmed)
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Reservasi hanya bisa check-in jika statusnya Dikonfirmasi.");
                    }

                    if (_visitRepository.ReservationHasVisit(reservationId, connection, transaction))
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Reservasi ini sudah pernah check-in.");
                    }

                    int queueNumber = QueueHelper.GenerateNextQueueNumber(DateTime.Today, _visitRepository, connection, transaction);
                    _visitRepository.CreateVisit(reservationId, queueNumber, connection, transaction);
                    _reservationRepository.UpdateReservationStatus(reservationId, ReservationStatusText.CheckedIn, connection, transaction);

                    transaction.Commit();
                    return ServiceResult.Ok("Check-in berhasil. Nomor antrian: " + queueNumber + ".");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ServiceResult.Fail("Check-in gagal: " + ex.Message);
                }
            }
        }

        public List<QueueListItem> GetTodayQueuesForCurrentUser(string status, string keyword)
        {
            if (SessionHelper.HasRole(UserRole.Admin))
            {
                return _visitRepository.GetTodayQueuesForAdmin(status, keyword);
            }

            if (SessionHelper.HasRole(UserRole.Dokter))
            {
                return _visitRepository.GetTodayQueuesForDoctor(SessionHelper.CurrentUser.Id, status, keyword);
            }

            return new List<QueueListItem>();
        }
    }
}
