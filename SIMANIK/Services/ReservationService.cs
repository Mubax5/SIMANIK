using System.Collections.Generic;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class ReservationService
    {
        private readonly ReservationRepository _repository;

        public ReservationService()
        {
            _repository = new ReservationRepository();
        }

        public List<LookupItem> GetActiveDoctors()
        {
            return _repository.GetDoctorsActive();
        }

        public List<ReservationScheduleItem> GetSchedulesByDoctor(int doctorId)
        {
            if (doctorId <= 0)
            {
                return new List<ReservationScheduleItem>();
            }

            return _repository.GetSchedulesByDoctor(doctorId);
        }

        public ServiceResult CreateReservation(int userId, int scheduleId, string complaint)
        {
            if (!SessionHelper.HasRole(UserRole.Pasien))
            {
                return ServiceResult.Fail("Hanya Pasien yang boleh membuat reservasi.");
            }

            PatientLookupItem patient = _repository.GetPatientByUserId(userId);
            if (patient == null)
            {
                return ServiceResult.Fail("Data pasien untuk akun ini tidak ditemukan.");
            }

            if (scheduleId <= 0)
            {
                return ServiceResult.Fail("Jadwal dokter wajib dipilih.");
            }

            if (!ValidationHelper.IsRequired(complaint))
            {
                return ServiceResult.Fail("Keluhan awal wajib diisi.");
            }

            ServiceResult quotaResult = ValidateScheduleQuota(scheduleId);
            if (!quotaResult.Success)
            {
                return quotaResult;
            }

            _repository.CreateReservation(patient.PatientId, scheduleId, complaint.Trim());
            return ServiceResult.Ok("Reservasi berhasil dibuat dan menunggu verifikasi admin.");
        }

        public ServiceResult CancelReservation(int userId, int reservationId)
        {
            if (!SessionHelper.HasRole(UserRole.Pasien))
            {
                return ServiceResult.Fail("Hanya Pasien yang boleh membatalkan reservasi sendiri.");
            }

            PatientLookupItem patient = _repository.GetPatientByUserId(userId);
            if (patient == null)
            {
                return ServiceResult.Fail("Data pasien untuk akun ini tidak ditemukan.");
            }

            if (reservationId <= 0)
            {
                return ServiceResult.Fail("Pilih reservasi yang ingin dibatalkan.");
            }

            int affectedRows = _repository.CancelReservationByPatient(reservationId, patient.PatientId);
            if (affectedRows <= 0)
            {
                return ServiceResult.Fail("Reservasi tidak bisa dibatalkan. Reservasi mungkin sudah check-in atau selesai.");
            }

            return ServiceResult.Ok("Reservasi berhasil dibatalkan.");
        }

        public ServiceResult ConfirmReservation(int reservationId)
        {
            if (!SessionHelper.HasRole(UserRole.Admin))
            {
                return ServiceResult.Fail("Hanya Admin yang boleh mengonfirmasi reservasi.");
            }

            if (reservationId <= 0)
            {
                return ServiceResult.Fail("Pilih reservasi terlebih dahulu.");
            }

            int affectedRows = _repository.ConfirmReservation(reservationId);
            if (affectedRows <= 0)
            {
                return ServiceResult.Fail("Reservasi hanya bisa dikonfirmasi jika statusnya Menunggu Verifikasi.");
            }

            return ServiceResult.Ok("Reservasi berhasil dikonfirmasi.");
        }

        public ServiceResult RejectReservation(int reservationId, string reason)
        {
            if (!SessionHelper.HasRole(UserRole.Admin))
            {
                return ServiceResult.Fail("Hanya Admin yang boleh menolak reservasi.");
            }

            if (reservationId <= 0)
            {
                return ServiceResult.Fail("Pilih reservasi terlebih dahulu.");
            }

            if (!ValidationHelper.IsRequired(reason))
            {
                return ServiceResult.Fail("Alasan penolakan wajib diisi.");
            }

            int affectedRows = _repository.RejectReservation(reservationId, reason.Trim());
            if (affectedRows <= 0)
            {
                return ServiceResult.Fail("Reservasi hanya bisa ditolak jika statusnya Menunggu Verifikasi.");
            }

            return ServiceResult.Ok("Reservasi berhasil ditolak.");
        }

        public List<ReservationListItem> GetPatientReservations(int userId)
        {
            return _repository.GetReservationsByPatientUserId(userId);
        }

        public List<ReservationListItem> GetPendingReservations()
        {
            return _repository.GetPendingReservations();
        }

        public List<ReservationListItem> GetConfirmedReservations()
        {
            return _repository.GetConfirmedReservations();
        }

        public ServiceResult ValidateScheduleQuota(int scheduleId)
        {
            int activeCount = _repository.CountActiveReservationsBySchedule(scheduleId);
            ReservationScheduleItem selectedSchedule = _repository.GetScheduleById(scheduleId);

            if (selectedSchedule == null)
            {
                return ServiceResult.Fail("Jadwal tidak ditemukan atau tidak aktif.");
            }

            if (activeCount >= selectedSchedule.Quota)
            {
                return ServiceResult.Fail("Jadwal dokter sudah penuh. Silakan pilih jadwal lain.");
            }

            return ServiceResult.Ok("Kuota tersedia.");
        }
    }
}
