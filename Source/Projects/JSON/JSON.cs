using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.Foundation.Metadata;

namespace WinRTJSON
{
    ///<summary>
    ///    This class encodes and decodes JSON strings.
    ///    Spec. details, see http://www.json.org/
    ///
    ///    JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and Hashtable.
    ///    All numbers are parsed to doubles.
    /// 
    ///     Found at
    /// 
    ///     http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
    /// 
    ///     Converted on 09/03/2012 to work with WinRT by changing Hashtable to IDictionary<string, object> and ArrayList to List<object>
    ///</summary>
    public sealed class JSON
    {
        internal const int TokenNone = 0;
        internal const int TokenCurlyOpen = 1;
        internal const int TokenCurlyClose = 2;
        internal const int TokenSquaredOpen = 3;
        internal const int TokenSquaredClose = 4;
        internal const int TokenColon = 5;
        internal const int TokenComma = 6;
        internal const int TokenString = 7;
        internal const int TokenNumber = 8;
        internal const int TokenTrue = 9;
        internal const int TokenFalse = 10;
        internal const int TokenNull = 11;

        private const int BuilderCapacity = 2000;

        /// <summary>
        ///     Parses the string json into a value
        /// </summary>
        /// <param name="json"> A JSON string. </param>
        /// <returns> An ArrayList, a Hashtable, a double, a string, null, true, or false </returns>
        public static object JsonDecode(string json)
        {
            return JsonDecode(json, true);
        }

        /// <summary>
        ///     Parses the string json into a value; and fills 'success' with the successfullness of the parse.
        /// </summary>
        /// <param name="json"> A JSON string. </param>
        /// <param name="success"> Successful parse? </param>
        /// <returns> An ArrayList, a Hashtable, a double, a string, null, true, or false </returns>
        [DefaultOverload]
        public static object JsonDecode(string json, bool success)
        {
            success = true;
            if (json == null) return null;

            var charArray = json.ToCharArray();
            var index = 0;
            var value = ParseValue(charArray, ref index, ref success);

            return value;
        }

        /// <summary>
        ///     Converts a Hashtable / ArrayList object into a JSON string
        /// </summary>
        /// <param name="json"> A Hashtable / ArrayList </param>
        /// <returns> A JSON encoded string, or null if object 'json' is not serializable </returns>
        public static string JsonEncode(object json)
        {
            var builder = new StringBuilder(BuilderCapacity);
            var success = SerializeValue(json, builder);
            return (success ? builder.ToString() : null);
        }

        internal static IList<object> ParseArray(char[] json, ref int index, ref bool success)
        {
            var array = new List<object>();

            // [
            NextToken(json, ref index);

            var done = false;
            while (!done)
            {
                var token = LookAhead(json, index);
                if (token == TokenNone)
                {
                    success = false;
                    return null;
                }
                else if (token == TokenComma) NextToken(json, ref index);
                else if (token == TokenSquaredClose)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    var value = ParseValue(json, ref index, ref success);
                    if (!success) return null;

                    array.Add(value);
                }
            }

            return array;
        }

        internal static double ParseNumber(char[] json, ref int index, ref bool success)
        {
            EatWhitespace(json, ref index);

            var lastIndex = GetLastIndexOfNumber(json, index);
            var charLength = (lastIndex - index) + 1;

            double number;
            success = Double.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);

            index = lastIndex + 1;
            return number;
        }

        internal static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
        {
            var table = new Dictionary<string, object>();

            // {
            NextToken(json, ref index);

            var done = false;
            while (!done)
            {
                var token = LookAhead(json, index);
                if (token == TokenNone)
                {
                    success = false;
                    return null;
                }
                if (token == TokenComma) NextToken(json, ref index);
                else if (token == TokenCurlyClose)
                {
                    NextToken(json, ref index);
                    return table;
                }
                else
                {
                    // name
                    var name = ParseString(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    // :
                    token = NextToken(json, ref index);
                    if (token != TokenColon)
                    {
                        success = false;
                        return null;
                    }

                    // value
                    var value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    table[name] = value;
                }
            }

            return table;
        }

        internal static string ParseString(char[] json, ref int index, ref bool success)
        {
            var s = new StringBuilder(BuilderCapacity);
            char c;

            EatWhitespace(json, ref index);

            // "
            c = json[index++];

            var complete = false;
            while (!complete)
            {
                if (index == json.Length) break;

                c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                else if (c == '\\')
                {
                    if (index == json.Length) break;
                    c = json[index++];
                    if (c == '"') s.Append('"');
                    else if (c == '\\') s.Append('\\');
                    else if (c == '/') s.Append('/');
                    else if (c == 'b') s.Append('\b');
                    else if (c == 'f') s.Append('\f');
                    else if (c == 'n') s.Append('\n');
                    else if (c == 'r') s.Append('\r');
                    else if (c == 't') s.Append('\t');
                    else if (c == 'u')
                    {
                        var remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;
                            if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint))) return "";
                            // convert the integer codepoint to a unicode char and add to string
                            s.Append(Char.ConvertFromUtf32((int)codePoint));
                            // skip 4 chars
                            index += 4;
                        }
                        else break;
                    }
                }
                else s.Append(c);
            }

            if (!complete)
            {
                success = false;
                return null;
            }

            return s.ToString();
        }

        internal static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case TokenString:
                    return ParseString(json, ref index, ref success);
                case TokenNumber:
                    return ParseNumber(json, ref index, ref success);
                case TokenCurlyOpen:
                    return ParseObject(json, ref index, ref success);
                case TokenSquaredOpen:
                    return ParseArray(json, ref index, ref success);
                case TokenTrue:
                    NextToken(json, ref index);
                    return true;
                case TokenFalse:
                    NextToken(json, ref index);
                    return false;
                case TokenNull:
                    NextToken(json, ref index);
                    return null;
                case TokenNone:
                    break;
            }

            success = false;
            return null;
        }

        internal static bool SerializeArray(IList<object> anArray, StringBuilder builder)
        {
            builder.Append("[");

            var first = true;
            for (var i = 0; i < anArray.Count; i++)
            {
                var value = anArray[i];

                if (!first) builder.Append(", ");

                if (!SerializeValue(value, builder)) return false;

                first = false;
            }

            builder.Append("]");
            return true;
        }

        internal static bool SerializeNumber(double number, StringBuilder builder)
        {
            builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
            return true;
        }

        internal static bool SerializeObject(IDictionary<string, object> anObject, StringBuilder builder)
        {
            builder.Append("{");

            var first = true;

            foreach(var e in anObject)
            {
                var key = e.Key;
                var value = e.Value;

                if (!first) builder.Append(", ");

                SerializeString(key, builder);
                builder.Append(":");
                if (!SerializeValue(value, builder)) return false;

                first = false;
            }

            builder.Append("}");
            return true;
        }

        internal static bool SerializeString(string aString, StringBuilder builder)
        {
            builder.Append("\"");

            var charArray = aString.ToCharArray();
            for (var i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];
                if (c == '"') builder.Append("\\\"");
                else if (c == '\\') builder.Append("\\\\");
                else if (c == '\b') builder.Append("\\b");
                else if (c == '\f') builder.Append("\\f");
                else if (c == '\n') builder.Append("\\n");
                else if (c == '\r') builder.Append("\\r");
                else if (c == '\t') builder.Append("\\t");
                else
                {
                    var codepoint = Convert.ToInt32(c);
                    if ((codepoint >= 32) && (codepoint <= 126)) builder.Append(c);
                    else builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                }
            }

            builder.Append("\"");
            return true;
        }

        internal static bool SerializeValue(object value, StringBuilder builder)
        {
            var success = true;

            if (value is string) success = SerializeString((string)value, builder);
            else if (value is IDictionary<string, object>) success = SerializeObject((IDictionary<string, object>)value, builder);
            else if (value is IList<object>) success = SerializeArray((IList<object>)value, builder);
            else if ((value is Boolean) && (Boolean)value) builder.Append("true");
            else if ((value is Boolean) && ((Boolean)value == false)) builder.Append("false");
            else if (value is ValueType)
            {
                // thanks to ritchie for pointing out ValueType to me
                success = SerializeNumber(Convert.ToDouble(value), builder);
            }
            else if (value == null) builder.Append("null");
            else success = false;
            return success;
        }

        private static void EatWhitespace(char[] json, ref int index)
        {
            for (; index < json.Length; index++) if (" \t\n\r".IndexOf(json[index]) == -1) break;
        }

        private static int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;

            for (lastIndex = index; lastIndex < json.Length; lastIndex++) if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) break;
            return lastIndex - 1;
        }

        private static int LookAhead(char[] json, int index)
        {
            var saveIndex = index;
            return NextToken(json, ref saveIndex);
        }

        private static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);

            if (index == json.Length) return TokenNone;

            var c = json[index];
            index++;
            switch (c)
            {
                case '{':
                    return TokenCurlyOpen;
                case '}':
                    return TokenCurlyClose;
                case '[':
                    return TokenSquaredOpen;
                case ']':
                    return TokenSquaredClose;
                case ',':
                    return TokenComma;
                case '"':
                    return TokenString;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return TokenNumber;
                case ':':
                    return TokenColon;
            }
            index--;

            var remainingLength = json.Length - index;

            // false
            if (remainingLength >= 5)
            {
                if (json[index] == 'f' &&
                    json[index + 1] == 'a' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 's' &&
                    json[index + 4] == 'e')
                {
                    index += 5;
                    return TokenFalse;
                }
            }

            // true
            if (remainingLength >= 4)
            {
                if (json[index] == 't' &&
                    json[index + 1] == 'r' &&
                    json[index + 2] == 'u' &&
                    json[index + 3] == 'e')
                {
                    index += 4;
                    return TokenTrue;
                }
            }

            // null
            if (remainingLength >= 4)
            {
                if (json[index] == 'n' &&
                    json[index + 1] == 'u' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 'l')
                {
                    index += 4;
                    return TokenNull;
                }
            }

            return TokenNone;
        }
    }
}