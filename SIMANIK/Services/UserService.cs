using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService()
        {
            _repository = new UserRepository();
        }

        public List<UserListItem> Search(string keyword, string role, string status)
        {
            return _repository.Search(keyword, role, status);
        }

        public ServiceResult Save(int userId, string username, string password, string roleText, bool isActive)
        {
            if (!ValidationHelper.IsRequired(username))
            {
                return ServiceResult.Fail("Username wajib diisi.");
            }

            UserRole role;
            if (!Enum.TryParse(roleText, true, out role))
            {
                return ServiceResult.Fail("Role tidak valid.");
            }

            if (userId <= 0 && !ValidationHelper.IsRequired(password))
            {
                return ServiceResult.Fail("Password wajib diisi saat tambah user.");
            }

            string cleanUsername = username.Trim();
            bool exists = userId <= 0
                ? _repository.IsUsernameExists(cleanUsername)
                : _repository.IsUsernameExists(cleanUsername, userId);

            if (exists)
            {
                return ServiceResult.Fail("Username sudah digunakan.");
            }

            User user = new User
            {
                Id = userId,
                Username = cleanUsername,
                PasswordHash = ValidationHelper.IsRequired(password) ? HashPassword(password) : null,
                Role = role,
                IsActive = isActive,
                CreatedAt = DateTime.Now
            };

            if (userId <= 0)
            {
                _repository.Insert(user);
                return ServiceResult.Ok("User berhasil ditambahkan.");
            }

            _repository.Update(user, ValidationHelper.IsRequired(password));
            return ServiceResult.Ok("User berhasil diperbarui.");
        }

        public ServiceResult SetActive(int userId, bool isActive)
        {
            if (userId <= 0)
            {
                return ServiceResult.Fail("Pilih user terlebih dahulu.");
            }

            _repository.SetActive(userId, isActive);
            return ServiceResult.Ok(isActive ? "User berhasil diaktifkan." : "User berhasil dinonaktifkan.");
        }

        internal static string HashPassword(string password)
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
