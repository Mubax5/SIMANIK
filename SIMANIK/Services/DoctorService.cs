using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class DoctorService
    {
        private readonly DoctorRepository _doctorRepository;
        private readonly UserRepository _userRepository;

        public DoctorService()
        {
            _doctorRepository = new DoctorRepository();
            _userRepository = new UserRepository();
        }

        public List<DoctorItem> Search(string keyword, string status)
        {
            return _doctorRepository.Search(keyword, status);
        }

        public List<LookupItem> GetDoctorUserOptions(int selectedUserId)
        {
            return _userRepository.GetDoctorUserOptions(selectedUserId);
        }

        public List<LookupItem> GetAvailableDoctorUsers()
        {
            return _userRepository.GetAvailableDoctorUsers();
        }

        public List<LookupItem> GetActiveDoctorOptions()
        {
            return _doctorRepository.GetActiveDoctorOptions();
        }

        public ServiceResult Save(DoctorItem doctor)
        {
            if (doctor == null)
            {
                return ServiceResult.Fail("Data dokter tidak valid.");
            }

            if (doctor.UserId <= 0)
            {
                return ServiceResult.Fail("Akun user dokter wajib dipilih.");
            }

            if (!_doctorRepository.IsDoctorUser(doctor.UserId))
            {
                return ServiceResult.Fail("User harus aktif dan memiliki role Dokter.");
            }

            if (_doctorRepository.IsUserLinkedToDoctor(doctor.UserId, doctor.DoctorId))
            {
                return ServiceResult.Fail("User dokter sudah terhubung ke data dokter lain.");
            }

            if (!ValidationHelper.IsRequired(doctor.FullName))
            {
                return ServiceResult.Fail("Nama dokter wajib diisi.");
            }

            if (!ValidationHelper.IsRequired(doctor.Specialization))
            {
                return ServiceResult.Fail("Spesialisasi wajib diisi.");
            }

            if (!ValidationHelper.IsValidPhoneNumber(doctor.PhoneNumber))
            {
                return ServiceResult.Fail("Nomor telepon tidak valid.");
            }

            doctor.FullName = doctor.FullName.Trim();
            doctor.Specialization = doctor.Specialization.Trim();
            doctor.PhoneNumber = string.IsNullOrWhiteSpace(doctor.PhoneNumber) ? null : doctor.PhoneNumber.Trim();

            if (doctor.DoctorId <= 0)
            {
                _doctorRepository.Insert(doctor);
                return ServiceResult.Ok("Dokter berhasil ditambahkan.");
            }

            _doctorRepository.Update(doctor);
            return ServiceResult.Ok("Dokter berhasil diperbarui.");
        }

        public ServiceResult SetActive(int doctorId, bool isActive)
        {
            if (doctorId <= 0)
            {
                return ServiceResult.Fail("Pilih dokter terlebih dahulu.");
            }

            _doctorRepository.SetActive(doctorId, isActive);
            return ServiceResult.Ok(isActive ? "Dokter berhasil diaktifkan." : "Dokter berhasil dinonaktifkan.");
        }

        public ServiceResult DeleteDoctor(int doctorId)
        {
            if (doctorId <= 0)
            {
                return ServiceResult.Fail("Pilih dokter yang ingin dihapus.");
            }

            if (_doctorRepository.HasRelations(doctorId))
            {
                _doctorRepository.Deactivate(doctorId);
                return ServiceResult.Ok("Data tidak bisa dihapus karena sudah dipakai. Data akan dinonaktifkan.");
            }

            try
            {
                _doctorRepository.Delete(doctorId);
                return ServiceResult.Ok("Dokter berhasil dihapus.");
            }
            catch (MySqlException)
            {
                _doctorRepository.Deactivate(doctorId);
                return ServiceResult.Ok("Data tidak bisa dihapus karena sudah dipakai. Data akan dinonaktifkan.");
            }
        }
    }
}
