using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Toprope.Infrastructure.Utilities
{
    /// <summary>
    /// Provides a number of utility methods for dealing with user input.
    /// </summary>
    public static class Input
    {
        #region Properties

        /// <summary>
        /// Gets the culture used for parsing.
        /// </summary>
        private static readonly System.Globalization.CultureInfo Culture = new System.Globalization.CultureInfo("en-US");

        #endregion

        /// <summary>
        /// Returns value indicating whether the given value is likely a boolean.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely a boolean.</returns>
        public static bool IsBool(object o)
        {
            return o != null && (o is bool || string.Compare(o.ToString(), "true", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(o.ToString(), "false", StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        /// <summary>
        /// Returns a boolean representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Boolean representation of the given object.</returns>
        public static bool GetBool(object o)
        {
            bool ret = false;

            if (o != null)
            {
                if (o is bool)
                    ret = (bool)o;
                else
                    bool.TryParse(o.ToString(), out ret);
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given value is likely a double.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely a double.</returns>
        public static bool IsDouble(object o)
        {
            double parsed = 0;

            return o != null && (o is double || (o.ToString().IndexOf(Culture.NumberFormat.NumberDecimalSeparator) > 0 &&
                double.TryParse(o.ToString(), System.Globalization.NumberStyles.Any, Culture, out parsed)));
        }

        /// <summary>
        /// Returns a double representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Double representation of the given object.</returns>
        public static double GetDouble(object o)
        {
            double ret = 0;

            if (o != null)
            {
                if (o is double)
                    ret = (double)o;
                else if (!double.TryParse(o.ToString(), System.Globalization.NumberStyles.AllowLeadingSign, Culture, out ret))
                    double.TryParse(o.ToString(), System.Globalization.NumberStyles.Any, Culture, out ret);
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given value is likely a integer.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely a integer.</returns>
        public static bool IsInt(object o)
        {
            int parsed = 0;

            return o != null && (o is int || int.TryParse(o.ToString(), out parsed));
        }

        /// <summary>
        /// Returns an integer representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Integer representation of the given object.</returns>
        public static int GetInt(object o)
        {
            int ret = 0;

            if (o != null)
            {
                if (o is int)
                    ret = (int)o;
                else if (!int.TryParse(o.ToString(), System.Globalization.NumberStyles.AllowLeadingSign, Culture, out ret))
                    int.TryParse(o.ToString(), System.Globalization.NumberStyles.Any, Culture, out ret);
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given value is likely a long.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely a long.</returns>
        public static bool IsLong(object o)
        {
            long parsed = 0;

            return o != null && (o is long || long.TryParse(o.ToString(), out parsed));
        }

        /// <summary>
        /// Returns an long representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Long representation of the given object.</returns>
        public static long GetLong(object o)
        {
            long ret = 0;

            if (o != null)
            {
                if (o is long)
                    ret = (long)o;
                else if (!long.TryParse(o.ToString(), System.Globalization.NumberStyles.AllowLeadingSign, Culture, out ret))
                    long.TryParse(o.ToString(), System.Globalization.NumberStyles.Any, Culture, out ret);
            }

            return ret;
        }

        /// <summary>
        /// Formats flags.
        /// </summary>
        /// <param name="flags">Flags.</param>
        /// <returns>Formatted flags.</returns>
        public static IEnumerable<string> FormatFlags(Enum flags)
        {
            Type t = flags.GetType();
            List<string> ret = new List<string>();

            foreach (Enum v in Enum.GetValues(t))
            {
                if (flags.HasFlag(v) && (int)((object)v) != 0)
                    ret.Add(Enum.GetName(t, v));
            }

            return ret;
        }

        /// <summary>
        /// HTML encodes tags.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>Content with HTML encoded tags.</returns>
        public static string HtmlEncodeTags(string content)
        {
            return (content ?? string.Empty).Replace("<", "&lt;").Replace(">", "&gt;");
        }

        /// <summary>
        /// HTML decodes tags.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>Content with HTML decoded tags.</returns>
        public static string HtmlDecodeTags(string content)
        {
            return (content ?? string.Empty).Replace("&lt;", "<").Replace("&gt;", ">");
        }

        /// <summary>
        /// Formats the tag from the given input.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Formatted tag.</returns>
        public static string FormatTag(string input)
        {
            string ret = (input ?? string.Empty).ToLowerInvariant().Trim();

            ret = Regex.Replace(Regex.Replace(ret, "[^a-z0-9_\\-]+", "-"), "\\-{2,}", "-");
            ret = ret.Trim().Trim('-').Trim();

            return ret;
        }

        /// <summary>
        /// Formats the given number.
        /// </summary>
        /// <param name="input">Number to format.</param>
        /// <returns>Formatted number.</returns>
        public static string FormatNumber(int input)
        {
            return FormatNumber((long)input);
        }

        /// <summary>
        /// Formats the given number.
        /// </summary>
        /// <param name="input">Number to format.</param>
        /// <returns>Formatted number.</returns>
        public static string FormatNumber(long input)
        {
            return input.ToString("N0", System.Globalization.CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Limits the length of a given string to 125 characters.
        /// </summary>
        /// <param name="input">Input string to process.</param>
        /// <returns>Limited string.</returns>
        public static string Truncate(string input)
        {
            return Truncate(input, 125);
        }

        /// <summary>
        /// Limits the length of a given string to a given number of characters.
        /// </summary>
        /// <param name="input">Input string to process.</param>
        /// <param name="maxChars">Maximum allowed number of characters.</param>
        /// <returns>Limited string.</returns>
        public static string Truncate(string input, int maxChars)
        {
            string ret = input ?? string.Empty;

            if (ret.Length > maxChars && maxChars > 3)
                ret = ret.Substring(0, maxChars - 3) + "...";

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given value is likely a string.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely a string.</returns>
        public static bool IsString(object o)
        {
            return o != null && (o is char || o is string);
        }

        /// <summary>
        /// Returns value indicating whether the given value is likely an email address.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely an email address.</returns>
        public static bool IsEmail(object o)
        {
            return o != null && o is string && Regex.IsMatch(o.ToString(), @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>String representation of the given object.</returns>
        public static string GetString(object o)
        {
            string ret = string.Empty;

            if (o != null)
            {
                if (o is IFormattable)
                    ret = (o as IFormattable).ToString(null, Culture);
                else
                    ret = o.ToString();
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given value is likely a GUID.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely a GUID.</returns>
        public static bool IsGuid(object o)
        {
            Guid parsed = Guid.Empty;

            return o != null && (o is Guid || Guid.TryParse(o.ToString(), out parsed));
        }

        /// <summary>
        /// Returns the Id representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>The Id representation of the given object.</returns>
        public static Guid GetGuid(object o)
        {
            Guid ret = Guid.Empty;

            if (o != null)
            {
                if (o is Guid)
                    ret = (Guid)o;
                else
                    Guid.TryParse(Input.GetString(o), out ret);
            }

            return ret;
        }

        /// <summary>
        /// Returns value indicating whether the given value is likely an enumeration.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>Value indicating whether the given value is likely an enumeration.</returns>
        public static bool IsEnum(object o)
        {
            return o != null && o is Enum;
        }

        /// <summary>
        /// Returns the culture representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>The culture representation of the given object.</returns>
        public static System.Globalization.CultureInfo GetCulture(object o)
        {
            System.Globalization.CultureInfo ret = null;

            if (o != null)
            {
                if (o is System.Globalization.CultureInfo)
                    ret = o as System.Globalization.CultureInfo;
                else if (o is string)
                {
                    try
                    {
                        ret = System.Globalization.CultureInfo.CreateSpecificCulture(o.ToString());
                    }
                    catch (System.ArgumentException) { }
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the enumeration representation of the given object.
        /// </summary>
        /// <param name="o">Object to examine.</param>
        /// <returns>The enumeration representation of the given object.</returns>
        public static T GetEnum<T>(object o) where T : struct
        {
            T ret = default(T);

            if (o != null)
            {
                if (o is T)
                    ret = (T)o;
                else
                    Enum.TryParse<T>(o.ToString(), true, out ret);
            }

            return ret;
        }
    }
}