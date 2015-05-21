using System;
using Microsoft.SPOT;
using System.Collections;
using System.Text;

namespace MicroTweet
{
    internal static class Utility
    {
        private static readonly Random _random = new Random();
        private const string _alphanumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Generates a random alphanumeric string of the specified length.
        /// </summary>
        /// <param name="length">The length of the string to be generated.</param>
        public static string GetRandomAlphanumericString(int length)
        {
            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
                randomChars[i] = _alphanumericChars[_random.Next(_alphanumericChars.Length)];

            return new string(randomChars);
        }

        /// <summary>
        /// Converts the value of the DateTime instance to a Unix timestamp (the number of seconds since January 1, 1970).
        /// </summary>
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan timeSpan = (dateTime - epoch);
            return timeSpan.Ticks / 10000000;
        }

        /// <summary>
        /// Applies percent-encoding (%xx) to a string value according to RFC3986.
        /// </summary>
        /// <param name="value">The string value to encode.</param>
        public static string UriEncode(string value)
        {
            if (value == null)
                return null;

            StringBuilder sb = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte b;
            bool encodeByte;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i];

                // Do we need to encode this byte?
                encodeByte = true;

                // Safe characters defined by RFC3986: http://oauth.net/core/1.0/#encoding_parameters
                if ((b >= '0' && b <= '9') ||
                    (b >= 'A' && b <= 'Z') ||
                    (b >= 'a' && b <= 'z') ||
                    b == '-' || b == '.' || b == '_' || b == '~')
                    encodeByte = false;

                if (encodeByte)
                {
                    sb.Append('%');
                    sb.Append(b.ToString("X2"));
                }
                else
                {
                    sb.Append((char)b);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sorts a list of IComparable objects.
        /// </summary>
        /// <param name="list">The list to be sorted.</param>
        public static void SortList(IList list)
        {
            if (list.Count <= 1)
                return;

            // Simple insertion sort
            int i, j;
            IComparable item;

            for (i = 1; i < list.Count; i++)
            {
                item = (IComparable)list[i];
                for (j = i - 1; j >= 0 && item.CompareTo(list[j]) < 0; j--)
                    list[j + 1] = list[j];
                list[j + 1] = item;
            }
        }

        /// <summary>
        /// Concatenates all the elements of a string array, using the specified separator between each element.
        /// </summary>
        /// <param name="separator">The string to use as a separator.</param>
        /// <param name="values">An array that contains the elements to concatenate.</param>
        public static string StringJoin(string separator, string[] values)
        {
            string result = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    result += separator;
                result += values[i];
            }

            return result;
        }

        /// <summary>
        /// Converts bytes to an RFC4648-compliant Base64 string.
        /// </summary>
        /// <param name="value">The bytes to convert.</param>
        public static string ConvertToRFC4648Base64String(byte[] value)
        {
            // NETMF's Convert.ToBase64String() does not follow RFC4648 by default. '!' is used in place of '+' and '*' is used in place of '/'.
            // A static property (Convert.UseRFC4648Encoding) was added in NETMF 4.3 to fix this, but setting it could affect other parts of user code.
            // Instead, we'll just search for and replace the incorrect characters.

            char[] resultChars = Convert.ToBase64String(value).ToCharArray();

            for (int i = 0; i < resultChars.Length; i++)
            {
                if (resultChars[i] == '!')
                    resultChars[i] = '+';
                else if (resultChars[i] == '*')
                    resultChars[i] = '/';
            }

            return new string(resultChars);
        }

        /// <summary>
        /// Converts a uint value between big-endian and little-endian representations.
        /// </summary>
        /// <param name="value">The uint value to convert.</param>
        public static uint ReverseBytes(uint value)
        {
            return
                (value & 0x000000FFu) << 24 |
                (value & 0x0000FF00u) << 8 |
                (value & 0x00FF0000u) >> 8 |
                (value & 0xFF000000u) >> 24;
        }

        /// <summary>
        /// Converts a ulong value between big-endian and little-endian representations.
        /// </summary>
        /// <param name="value">The ulong value to convert.</param>
        public static ulong ReverseBytes(ulong value)
        {
            return
                (value & 0x00000000000000FFul) << 56 |
                (value & 0x000000000000FF00ul) << 40 |
                (value & 0x0000000000FF0000ul) << 24 |
                (value & 0x00000000FF000000ul) << 8 |
                (value & 0x000000FF00000000ul) >> 8 |
                (value & 0x0000FF0000000000ul) >> 24 |
                (value & 0x00FF000000000000ul) >> 40 |
                (value & 0xFF00000000000000ul) >> 56;
        }

        /// <summary>
        /// Parses a Twitter date/time string (e.g., "Tue May 19 19:53:49 +0000 2015").
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        public static DateTime ParseTwitterDateTime(string value)
        {
            // Year
            int year = int.Parse(value.Substring(26, 4));

            // Month
            int month;
            switch (value.Substring(4, 3))
            {
                case "Jan": month = 1; break;
                case "Feb": month = 2; break;
                case "Mar": month = 3; break;
                case "Apr": month = 4; break;
                case "May": month = 5; break;
                case "Jun": month = 6; break;
                case "Jul": month = 7; break;
                case "Aug": month = 8; break;
                case "Sep": month = 9; break;
                case "Oct": month = 10; break;
                case "Nov": month = 11; break;
                case "Dec": month = 12; break;
                default: throw new Exception();
            }

            // Day
            int day = int.Parse(value.Substring(8, 2));

            // Hour
            int hour = int.Parse(value.Substring(11, 2));

            // Minute
            int minute = int.Parse(value.Substring(14, 2));

            // Second
            int second = int.Parse(value.Substring(17, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
