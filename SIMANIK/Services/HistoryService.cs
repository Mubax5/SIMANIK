using System;
using System.Collections.Generic;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class HistoryService
    {
        private readonly HistoryRepository _repository;

        public HistoryService()
        {
            _repository = new HistoryRepository();
        }

        public List<HistoryItem> GetHistoryForCurrentUser(string historyType, string keyword, DateTime? startDate, DateTime? endDate)
        {
            if (SessionHelper.CurrentUser == null)
            {
                return new List<HistoryItem>();
            }

            NormalizeDates(ref startDate, ref endDate);

            string type = string.IsNullOrWhiteSpace(historyType) ? "Reservasi" : historyType.Trim();
            int userId = SessionHelper.CurrentUser.Id;

            if (SessionHelper.HasRole(UserRole.Admin))
            {
                return _repository.GetAdminHistory(type, keyword, startDate, endDate);
            }

            if (SessionHelper.HasRole(UserRole.Dokter))
            {
                return _repository.GetDoctorPatientHistory(userId, type, keyword, startDate, endDate);
            }

            if (SessionHelper.HasRole(UserRole.Pasien))
            {
                if (IsType(type, "Pemeriksaan"))
                {
                    return _repository.GetPatientExaminationHistory(userId, keyword, startDate, endDate);
                }

                if (IsType(type, "Obat"))
                {
                    return _repository.GetPatientMedicineHistory(userId, keyword, startDate, endDate);
                }

                if (IsType(type, "Diagnosa"))
                {
                    return _repository.GetPatientDiagnosisHistory(userId, keyword, startDate, endDate);
                }

                return _repository.GetPatientReservationHistory(userId, keyword, startDate, endDate);
            }

            return new List<HistoryItem>();
        }

        private static void NormalizeDates(ref DateTime? startDate, ref DateTime? endDate)
        {
            if (startDate.HasValue)
            {
                startDate = startDate.Value.Date;
            }

            if (endDate.HasValue)
            {
                endDate = endDate.Value.Date;
            }

            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                DateTime start = startDate.Value;
                startDate = endDate.Value;
                endDate = start;
            }
        }

        private static bool IsType(string historyType, string value)
        {
            return string.Equals(historyType, value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
