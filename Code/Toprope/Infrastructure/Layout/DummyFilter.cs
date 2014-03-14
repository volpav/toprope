using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Represents a filter that does nothing with the content.
    /// </summary>
    public class DummyFilter : ResponseFilter
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public DummyFilter() { }

        /// <summary>
        /// Filters the data from one stream and writes the filtered data into another stream.
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="outputStream">Output stream.</param>
        public override void Filter(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            int bytesRead = 0;
            byte[] buffer = null;
            const int bufferSize = 1000;

            if (inputStream != null && outputStream != null)
            {
                do
                {
                    if (inputStream.CanRead)
                    {
                        buffer = new byte[bufferSize];
                        bytesRead = inputStream.Read(buffer, 0, bufferSize);

                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }
                while (bytesRead == bufferSize);
            }
        }
    }
}