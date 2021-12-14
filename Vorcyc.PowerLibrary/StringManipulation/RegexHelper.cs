using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Vorcyc.PowerLibrary.StringManipulation
{
    public static class RegexHelper
    {
        public static Regex PhoneNumber = new Regex(Regex_Phone, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static string Regex_Phone = @"((\d{3,4})-(\d{7,8}))|((\d{3,4})-(\d{7,8})-(\d{1,4})|(\d{11})|(\d{10})|(\d{7}))";

        public static string GetAllPhoneNumbers(string source)
        {
            StringBuilder builder = new StringBuilder(1000);
            for (Match match = PhoneNumber.Match(source); match.Success; match = match.NextMatch())
            {
                string str = match.Groups[0].ToString();
                builder.Append('{');
                builder.Append(str);
                builder.Append('}');
            }
            return builder.ToString();
        }
    }

 

}
