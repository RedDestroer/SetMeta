using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SetMeta.Util
{
    public static class DataConversion
    {
        /// <summary>
        /// ODBC date format
        /// </summary>
        public static readonly string DateTimeOdbcFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// ODBC date format with milliseconds
        /// </summary>
        public static readonly string DateTimeOdbcWithMsFormat = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// ISO8601 date format
        /// </summary>
        public static readonly string DateTimeIso8601Format = "yyyy-MM-ddTHH:mm:ss.fff";

        private static readonly Dictionary<Type, ITryParser> Parsers;
        private static string[] _dateTimeFormats = { DateTimeIso8601Format, DateTimeOdbcWithMsFormat, DateTimeOdbcFormat };

        private static IFormatProvider _formatProvider;

        static DataConversion()
        {
            FormatProvider = new CultureInfo("en-US");
            Parsers = new Dictionary<Type, ITryParser>();
            AddParser((string input, out byte value) =>
            {
                bool result = byte.TryParse(input, NumberStyles.Number, FormatProvider, out byte value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out byte? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = byte.TryParse(input, NumberStyles.Number, FormatProvider, out byte value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out DateTime value) =>
            {
                if (DateTime.TryParse(input, FormatProvider, DateTimeStyles.None, out var result))
                {
                    value = result;
                    return true;
                }

                if (DateTime.TryParseExact(input, _dateTimeFormats, FormatProvider, DateTimeStyles.None, out result))
                {
                    value = result;
                    return true;
                }

                value = DateTime.MinValue;
                return false;
            });
            AddParser((string input, out DateTime? value) =>
            {
                if (input == null)
                {
                    value = new DateTime?();
                    return true;
                }

                if (DateTime.TryParse(input, FormatProvider, DateTimeStyles.None, out var result))
                {
                    value = result;
                    return true;
                }

                if (DateTime.TryParseExact(input, _dateTimeFormats, FormatProvider, DateTimeStyles.None, out result))
                {
                    value = result;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out short value) =>
            {
                bool result = short.TryParse(input, NumberStyles.Number, FormatProvider, out short value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out short? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = short.TryParse(input, NumberStyles.Number, FormatProvider, out short value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out ushort value) =>
            {
                bool result = ushort.TryParse(input, NumberStyles.Number, FormatProvider, out ushort value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out ushort? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = ushort.TryParse(input, NumberStyles.Number, FormatProvider, out ushort value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out int value) =>
            {
                bool result = int.TryParse(input, NumberStyles.Number, FormatProvider, out int value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out int? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = int.TryParse(input, NumberStyles.Number, FormatProvider, out int value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out uint value) =>
            {
                bool result = uint.TryParse(input, NumberStyles.Number, FormatProvider, out uint value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out uint? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = uint.TryParse(input, NumberStyles.Number, FormatProvider, out uint value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out long value) =>
            {
                bool result = long.TryParse(input, NumberStyles.Number, FormatProvider, out long value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out long? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = long.TryParse(input, NumberStyles.Number, FormatProvider, out long value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out ulong value) =>
            {
                bool result = ulong.TryParse(input, NumberStyles.Number, FormatProvider, out ulong value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out ulong? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = ulong.TryParse(input, NumberStyles.Number, FormatProvider, out ulong value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out bool value) =>
            {
                if (input == "0")
                {
                    value = false;
                    return true;
                }

                if (input == "1")
                {
                    value = true;
                    return true;
                }

                if (bool.TryParse(input, out bool result))
                {
                    value = result;
                    return true;
                }

                value = false;
                return false;
            });
            AddParser((string input, out bool? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                if (input == "0")
                {
                    value = false;
                    return true;
                }

                if (input == "1")
                {
                    value = true;
                    return true;
                }

                if (bool.TryParse(input, out bool result))
                {
                    value = result;
                    return true;
                }

                value = false;
                return false;
            });
            AddParser((string input, out float value) =>
            {
                bool result = float.TryParse(input, NumberStyles.Number, FormatProvider, out float value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Let's try to go round the standards of Russia
                input = input.Replace(',', '.');
                if (input.StartsWith(".") || input.StartsWith(","))
                    input = string.Concat("0", input);
                if (input.EndsWith(".") || input.EndsWith(","))
                    input = string.Concat(input, "0");

                result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                // Last attempt - maybe the locale was actually Russian
                input = input.Replace('.', ',');
                result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out float? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = float.TryParse(input, NumberStyles.Number, FormatProvider, out float value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Let's try to go round the standards of Russia
                input = input.Replace(',', '.');
                if (input.StartsWith(".") || input.StartsWith(","))
                    input = string.Concat("0", input);
                if (input.EndsWith(".") || input.EndsWith(","))
                    input = string.Concat(input, "0");

                result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                // Last attempt - maybe the locale was actually Russian
                input = input.Replace('.', ',');
                result = float.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out double value) =>
            {
                bool result = double.TryParse(input, NumberStyles.Number, FormatProvider, out double value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Let's try to go round the standards of Russia
                input = input.Replace(',', '.');
                if (input.StartsWith(".") || input.StartsWith(","))
                    input = string.Concat("0", input);
                if (input.EndsWith(".") || input.EndsWith(","))
                    input = string.Concat(input, "0");

                result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                // Last attempt - maybe the locale was actually Russian
                input = input.Replace('.', ',');
                result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out double? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = double.TryParse(input, NumberStyles.Number, FormatProvider, out double value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Let's try to go round the standards of Russia
                input = input.Replace(',', '.');
                if (input.StartsWith(".") || input.StartsWith(","))
                    input = string.Concat("0", input);
                if (input.EndsWith(".") || input.EndsWith(","))
                    input = string.Concat(input, "0");

                result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                // Last attempt - maybe the locale was actually Russian
                input = input.Replace('.', ',');
                result = double.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out decimal value) =>
            {
                bool result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out decimal value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Let's try to go round the standards of Russia
                input = input.Replace(',', '.');
                if (input.StartsWith(".") || input.StartsWith(","))
                    input = string.Concat("0", input);
                if (input.EndsWith(".") || input.EndsWith(","))
                    input = string.Concat(input, "0");

                result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                // Last attempt - maybe the locale was actually Russian
                input = input.Replace('.', ',');
                result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = 0;
                return false;
            });
            AddParser((string input, out decimal? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                bool result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out decimal value2);

                if (result)
                {
                    value = value2;
                    return true;
                }

                // Let's try to go round the standards of Russia
                input = input.Replace(',', '.');
                if (input.StartsWith(".") || input.StartsWith(","))
                    input = string.Concat("0", input);
                if (input.EndsWith(".") || input.EndsWith(","))
                    input = string.Concat(input, "0");

                result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                // Last attempt - maybe the locale was actually Russian
                input = input.Replace('.', ',');
                result = decimal.TryParse(input, NumberStyles.Number, FormatProvider, out value2);
                if (result)
                {
                    value = value2;
                    return true;
                }

                value = null;
                return false;
            });
            AddParser((string input, out string value) =>
            {
                value = input;
                return true;
            });
            AddParser<Guid>(Guid.TryParse);
            AddParser((string input, out Guid? value) =>
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        value = null;
                        return true;
                    }

                    try
                    {
                        value = Guid.Parse(input);
                        return true;
                    }
                    catch
                    {
                        value = null;
                        return false;
                    }
                });
            AddParser((string input, out TimeSpan value) =>
            {
                try
                {
                    value = TimeSpan.Parse(input, FormatProvider);
                    return true;
                }
                catch
                {
                    value = TimeSpan.FromSeconds(0);
                    return false;
                }
            });
            AddParser((string input, out TimeSpan? value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                try
                {
                    value = TimeSpan.Parse(input, FormatProvider);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            });
            AddParser<Version>(Version.TryParse);
            AddParser((string input, out byte[] value) =>
            {
                if (string.IsNullOrEmpty(input))
                {
                    value = null;
                    return true;
                }

                try
                {
                    value = System.Convert.FromBase64String(input);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            });
        }

        /// <summary>
        /// The provider of the format that is used when parsing
        /// </summary>
        /// <exception cref="ArgumentNullException">If value null</exception>
        public static IFormatProvider FormatProvider
        {
            get => _formatProvider;
            set => _formatProvider = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Add a date format option that will be parsed when parsing dates
        /// </summary>
        /// <param name="format"></param>
        public static void AddDateTimeFormat(string format)
        {
            lock (typeof(DataConversion))
            {
                if (_dateTimeFormats.Any(o => string.Equals(o, format)))
                    return;

                var data = new string[_dateTimeFormats.Length + 1];
                Array.Copy(_dateTimeFormats, data, _dateTimeFormats.Length);
                data[data.Length - 1] = format;

                _dateTimeFormats = data;
            }
        }

        /// <summary>
        /// Add parser
        /// </summary>
        /// <typeparam name="T">Value type, which parser return</typeparam>
        /// <param name="parseMethod">parser delegate</param>
        public static void AddParser<T>(TryParseMethod<T> parseMethod)
        {
            Parsers[typeof(T)] = new TryParser<T>(parseMethod);
        }

        /// <summary>
        /// Convert string into expected type
        /// </summary>
        /// <typeparam name="T">Value type, which parser return</typeparam>
        /// <param name="input">Input string</param>
        /// <returns></returns>
        public static T Convert<T>(string input)
        {
            if (TryConvert(input, out T answer))
                return answer;

            throw new InvalidOperationException($"Unable to convert value '{input}' to type '{typeof(T).FullName}'.");
        }

        /// <summary>
        /// Try to convert string into expected type
        /// </summary>
        /// <typeparam name="T">Value type, which parser return</typeparam>
        /// <param name="input">Input string</param>
        /// <param name="value">Output value</param>
        /// <returns></returns>
        public static bool TryConvert<T>(string input, out T value)
        {
            var type = typeof(T);

            bool success = TryConvert(type, input, out var parseResult);
            if (success)
            {
                value = (T)parseResult;
            }
            else
            {
                if (type.IsEnum)
                {
                    success = TryGetEnumValue(typeof(T), input, out var resultValue);
                    if (success)
                        value = (T)resultValue;
                    else
                        value = default(T);
                }
                else
                {
                    value = default(T);
                }
            }

            return success;
        }

        /// <summary>
        /// Try to convert string into expected type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryConvert(Type type, string input, out object value)
        {
            if (Parsers.TryGetValue(type, out var parser))
                return parser.TryParse(input, out value);

            value = null;

            return false;
        }

        /// <summary>
        /// Returns value Enum from string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T GetEnumValue<T>(string input, bool ignoreCase = true)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new InvalidOperationException("Transferred type is not Enum.");

            return (T)Enum.Parse(type, input, ignoreCase);
        }

        /// <summary>
        /// Try parse Enum type
        /// </summary>
        /// <typeparam name="T">Value type which parser returns and which must be Enum</typeparam>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetEnumValue<T>(string input, out T value)
        {
            bool result = TryGetEnumValue(typeof(T), input, out var resultValue);
            if (result)
                value = (T)resultValue;
            else
                value = default(T);

            return result;
        }

        /// <summary>
        /// Try parse Enum type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetEnumValue(Type type, string input, out object value)
        {
            try
            {
                if (!type.IsEnum)
                {
                    value = null;
                    return false;
                }

                value = Enum.Parse(type, input, true);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}