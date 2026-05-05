using System;
using System.Collections.Generic;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class MedicalRecordService
    {
        private readonly MedicalRecordRepository _repository;

        public MedicalRecordService()
        {
            _repository = new MedicalRecordRepository();
        }

        public MedicalRecordItem GetMedicalRecordForPatient(int patientId)
        {
            if (patientId <= 0)
            {
                return null;
            }

            return _repository.GetByPatientId(patientId);
        }

        public MedicalRecordViewItem GetMedicalRecordForCurrentPatient()
        {
            if (!SessionHelper.HasRole(UserRole.Pasien) || SessionHelper.CurrentUser == null)
            {
                return null;
            }

            return _repository.GetByPatientUserId(SessionHelper.CurrentUser.Id);
        }

        public List<MedicalRecordViewItem> SearchMedicalRecordsForAdmin(string keyword)
        {
            if (!SessionHelper.HasRole(UserRole.Admin))
            {
                return new List<MedicalRecordViewItem>();
            }

            return _repository.Search(keyword);
        }

        public void UpdateLastVisitDate(int patientId)
        {
            if (patientId <= 0)
            {
                return;
            }

            _repository.CreateIfNotExists(patientId);
            _repository.UpdateLastVisitDate(patientId, DateTime.Now);
        }

        public ServiceResult UpdateMedicalRecord(MedicalRecordItem record)
        {
            if (!SessionHelper.HasRole(UserRole.Admin))
            {
                return ServiceResult.Fail("Hanya Admin yang boleh memperbarui medical record.");
            }

            if (record == null || record.PatientId <= 0)
            {
                return ServiceResult.Fail("Data medical record tidak valid.");
            }

            _repository.CreateIfNotExists(record.PatientId);
            _repository.UpdateMedicalRecord(record);
            return ServiceResult.Ok("Medical record berhasil diperbarui.");
        }
    }
}
