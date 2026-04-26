using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class RegisterPasienRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string KonfirmasiPassword { get; set; }
        public string NamaLengkap { get; set; }
        public DateTime? TanggalLahir { get; set; }
        public JenisKelamin JenisKelamin { get; set; }
        public string Alamat { get; set; }
        public string NoTelepon { get; set; }
    }

    public class PasienService
    {
        private readonly UserRepository _userRepository;
        private readonly PasienRepository _pasienRepository;

        public PasienService()
        {
            _userRepository = new UserRepository();
            _pasienRepository = new PasienRepository();
        }

        public ServiceResult RegisterPasien(RegisterPasienRequest request)
        {
            string validationMessage = ValidateRegisterRequest(request);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return ServiceResult.Fail(validationMessage);
            }

            using (MySqlConnection connection = DatabaseHelper.OpenConnection())
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    string username = request.Username.Trim();

                    if (_userRepository.IsUsernameExists(username, connection, transaction))
                    {
                        transaction.Rollback();
                        return ServiceResult.Fail("Username sudah digunakan.");
                    }

                    User user = new User
                    {
                        Username = username,
                        PasswordHash = HashPassword(request.Password),
                        NamaLengkap = request.NamaLengkap.Trim(),
                        Role = UserRole.Pasien,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    int userId = _userRepository.Insert(user, connection, transaction);
                    string noRekamMedis = _pasienRepository.GenerateNextNoRekamMedis(connection, transaction);

                    Pasien pasien = new Pasien
                    {
                        UserId = userId,
                        NoRekamMedis = noRekamMedis,
                        NamaLengkap = request.NamaLengkap.Trim(),
                        JenisKelamin = request.JenisKelamin,
                        TanggalLahir = request.TanggalLahir.Value.Date,
                        NoTelepon = request.NoTelepon.Trim(),
                        Alamat = string.IsNullOrWhiteSpace(request.Alamat) ? null : request.Alamat.Trim(),
                        CreatedAt = DateTime.Now
                    };

                    int pasienId = _pasienRepository.Insert(pasien, connection, transaction);
                    _pasienRepository.InsertEmptyMedicalRecordIfTableExists(pasienId, noRekamMedis, connection, transaction);

                    transaction.Commit();
                    return ServiceResult.Ok("Registrasi pasien berhasil.");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private static string ValidateRegisterRequest(RegisterPasienRequest request)
        {
            if (request == null)
            {
                return "Data registrasi tidak valid.";
            }

            if (!ValidationHelper.IsRequired(request.Username))
            {
                return "Username wajib diisi.";
            }

            if (!ValidationHelper.IsRequired(request.Password))
            {
                return "Password wajib diisi.";
            }

            if (request.Password != request.KonfirmasiPassword)
            {
                return "Password dan konfirmasi password harus sama.";
            }

            if (!ValidationHelper.IsRequired(request.NamaLengkap))
            {
                return "Nama lengkap wajib diisi.";
            }

            if (!request.TanggalLahir.HasValue || request.TanggalLahir.Value.Date > DateTime.Today)
            {
                return "Tanggal lahir tidak valid.";
            }

            if (request.JenisKelamin == JenisKelamin.TidakDiketahui)
            {
                return "Jenis kelamin wajib dipilih.";
            }

            if (!ValidationHelper.IsRequired(request.NoTelepon))
            {
                return "Nomor telepon wajib diisi.";
            }

            if (!ValidationHelper.IsValidPhoneNumber(request.NoTelepon))
            {
                return "Nomor telepon tidak valid.";
            }

            return null;
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder(hash.Length * 2);

                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
