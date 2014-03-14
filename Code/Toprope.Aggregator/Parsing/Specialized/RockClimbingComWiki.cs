using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Toprope.Aggregator.Parsing.Specialized
{
    /// <summary>
    /// Represents a wiki page information on rockclimbing.com.
    /// </summary>
    public class RockClimbingComWiki
    {
        #region Properties

        /// <summary>
        /// Gets the location culture.
        /// </summary>
        private static readonly CultureInfo LocationCulture = new CultureInfo("en-US");

        /// <summary>
        /// Gets or sets page name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets page description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the geographical coordinates specified on a page.
        /// </summary>
        public Toprope.Models.Location Location { get; set; }

        /// <summary>
        /// Gets or sets the best climbing seasons.
        /// </summary>
        public Models.Seasons Season { get; set; }

        /// <summary>
        /// Gets or sets the climbing types.
        /// </summary>
        public Models.ClimbingTypes Climbing { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RockClimbingComWiki()
        {
            Season = Models.Seasons.NotSpecified;
            Climbing = Models.ClimbingTypes.NotSpecified;
        }

        #region Static methods

        /// <summary>
        /// Fixes the punctuation of a given content.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>Content with fixed punctiation.</returns>
        public static string FixPunctiation(string content)
        {
            string ret = content;
            List<char> chars = new List<char>();

            ret = (ret ?? string.Empty).Trim();

            if (string.Compare(ret, "[]", StringComparison.InvariantCultureIgnoreCase) == 0)
                ret = string.Empty;
            else
            {
                if (!ret.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ret.Length > 1 && ret.StartsWith("(") && !ret.EndsWith(")") && !ret.EndsWith(").") && !ret.EndsWith(")!") && !ret.EndsWith(")?"))
                        ret = ret.Substring(1).Trim();

                    ret = Regex.Replace(ret, "&#[a-zA-Z0-9]+;", string.Empty);
                    ret = ret.Trim('-', '*', '+').Trim();

                    if (ret.Length < 3)
                        ret = string.Empty;
                    else
                    {
                        if (!string.IsNullOrEmpty(ret) && !char.IsPunctuation(ret[ret.Length - 1]))
                            ret = string.Concat(ret, ".");

                        if (!string.IsNullOrEmpty(ret))
                        {
                            if (char.IsLetter(ret[0]) && !char.IsUpper(ret[0]))
                            {
                                chars = new List<char>(ret.ToCharArray());
                                chars[0] = char.ToUpperInvariant(chars[0]);

                                ret = new string(chars.ToArray());
                            }

                            ret = Regex.Replace(Regex.Replace(Regex.Replace(ret, @"(\w)-(\w)", "$1 - $2"), @"(\w)-\s+", "$1 - "), @"\s+-(\w)", " - $1");
                            ret = Regex.Replace(Regex.Replace(Regex.Replace(ret, @"(\w),(\w)", "$1, $2"), @"\s+,\s", ", "), @",\s+", ", ");
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Normalizes name.
        /// </summary>
        /// <param name="name">Name to normalize.</param>
        /// <param name="capitalizeWords">Value indicating whether to capitalize words.</param>
        /// <returns>Normalized name.</returns>
        public static string NormalizeName(string name, bool capitalizeWords)
        {
            Match m = null;
            string[] names = null;
            string ret = name ?? string.Empty;
            string[] uncapitalizableNames = new string[] { "an", "the", "and", "but", "or", "of", "nor", "for", "yet", "so", "as", "la", "del", "de" };
            
            Func<string, string> startWithCapital = (str) =>
                {
                    string result = str;
                    List<char> chars = new List<char>();

                    if (!string.IsNullOrEmpty(result) && !char.IsUpper(result[0]))
                    {
                        chars = new List<char>(result.ToCharArray());
                        chars[0] = char.ToUpperInvariant(chars[0]);

                        result = new string(chars.ToArray());
                    }

                    return result;
                };
            
            if (!string.IsNullOrEmpty(ret))
            {
                ret = Regex.Replace(ret, "&#[a-zA-Z0-9]+;", string.Empty);
                ret = Regex.Replace(ret, "^(\\s|\\.|-|\\*)*[0-9]+\\s*(\\.|-|\\*)+", string.Empty);
                ret = ret.Trim().Trim('.', '!', '?', ',', '-', '*', ':', ';').Trim();
                ret = Regex.Replace(Regex.Replace(ret, "(\\w)\\(", "$1 (", RegexOptions.IgnoreCase), "\\)(\\w)", ") $1", RegexOptions.IgnoreCase);

                if (!ret.Any(c => char.IsLetter(c)))
                    ret = string.Empty;
                else
                {
                    if (capitalizeWords)
                    {
                        if (ret.IndexOf(' ') > 0)
                        {
                            names = ret.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            ret = string.Join(" ", names.Select(n => n.Length > 3 || uncapitalizableNames.FirstOrDefault(un =>
                                string.Compare(un, n, true) == 0) == null ? startWithCapital(n) : n));
                        }
                    }

                    ret = startWithCapital(ret);
                }

                m = Regex.Match(ret, @"(0)?[0-9]\s+", RegexOptions.IgnoreCase);
                if (m != null && m.Success && m.Index == 0)
                    ret = ret.Substring(m.Length).TrimStart();
            }

            return ret;
        }

        /// <summary>
        /// Parses page from the given content.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>Page.</returns>
        public static RockClimbingComWiki Parse(string content)
        {
            Match m = null;
            double parsed = -1;
            string[] seasons = null;
            string[] climbingTypes = null;
            RockClimbingComWiki ret = null;
            bool hasGeneralDescription = false;
            StringBuilder description = new StringBuilder();
            Models.Seasons season = Models.Seasons.NotSpecified;
            Models.ClimbingTypes climbing = Models.ClimbingTypes.NotSpecified;
            
            Func<string, string> getFieldValue = (name) =>
                {
                    string result = string.Empty;

                    m = Regex.Match(content, string.Format("{0}:\\s*</strong>\\s*</td>\\s*<td[^>]+>([^<]+)</td>", name), RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (m != null && m.Success)
                        result = FixPunctiation(m.Groups[1].Value.Trim());

                    return result;
                };

            Action<string> appendToDescription = (name) =>
                {
                    string nearest = string.Empty;
                    string v = getFieldValue(name);
                    string approachTime = string.Empty;

                    if (string.Compare(name, "Directions", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        nearest = getFieldValue("Nearest town or city");
                     
                        if (!string.IsNullOrEmpty(nearest))
                            v = string.Concat(string.Format("The nearest town or city is {0}. ", nearest.Trim().TrimEnd('.')), v);
                    }
                    else if (string.Compare(name, "Approach", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        approachTime = getFieldValue("Approach Time");

                        if (!string.IsNullOrEmpty(approachTime))
                        {
                            approachTime = Regex.Replace(Regex.Replace(approachTime, @"([0-9]+)", " $1 ", RegexOptions.IgnoreCase).Trim(), @"\s+", " ").Trim();

                            if (Regex.IsMatch(approachTime, "[a-zA-Z]+", RegexOptions.IgnoreCase))
                                v = string.Concat(string.Format("The approach time is {0}. ", approachTime.Trim().TrimEnd('.').Trim()), v);
                        }
                    }

                    if (!string.IsNullOrEmpty(v))
                    {
                        v = v.Trim();

                        if (description.Length > 0)
                            description.AppendLine();

                        if (hasGeneralDescription)
                            description.AppendLine(string.Format("### {0}", name));

                        description.Append(v);
                        hasGeneralDescription = v.Length > 0;
                    }
                };

            m = Regex.Match(content, "<h2>([^>]+)</h2>", RegexOptions.IgnoreCase);

            if (m != null && m.Success)
            {
                ret = new RockClimbingComWiki();

                ret.Name = NormalizeName(m.Groups[1].Value.Trim(), true);

                if (!string.IsNullOrEmpty(ret.Name))
                {
                    m = Regex.Match(content, "<td[^>]+>\\s*<!--[^>]+Description[^>]+-->([^<]+)</td>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (m != null && m.Success)
                    {
                        description.Append(FixPunctiation(m.Groups[1].Value.Trim()));
                        hasGeneralDescription = description.Length > 0;
                    }

                    appendToDescription("Directions");
                    appendToDescription("Approach");
                    appendToDescription("Access issues");

                    ret.Description = description.ToString();

                    m = Regex.Match(content, "<td[^>]+>(\\-?\\d+(\\.\\d+)?),\\s*(\\-?\\d+(\\.\\d+)?)</td>", RegexOptions.IgnoreCase);

                    if (m != null && m.Success)
                    {
                        ret.Location = new Models.Location();

                        if (double.TryParse(m.Groups[1].Value, NumberStyles.AllowLeadingSign, LocationCulture, out parsed) ||
                            double.TryParse(m.Groups[1].Value, NumberStyles.Any, LocationCulture, out parsed))
                        {
                            ret.Location.Latitude = parsed;
                        }

                        if (double.TryParse(m.Groups[3].Value, NumberStyles.AllowLeadingSign, LocationCulture, out parsed) ||
                            double.TryParse(m.Groups[3].Value, NumberStyles.Any, LocationCulture, out parsed))
                        {
                            ret.Location.Longitude = parsed;
                        }

                        if (ret.Location.Latitude < -90 || ret.Location.Latitude > 90)
                            ret.Location = null;
                    }

                    seasons = getFieldValue("When to Climb").Split(new char[] { ' ', '\n', '/' },
                        StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToArray();

                    if (seasons != null && seasons.Any())
                    {
                        foreach (string s in seasons)
                        {
                            if (Enum.TryParse<Models.Seasons>(s.Trim().Trim(',', '.', ';').Trim(), true, out season))
                            {
                                if (ret.Season == Models.Seasons.NotSpecified)
                                    ret.Season = season;
                                else
                                    ret.Season |= season;
                            }
                        }
                    }

                    climbingTypes = getFieldValue("Type of Climbing").Split(new char[] { ' ', '\n', '/' },
                        StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToArray();

                    if (climbingTypes != null && climbingTypes.Any())
                    {
                        foreach (string c in climbingTypes)
                        {
                            if (Enum.TryParse<Models.ClimbingTypes>(c.Trim().Trim(',', '.', ';').Trim(), true, out climbing))
                            {
                                if (ret.Climbing == Models.ClimbingTypes.NotSpecified)
                                    ret.Climbing = climbing;
                                else
                                    ret.Climbing |= climbing;
                            }
                        }
                    }

                    if (ret.Climbing == Models.ClimbingTypes.NotSpecified)
                        ret.Climbing = Models.ClimbingTypes.Sport;
                }
                else
                    ret = null;
            }

            return ret;
        }

        #endregion
    }
}
