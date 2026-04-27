using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class MedicineService
    {
        private readonly MedicineRepository _repository;

        public MedicineService()
        {
            _repository = new MedicineRepository();
        }

        public List<MedicineItem> Search(string keyword, string status)
        {
            return _repository.Search(keyword, status);
        }

        public ServiceResult Save(MedicineItem medicine)
        {
            if (medicine == null)
            {
                return ServiceResult.Fail("Data obat tidak valid.");
            }

            if (!ValidationHelper.IsRequired(medicine.MedicineName))
            {
                return ServiceResult.Fail("Nama obat wajib diisi.");
            }

            if (medicine.Stock < 0)
            {
                return ServiceResult.Fail("Stok tidak boleh negatif.");
            }

            medicine.MedicineName = medicine.MedicineName.Trim();
            medicine.MedicineType = string.IsNullOrWhiteSpace(medicine.MedicineType) ? null : medicine.MedicineType.Trim();
            medicine.Unit = string.IsNullOrWhiteSpace(medicine.Unit) ? null : medicine.Unit.Trim();
            medicine.DefaultInstruction = string.IsNullOrWhiteSpace(medicine.DefaultInstruction) ? null : medicine.DefaultInstruction.Trim();

            if (medicine.MedicineId <= 0)
            {
                _repository.Insert(medicine);
                return ServiceResult.Ok("Obat berhasil ditambahkan.");
            }

            _repository.Update(medicine);
            return ServiceResult.Ok("Obat berhasil diperbarui.");
        }

        public ServiceResult SetActive(int medicineId, bool isActive)
        {
            if (medicineId <= 0)
            {
                return ServiceResult.Fail("Pilih obat terlebih dahulu.");
            }

            _repository.SetActive(medicineId, isActive);
            return ServiceResult.Ok(isActive ? "Obat berhasil diaktifkan." : "Obat berhasil dinonaktifkan.");
        }

        public ServiceResult DeleteMedicine(int medicineId)
        {
            if (medicineId <= 0)
            {
                return ServiceResult.Fail("Pilih obat yang ingin dihapus.");
            }

            if (_repository.HasRelations(medicineId))
            {
                _repository.Deactivate(medicineId);
                return ServiceResult.Ok("Data tidak bisa dihapus karena sudah dipakai. Data akan dinonaktifkan.");
            }

            try
            {
                _repository.Delete(medicineId);
                return ServiceResult.Ok("Obat berhasil dihapus.");
            }
            catch (MySqlException)
            {
                _repository.Deactivate(medicineId);
                return ServiceResult.Ok("Data tidak bisa dihapus karena sudah dipakai. Data akan dinonaktifkan.");
            }
        }
    }
}
