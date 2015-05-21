using System;
using Microsoft.SPOT;
using System.Collections;
using System.Text;

namespace MicroTweet
{
    // JSON reference: http://json.org/
    internal static class Json
    {
        /// <summary>
        /// Parses a JSON-encoded string into a collection of Hashtables, ArrayLists, and values.
        /// </summary>
        /// <param name="input">The JSON-encoded string to parse.</param>
        /// <returns>The decoded root object.</returns>
        public static object Parse(string input)
        {
            return Parse(input.ToCharArray());
        }

        /// <summary>
        /// Parses a JSON-encoded character array into a collection of Hashtables, ArrayLists, and values.
        /// </summary>
        /// <param name="input">The JSON-encoded character array to parse.</param>
        /// <returns>The decoded root object.</returns>
        public static object Parse(char[] input)
        {
            object result;
            TryParse(input, out result, true);
            return result;
        }

        /// <summary>
        /// Attempts to parse a JSON-encoded string into a collection of Hashtables, ArrayLists, and values.
        /// </summary>
        /// <param name="input">The JSON-encoded string to parse.</param>
        /// <param name="result">Returns the decoded root object.</param>
        /// <returns>true is parsing was successful; otherwise, false.</returns>
        public static bool TryParse(string input, out object result)
        {
            return TryParse(input.ToCharArray(), out result);
        }

        /// <summary>
        /// Attempts to parse a JSON-encoded character array into a collection of Hashtables, ArrayLists, and values.
        /// </summary>
        /// <param name="input">The JSON-encoded character array to parse.</param>
        /// <param name="result">Returns the decoded root object.</param>
        /// <returns>true is parsing was successful; otherwise, false.</returns>
        public static bool TryParse(char[] input, out object result)
        {
            return TryParse(input, out result, false);
        }

        private static bool TryParse(char[] input, out object result, bool throwIfError)
        {
            int index = 0;

            // Attempt to parse the root value
            bool success = TryParseValue(input, ref index, out result);

            // Throw an exception if necessary
            if (!success && throwIfError)
                throw new Exception("JSON parse error at index " + index);

            // Done
            return success;
        }

        /// <summary>
        /// Skips ahead to the next non-whitespace character.
        /// </summary>
        private static void SkipWhitespace(char[] input, ref int index)
        {
            while (index < input.Length)
            {
                switch (input[index])
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        index++;
                        break;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Attempts to parse a JSON object, array, or value starting at the specified index.
        /// </summary>
        private static bool TryParseValue(char[] input, ref int index, out object result)
        {
            // Skip whitespace and make sure we're not at the end of the input
            SkipWhitespace(input, ref index);
            if (index >= input.Length)
            {
                result = null;
                return false;
            }

            // Detect the type of value at this position
            char c = input[index];
            switch (c)
            {
                case '{': // Object
                    return TryParseObject(input, ref index, out result);
                case '[': // Array
                    return TryParseArray(input, ref index, out result);
                case '"': // String
                    return TryParseString(input, ref index, out result);
                case 't': // true
                case 'f': // false
                case 'n': // null
                    return TryParseLiteral(input, ref index, out result);
            }
            // Numeric value
            if ((c >= '0' && c <= '9') || c == '-')
                return TryParseNumber(input, ref index, out result);

            // Couldn't determine the type of value
            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a JSON object starting at the specified index.
        /// </summary>
        private static bool TryParseObject(char[] input, ref int index, out object result)
        {
            if (input[index++] != '{')
            {
                result = null;
                return false;
            }

            SkipWhitespace(input, ref index);

            string key;
            object value;
            char c;
            Hashtable hashtable = new Hashtable();

            while (index < input.Length)
            {
                // Are we at the end of the object?
                if (input[index] == '}')
                {
                    index++;
                    result = hashtable;
                    return true;
                }

                // Try to parse the key (as a string)
                if (!TryParseString(input, ref index, out value))
                    break;
                key = (string)value;

                // Skip ahead to the ':' delimiter
                SkipWhitespace(input, ref index);
                if (index >= input.Length)
                    break;
                if (input[index++] != ':')
                    break;

                // Try to parse the value
                if (!TryParseValue(input, ref index, out value))
                    break;

                // Success, add it to the result
                hashtable[key] = value;

                // Skip ahead to the next non-whitespace character
                SkipWhitespace(input, ref index);
                if (index >= input.Length)
                    break;

                c = input[index];

                if (c == '}')
                    continue;

                if (c == ',')
                {
                    index++;
                    SkipWhitespace(input, ref index);
                    continue;
                }

                // Error: there should be a ',' or '}' after each key/value pair
                break;
            }

            // An error occurred or we reached the end of the input stream before finishing
            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a JSON array starting at the specified index.
        /// </summary>
        private static bool TryParseArray(char[] input, ref int index, out object result)
        {
            if (input[index++] != '[')
            {
                result = null;
                return false;
            }

            SkipWhitespace(input, ref index);

            object value;
            char c;
            ArrayList arrayList = new ArrayList();

            while (index < input.Length)
            {
                // Are we at the end of the array?
                if (input[index] == ']')
                {
                    index++;
                    result = arrayList;
                    return true;
                }

                // Try to parse a value
                if (!TryParseValue(input, ref index, out value))
                    break;

                // Success, add it to the list
                arrayList.Add(value);

                // Skip ahead to the next non-whitespace character
                SkipWhitespace(input, ref index);
                if (index >= input.Length)
                    break;

                c = input[index];

                if (c == ']')
                    continue;

                if (c == ',')
                {
                    index++;
                    SkipWhitespace(input, ref index);
                    continue;
                }

                // Error: there should be a ',' or ']' after each value
                break;
            }

            // An error occurred or we reached the end of the input stream before finishing
            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a JSON-encoded string at the specified index.
        /// </summary>
        private static bool TryParseString(char[] input, ref int index, out object result)
        {
            if (input[index++] != '"')
            {
                result = null;
                return false;
            }

            char c;
            StringBuilder sb = new StringBuilder();

            while (index < input.Length)
            {
                c = input[index++];

                // Are we at the end of the string?
                if (c == '"')
                {
                    result = sb.ToString();
                    return true;
                }
                // Is this an escape character?
                else if (c == '\\')
                {
                    // Make sure we can read at least one more char
                    if (index >= input.Length)
                        break;

                    c = input[index++];

                    // What kind of escape character is this?
                    switch (c)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            sb.Append(c);
                            continue;

                        case 'b':
                            sb.Append('\b');
                            continue;

                        case 'f':
                            sb.Append('\f');
                            continue;

                        case 'n':
                            sb.Append('\n');
                            continue;

                        case 'r':
                            sb.Append('\r');
                            continue;

                        case 't':
                            sb.Append('\t');
                            continue;

                        // Arbitrary character (encoded as 4 hex characters)
                        case 'u':
                            // Make sure we can read at least four more chars
                            if (index + 3 >= input.Length)
                                break;
                            try
                            {
                                // Parse the hexadecimal string into a char
                                c = (char)Convert.ToInt32(new string(input, index, 4), 16);
                                index += 4;
                                sb.Append(c);
                                continue;
                            }
                            catch { }
                            break;
                    }

                    // Could not parse the escaped character
                    break;
                }
                else
                {
                    // Normal character, just add it to the result
                    sb.Append(c);
                }
            }

            // An error occurred or we reached the end of the input stream before finishing
            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a JSON literal (true, false, or null) at the specified index.
        /// </summary>
        private static bool TryParseLiteral(char[] input, ref int index, out object result)
        {
            switch (input[index++])
            {
                case 't':
                    if (index + 2 < input.Length && input[index + 0] == 'r' && input[index + 1] == 'u' && input[index + 2] == 'e')
                    {
                        index += 3;
                        result = true;
                        return true;
                    }
                    break;

                case 'f':
                    if (index + 3 < input.Length && input[index + 0] == 'a' && input[index + 1] == 'l' && input[index + 2] == 's' && input[index + 3] == 'e')
                    {
                        index += 4;
                        result = false;
                        return true;
                    }
                    break;

                case 'n':
                    if (index + 2 < input.Length && input[index + 0] == 'u' && input[index + 1] == 'l' && input[index + 2] == 'l')
                    {
                        index += 3;
                        result = null;
                        return true;
                    }
                    break;
            }

            // Could not match "true", "false", or "null" or we reached the end of the input stream before finishing
            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a number at the specified index. The numeric type (long or double) is automatically chosen based on the value.
        /// </summary>
        private static bool TryParseNumber(char[] input, ref int index, out object result)
        {
            char c;
            StringBuilder sb = new StringBuilder();

            // We'll need to know whether the number contains a decimal point (or is expressed in scientific notation)
            bool useDouble = false;

            while (index < input.Length)
            {
                c = input[index];

                if (c >= '0' && c <= '9')
                {
                    sb.Append(c);
                    index++;
                    continue;
                }

                if (c == '.' || c == 'e' || c == 'E' || c == '+' || c == '-')
                {
                    useDouble = true;
                    sb.Append(c);
                    index++;
                    continue;
                }

                // Reached a non-numeric character
                break;
            }

            // Attempt to parse the number
            if (useDouble)
            {
                double value;
                if (double.TryParse(sb.ToString(), out value))
                {

                    result = value;
                    return true;
                }
            }
            else
            {
                try
                {
                    long value = long.Parse(sb.ToString());
                    result = value;
                    return true;
                }
                catch { }
            }

            // An error occurred or we reached the end of the input stream before finishing
            result = null;
            return false;
        }
    }
}
