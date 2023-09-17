﻿using System.Text.RegularExpressions;

namespace Core.Application.Custom
{
    public static class CustomValidator
    {
        public static bool IsAtLeastNYearsOld(DateTime? dateOfBirth, int year)
        {
            DateTime currentDate = DateTime.Now;
            DateTime minimumBirthDate = currentDate.AddYears(-year);
            return dateOfBirth <= minimumBirthDate;
        }

        public static bool BeValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsAfterToday(DateTime? time)
        {
            DateTime currentDate = DateTime.Now;
            return time > currentDate;
        }
    }
}
