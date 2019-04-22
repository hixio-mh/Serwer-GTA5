using System;

namespace Extend.Strings
{
    public static class StringExtend
    {
        public static bool IsValidEmail(this string str)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(str);
                return addr.Address == str;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsBetween(this string str, ushort min, ushort max)
        {
            return str.Length >= min && str.Length <= max;
        }
    }
}
