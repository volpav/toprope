using System;
using System.Text.RegularExpressions;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Allows removing all whitespaces from the resulting HTML.
    /// </summary>
    public class WhitespaceStripFilter : ResponseFilter
    {
        #region Properties

        /// <summary>
        /// Gets value indicating whether filter can execute.
        /// </summary>
        public override bool CanFilter
        {
            get
            {
                bool ret = base.CanFilter;

                if (ret && Response != null)
                    ret = string.Compare(Infrastructure.Utilities.Input.GetString(Response.ContentType), "text/html", StringComparison.InvariantCultureIgnoreCase) == 0;

                return ret;
            }
        }

        #endregion

        #region Patterns

        private static readonly string PrePlaceholderFormat = "#pre{0}#";

        private static readonly Regex Tab = new Regex("\t", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex MultiSpace = new Regex("\\s{2,}", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex SpaceBetweenTags = new Regex(">\\s<", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex CarriageReturn = new Regex(">\r\n<", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex CarriageReturnSafe = new Regex("\r\n", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex Pre = new Regex("<pre[^>]*>[^<]+</pre>", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex PrePlaceholder = new Regex("#pre([0-9]+)#", RegexOptions.Compiled | RegexOptions.Multiline);

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public WhitespaceStripFilter() { }

        /// <summary>
        /// Filters the data from one stream and writes the filtered data into another stream.
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="outputStream">Output stream.</param>
        public override void Filter(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            string contents = string.Empty;
            System.Collections.Generic.List<string> pre = new System.Collections.Generic.List<string>();
            
            if (inputStream != null && outputStream != null)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(inputStream))
                    contents = reader.ReadToEnd();

                contents = Pre.Replace(contents, (m) =>
                    {
                        string result = string.Empty;

                        if (m != null && m.Success)
                        {
                            result = string.Format(PrePlaceholderFormat, pre.Count);
                            pre.Add(m.Value);
                        }

                        return result;
                    });

                if (pre.Count > 0)
                {
                    for (int index = 0; index < pre.Count; index++)
                    {
                        pre[index] = pre[index].Replace("&lt;", "<");
                        pre[index] = pre[index].Replace("&gt;", ">");
                    }
                }

                contents = Tab.Replace(contents, string.Empty);
                contents = CarriageReturn.Replace(contents, "><");
                contents = CarriageReturnSafe.Replace(contents, " ");

                while (MultiSpace.IsMatch(contents))
                    contents = MultiSpace.Replace(contents, " ");

                contents = SpaceBetweenTags.Replace(contents, "><");

                contents = PrePlaceholder.Replace(contents, (m) =>
                    {
                        int index = -1;
                        string result = string.Empty;

                        if (m != null && m.Success)
                        {
                            result = m.Value;
                            if (m.Groups.Count > 1 && int.TryParse(m.Groups[1].Value, out index))
                            {
                                if (index >= 0 && index < pre.Count)
                                    result = pre[index];
                            }
                        }

                        return result;
                    });

                outputStream.Write(System.Text.Encoding.UTF8.GetBytes(contents), 0, System.Text.Encoding.UTF8.GetByteCount(contents));
            }
        }
    }
}