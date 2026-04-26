using System;
using SIMANIK.Models;

namespace SIMANIK.Helpers
{
    public static class SessionHelper
    {
        public static User CurrentUser { get; private set; }

        public static bool IsLoggedIn
        {
            get { return CurrentUser != null; }
        }

        public static UserRole? CurrentRole
        {
            get { return CurrentUser == null ? (UserRole?)null : CurrentUser.Role; }
        }

        public static void SetCurrentUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            CurrentUser = user;
        }

        public static bool HasRole(UserRole role)
        {
            return CurrentUser != null && CurrentUser.Role == role;
        }

        public static void Clear()
        {
            CurrentUser = null;
        }
    }
}
