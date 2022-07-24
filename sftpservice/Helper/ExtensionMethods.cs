using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Helper
{
    /// <summary>
    /// It is a helper class, which supplies various methods to the application business.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get All Months
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<Int32, String>> GetAllMonths()
        {
            var monthList = new List<KeyValuePair<int, String>>();
            for (var i = 1; i <= 12; i++)
            {
                if (DateTimeFormatInfo.CurrentInfo != null)
                    monthList.Add(new KeyValuePair<Int32, String>(i, DateTimeFormatInfo.CurrentInfo.GetMonthName(i)));
            }
            return monthList;
        }

        /// <summary>
        /// Checking a certain value whether digit or not using IsAllDigits method.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Converting the DateTime value into UnixTimestamp value using the method ToUnixTimestamp.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(DateTime value)
        {
            return (long)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        /// <summary>
        /// Convert the string to Pascal case.
        /// </summary>
        /// <param name="the_string"></param>
        /// <returns></returns>
        public static string ToPascalCase(string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return string.Empty;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// Convert the string to camel case.
        /// </summary>
        /// <param name="the_string"></param>
        /// <returns></returns>
        public static string ToCamelCase(string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2)
                return string.Empty;

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        /// <summary>
        /// Capitalize the first character and add a space before
        /// </summary>
        /// <param name="the_string"></param>
        /// <returns></returns>
        public static string ToProperCase(string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return string.Empty;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Start with the first character.
            string result = the_string.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (int i = 1; i < the_string.Length; i++)
            {
                if (char.IsUpper(the_string[i])) result += " ";
                result += the_string[i];
            }

            return result;
        }

        /// <summary>
        /// Converting first letter of a signle word into lower case.
        /// </summary>
        /// <param name="pasCamWord"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string FirstCharToLower(string pasCamWord)
        {
            switch (pasCamWord)
            {
                case null: throw new ArgumentNullException(nameof(pasCamWord));
                case "": throw new ArgumentException($"{nameof(pasCamWord)} {ConstantSupplier.CANNOT_BE_EMPTY_MSG}", nameof(pasCamWord));
                default: return pasCamWord.First().ToString().ToLower() + pasCamWord.Substring(1);
            }
        }

        /// <summary>
        /// Extract Value out of any given substring
        /// </summary>
        /// <param name="input"></param>
        /// <param name="strFrom"></param>
        /// <param name="strTo"></param>
        /// <returns></returns>
        public static string GetStringFromResponse(string input, string strFrom, string strTo)
        {
            return input.Substring((input.IndexOf(strFrom) + strFrom.Length), ((input.IndexOf(strTo)) - (input.IndexOf(strFrom) + strFrom.Length)));
        }

        public static string FtpDirectory(string rootDirectory)
        {
            rootDirectory = rootDirectory.Trim('/');
            return string.Format(@"/{0}/", rootDirectory);
        }

        public static string CombineDirectory(string rootDirectory, string childDirectory)
        {
            rootDirectory = rootDirectory.Trim('/');
            childDirectory = childDirectory.Trim('/');
            return string.Format(@"/{0}/{1}/", rootDirectory, childDirectory);
        }

        public static string CombineFile(string rootDirectory, string filePathOrName)
        {
            rootDirectory = rootDirectory.Trim('/'); ;
            filePathOrName = filePathOrName.Trim('/'); ;
            return string.Format(@"/{0}/{1}", rootDirectory, filePathOrName);
        }

        public static string ServerDetails(string host, string port, string userName, string type = ConstantSupplier.REMOTE_TYPE_FTP)
        {
            return String.Format("Type: '{3}' Host:'{0}' Port:'{1}' User:'{2}'", host, port, userName, type);
        }

        public static List<string> GetFiles(string path)
        {
            List<string> list = new();
            foreach (var filename in Directory.GetFiles(path))
            {
                list.Add(filename);
            }
            return list;
        }
    }
}
