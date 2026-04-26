using System;
using System.Text.RegularExpressions;

namespace SIMANIK.Helpers
{
    public static class ValidationHelper
    {
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const string PhonePattern = @"^[0-9+\-\s]{8,20}$";

        public static bool IsRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool HasMinimumLength(string value, int minimumLength)
        {
            return value != null && value.Trim().Length >= minimumLength;
        }

        public static bool IsValidEmail(string email)
        {
            return string.IsNullOrWhiteSpace(email) || Regex.IsMatch(email.Trim(), EmailPattern);
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            return string.IsNullOrWhiteSpace(phoneNumber) || Regex.IsMatch(phoneNumber.Trim(), PhonePattern);
        }

        public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate;
        }
    }
}
