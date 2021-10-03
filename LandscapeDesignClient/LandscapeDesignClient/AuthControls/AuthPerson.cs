using LandscapeDesignClient.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LandscapeDesignClient.AuthControls
{
    public static class AuthPerson
    {
        private static Regex regexName = new Regex(@"^(?=.*[а-яА-ЯёЁa-zA-Z0-9]$)[а-яА-ЯёЁa-zA-Z][а-яА-ЯёЁa-zA-Z0-9 .'-]{2,50}$");
        private static Regex regexEmail = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
        private static Regex regexPassword = new Regex(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+-_=])(?=\S+$).{8,20}$");

        public static int ValidateEmail(string email)
        {
            if (email == null || email.Trim().Length == 0)
                return 201;
            if (!regexEmail.IsMatch(email))
                return 202;
            return -1;
        }
        public static int ValidatePassword(string password)
        {
            if (password == null || password.Trim().Length == 0)
                return 203;
            if (!regexPassword.IsMatch(password))
                return 204;
            return -1;
        }
        public static int ValidateName(string name)
        {
            if (name == null || name.Length == 0 ||name.Trim().Length == 0)
                return -1;
            if (!regexName.IsMatch(name))
                return 205;
            return -1;
        }
        public static int ValidatePasswordCopy(string password, string passwordConfirm)
        {
            int error = ValidatePassword(password);
            if (error != -1)
                return error;
            if (passwordConfirm == null || passwordConfirm.Length == 0)
                return 206;
            if (password != passwordConfirm)
                return 207;
            return -1;
        }
    }
}
