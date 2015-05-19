using System;
using Microsoft.SPOT;
using System.Collections;

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

            string result = string.Empty;

            char c;
            bool encodeChar;
            for (int i = 0; i < value.Length; i++)
            {
                c = value[i];

                // Do we need to encode this character?
                encodeChar = true;

                // Safe characters defined by RFC3986: http://oauth.net/core/1.0/#encoding_parameters
                if (c >= '0' && c <= '9')
                    encodeChar = false;
                if (c >= 'A' && c <= 'Z')
                    encodeChar = false;
                if (c >= 'a' && c <= 'z')
                    encodeChar = false;
                switch (c)
                {
                    case '-':
                    case '.':
                    case '_':
                    case '~':
                        encodeChar = false;
                        break;
                }

                if (encodeChar)
                    result += '%' + ((byte)c).ToString("X2");
                else
                    result += value[i];
            }

            return result;
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
    }
}
