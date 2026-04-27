using System.Collections.Generic;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class DiseaseService
    {
        private readonly DiseaseRepository _repository;

        public DiseaseService()
        {
            _repository = new DiseaseRepository();
        }

        public List<DiseaseItem> Search(string keyword, string status)
        {
            return _repository.Search(keyword, status);
        }

        public ServiceResult Save(DiseaseItem disease)
        {
            if (disease == null)
            {
                return ServiceResult.Fail("Data penyakit tidak valid.");
            }

            if (!ValidationHelper.IsRequired(disease.DiseaseCode))
            {
                return ServiceResult.Fail("Kode penyakit wajib diisi.");
            }

            if (!ValidationHelper.IsRequired(disease.DiseaseName))
            {
                return ServiceResult.Fail("Nama penyakit wajib diisi.");
            }

            disease.DiseaseCode = disease.DiseaseCode.Trim();
            disease.DiseaseName = disease.DiseaseName.Trim();
            disease.Description = string.IsNullOrWhiteSpace(disease.Description) ? null : disease.Description.Trim();

            if (_repository.IsCodeExists(disease.DiseaseCode, disease.DiseaseId))
            {
                return ServiceResult.Fail("Kode penyakit sudah digunakan.");
            }

            if (disease.DiseaseId <= 0)
            {
                _repository.Insert(disease);
                return ServiceResult.Ok("Penyakit berhasil ditambahkan.");
            }

            _repository.Update(disease);
            return ServiceResult.Ok("Penyakit berhasil diperbarui.");
        }

        public ServiceResult SetActive(int diseaseId, bool isActive)
        {
            if (diseaseId <= 0)
            {
                return ServiceResult.Fail("Pilih penyakit terlebih dahulu.");
            }

            _repository.SetActive(diseaseId, isActive);
            return ServiceResult.Ok(isActive ? "Penyakit berhasil diaktifkan." : "Penyakit berhasil dinonaktifkan.");
        }
    }
}
