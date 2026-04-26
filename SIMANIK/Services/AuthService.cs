using SIMANIK.Helpers;
using SIMANIK.Models;
using SIMANIK.Repositories;

namespace SIMANIK.Services
{
    public class LoginResult : ServiceResult
    {
        public User User { get; private set; }

        private LoginResult(bool success, string message, User user)
            : base(success, message)
        {
            User = user;
        }

        public static LoginResult Ok(User user)
        {
            return new LoginResult(true, "Login berhasil.", user);
        }

        public new static LoginResult Fail(string message)
        {
            return new LoginResult(false, message, null);
        }
    }

    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public LoginResult Login(string username, string password)
        {
            if (!ValidationHelper.IsRequired(username))
            {
                return LoginResult.Fail("Username wajib diisi.");
            }

            if (!ValidationHelper.IsRequired(password))
            {
                return LoginResult.Fail("Password wajib diisi.");
            }

            User user = _userRepository.FindByUsernameAndPassword(username.Trim(), password);

            if (user == null)
            {
                return LoginResult.Fail("Username atau password salah.");
            }

            if (!user.IsActive)
            {
                return LoginResult.Fail("Akun tidak aktif.");
            }

            SessionHelper.SetCurrentUser(user);
            return LoginResult.Ok(user);
        }

        public void Logout()
        {
            SessionHelper.Clear();
        }
    }
}
