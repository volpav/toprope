using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Toprope.Infrastructure.Utilities
{
    /// <summary>
    /// Provides methods and properties for manipulating with rich text.
    /// </summary>
    public static class RichText
    {
        /// <summary>
        /// Limits the length of a given string to 125 characters.
        /// </summary>
        /// <param name="input">Input string to process.</param>
        /// <returns>Limited string.</returns>
        public static IHtmlString Truncate(string input)
        {
            return Truncate(input, 125);
        }

        /// <summary>
        /// Limits the length of a given string to a given number of characters.
        /// </summary>
        /// <param name="input">Input string to process.</param>
        /// <param name="maxChars">Maximum allowed number of characters.</param>
        /// <returns>Limited string.</returns>
        public static IHtmlString Truncate(string input, int maxChars)
        {
            string ret = Simplify(input).ToString();

            if (ret.Length > maxChars && maxChars > 3)
                ret = ret.Substring(0, maxChars - 3) + "...";

            return new MvcHtmlString(ret);
        }

        /// <summary>
        /// Enables rich text on a given input text.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Rich text.</returns>
        public static IHtmlString Enable(IHtmlString input)
        {
            return Enable(input != null ? input.ToString() : string.Empty);
        }

        /// <summary>
        /// Enables rich text on a given input text.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Rich text.</returns>
        public static IHtmlString Enable(string input)
        {
            Match headingMatch = null;
            string ret = string.Empty;
            Match orderedMatch = null;
            string line = string.Empty;
            Match unorderedMatch = null;
            string listTag = string.Empty;
            string emptyParagraph = "<p></p>";
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            System.Func<string, string> makeHtmlLine = (l) => { return string.Format("{0}<br />", line); };

            // Unordered/ordered lists and headings
            using (System.IO.StringReader reader = new System.IO.StringReader(HttpUtility.HtmlEncode(input ?? string.Empty)))
            {
                output.Append("<p>");

                while (true)
                {
                    line = reader.ReadLine();
                    
                    if (line == null)
                        break;
                    else
                    {
                        headingMatch = Regex.Match(line, @"^(#{2,6})\s(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        orderedMatch = Regex.Match(line, @"^[1-9]+\.\s{1,}(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        unorderedMatch = Regex.Match(line, @"^(?:-|\*)\s{1,}(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                        if (headingMatch.Success)
                            output.AppendFormat("<h{0}>{1}</h{0}>", headingMatch.Groups[1].Value.Length, headingMatch.Groups[2].Value);
                        else if (orderedMatch.Success || unorderedMatch.Success)
                        {
                            if (orderedMatch.Success)
                            {
                                if (string.IsNullOrEmpty(listTag))
                                    output.Append("</p><ol>");

                                listTag = "ol";
                            }
                            else if (unorderedMatch.Success)
                            {
                                if (string.IsNullOrEmpty(listTag))
                                    output.Append("</p><ul>");

                                listTag = "ul";
                            }

                            output.AppendFormat("<li>{0}</li>", orderedMatch.Success ? orderedMatch.Groups[1].Value : unorderedMatch.Groups[1].Value);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(listTag))
                            {
                                output.AppendFormat("</{0}><p>", listTag);
                                listTag = string.Empty;
                            }

                            output.Append(makeHtmlLine(line));
                        }
                    }
                }

                output.Append("</p>");
            }

            ret = output.ToString();

            // Italic
            ret = Regex.Replace(ret, @"_(\w)([^<_]+)_", "<i>$1$2</i>", RegexOptions.IgnoreCase);

            // Bold
            ret = Regex.Replace(ret, @"__(\w)([^<_]+)__", "<strong>$1$2</strong>", RegexOptions.IgnoreCase);

            ret = ret.Replace("<br /><br />", "</p><p>");

            ret = Regex.Replace(ret, "</h([2-6])><br />", "</h$1>", RegexOptions.IgnoreCase);
            ret = Regex.Replace(ret, "</ul><br />", "</ul>", RegexOptions.IgnoreCase);
            ret = Regex.Replace(ret, "</ol><br />", "</ol>", RegexOptions.IgnoreCase);
            ret = Regex.Replace(ret, "<br /><ul>", "<ul>", RegexOptions.IgnoreCase);
            ret = Regex.Replace(ret, "<br /><ol>", "<ol>", RegexOptions.IgnoreCase);

            ret = Regex.Replace(ret, "<h([2-6])>", "</p><h$1>", RegexOptions.IgnoreCase);
            ret = Regex.Replace(ret, "</h([2-6])>", "</h$1><p>", RegexOptions.IgnoreCase);
            ret = Regex.Replace(ret, "<br /></p>", "</p>", RegexOptions.IgnoreCase);

            // Shorten links and enable them (make clickable)
            ret = ReplaceLinks(ret, (url, text) =>
                {
                    string urlText = text;

                    if (urlText.Length > 50)
                        urlText = urlText.Substring(0, 50) + "...";

                    if (Infrastructure.Utilities.Input.IsEmail(url))
                        url = string.Format("mailto:{0}", url);

                    return string.Format("<a title=\"{0}\" href=\"{0}\">{1}</a>",
                        System.Web.HttpUtility.HtmlAttributeEncode(url), urlText);
                });

            if (ret.StartsWith(emptyParagraph, System.StringComparison.InvariantCultureIgnoreCase))
                ret = ret.Substring(emptyParagraph.Length);
            
            return new MvcHtmlString(ret);
        }

        /// <summary>
        /// Simplifies the rich text formatting.
        /// </summary>
        /// <param name="input">Input text.</param>
        /// <returns>Input text with simplified rich text formatting.</returns>
        public static IHtmlString Simplify(string input)
        {
            string ret = string.Empty;
            Match headingMatch = null;
            string line = string.Empty;
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            using (System.IO.StringReader reader = new System.IO.StringReader(HttpUtility.HtmlEncode(input ?? string.Empty)))
            {
                while (true)
                {
                    line = reader.ReadLine();

                    if (line == null)
                        break;
                    else
                    {
                        headingMatch = Regex.Match(line, @"^(#{2,6})\s(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                        if (headingMatch == null || !headingMatch.Success)
                            output.AppendLine(line);
                        else
                            output.AppendLine(" ... ");
                    }
                }
            }

            // Italic
            ret = Regex.Replace(ret, @"_(\w)([^<_]+)_", "$1$2", RegexOptions.IgnoreCase);

            // Bold
            ret = Regex.Replace(ret, @"__(\w)([^<_]+)__", "$1$2", RegexOptions.IgnoreCase);

            ret = output.ToString();

            // Shorten links but don't enable them (don't make clickable)
            ret = ReplaceLinks(ret, (url, text) =>
                {
                    string urlText = text;

                    if (urlText.Length > 50)
                        urlText = urlText.Substring(0, 50) + "...";

                    return urlText;
                });

            return new MvcHtmlString(ret);
        }

        /// <summary>
        /// Replaces links.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="replacer">Replacer.</param>
        /// <returns>Replaced links.</returns>
        private static string ReplaceLinks(string content, System.Func<string, string, string> replacer)
        {
            return Regex.Replace(content, @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)", (m) =>
            {
                int schemeIndex = -1;
                string scheme = "://";
                string url = string.Empty;
                string text = string.Empty;
                
                System.Tuple<string, string> link = GetLink(m.Value);

                url = text = link.Item1;

                schemeIndex = text.IndexOf(scheme);
                if (schemeIndex > 0)
                    text = text.Substring(schemeIndex + scheme.Length);

                return string.Concat(replacer(url, text), link.Item2);
            });
        }

        /// <summary>
        /// Returns link URL and the chrome around it (if exists).
        /// </summary>
        /// <param name="link">Link URL.</param>
        /// <returns>Link URL and the chrome around it (if exists).</returns>
        private static System.Tuple<string, string> GetLink(string link)
        {
            string url = link;
            int chromeChars = 0;
            string chrome = string.Empty;
            
            for (int i = link.Length - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(link[i]) || char.IsPunctuation(link[i]))
                    chromeChars += 1;
                else
                    break;
            }

            if (chromeChars > 0)
            {
                chrome = url.Substring(url.Length - chromeChars);
                url = url.Substring(0, url.Length - chromeChars);
            }

            return new System.Tuple<string, string>(url, chrome);
        }
    }
}