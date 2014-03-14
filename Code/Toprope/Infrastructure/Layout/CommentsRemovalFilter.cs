using System;
using System.Text.RegularExpressions;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Allows removing all HTML comments from the resulting HTML.
    /// </summary>
    public class CommentsRemovalFilter : ResponseFilter
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

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public CommentsRemovalFilter() { }

        /// <summary>
        /// Filters the data from one stream and writes the filtered data into another stream.
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="outputStream">Output stream.</param>
        public override void Filter(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            string contents = string.Empty;
            Regex commentRx = new Regex("<!--([^>]+)-->", RegexOptions.Compiled | RegexOptions.Multiline);

            if (inputStream != null && outputStream != null)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(inputStream))
                    contents = reader.ReadToEnd();

                contents = commentRx.Replace(contents, string.Empty);

                outputStream.Write(System.Text.Encoding.UTF8.GetBytes(contents), 0, System.Text.Encoding.UTF8.GetByteCount(contents));
            }
        }
    }
}