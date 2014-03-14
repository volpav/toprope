using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Compression;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Represents a filter that compresses the output using either Gzip or Deflate compression.
    /// </summary>
    public class CompressionFilter : ResponseFilter
    {
        #region Properties

        private bool? _canFilter = null;
        private ContentCompressionAlgorithm? _compressionAlgorithm = null;

        /// <summary>
        /// Gets or sets compression algorithm.
        /// </summary>
        public ContentCompressionAlgorithm CompressionAlgorithm
        {
            get
            {
                string encoding = string.Empty;

                if (_compressionAlgorithm == null || !_compressionAlgorithm.HasValue)
                {
                    if (Request != null && Request.Headers != null)
                    {
                        encoding = Request.Headers["Accept-Encoding"];

                        if (!string.IsNullOrEmpty(encoding))
                        {
                            if (encoding.IndexOf("gzip", System.StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                                encoding.IndexOf("x-gzip", System.StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                                encoding.IndexOf("*", System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                _compressionAlgorithm = ContentCompressionAlgorithm.Gzip;
                            }
                            else if (encoding.IndexOf("deflate", System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                                _compressionAlgorithm = ContentCompressionAlgorithm.Deflate;
                        }
                    }

                    if (_compressionAlgorithm == null || !_compressionAlgorithm.HasValue)
                        _compressionAlgorithm = ContentCompressionAlgorithm.None;
                }

                return _compressionAlgorithm.Value;
            }
            set { _compressionAlgorithm = value; }
        }

        /// <summary>
        /// Gets value indicating whether filter can execute.
        /// </summary>
        public override bool CanFilter
        {
            get
            {
                bool allowed = false;
                bool isAjaxRequest = false;
                string encoding = string.Empty;
                string pathExtension = string.Empty;
                string[] prohibitExtensions = new string[] { "ashx", "asmx", "axd", "png", "gif", "jpg", "css", "js", "ico" };

                if (base.CanFilter && Response != null)
                {
                    allowed = true;

                    if (_canFilter == null || !_canFilter.HasValue)
                    {
                        isAjaxRequest = !string.IsNullOrEmpty(Request["HTTP_X_MICROSOFTAJAX"]) ||
                            string.Compare(Infrastructure.Utilities.Input.GetString(Request["HTTP_X_REQUESTED_WITH"]), "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                            string.Compare(Infrastructure.Utilities.Input.GetString(Request.Headers["X-Requested-With"]), "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                            string.Compare(Infrastructure.Utilities.Input.GetString(Request["HTTP_ACCEPT"]), "application/json", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                            string.Compare(Infrastructure.Utilities.Input.GetString(Request.Headers["Accept"]), "application/json", StringComparison.InvariantCultureIgnoreCase) == 0; ;

                        if (isAjaxRequest)
                            allowed = false;
                        else
                        {
                            pathExtension = Request.CurrentExecutionFilePathExtension;
                            if (!string.IsNullOrEmpty(pathExtension))
                            {
                                pathExtension = pathExtension.Trim(' ', '.').ToLowerInvariant();
                                for (int i = 0; i < prohibitExtensions.Length; i++)
                                {
                                    if (string.Compare(prohibitExtensions[i], pathExtension, StringComparison.InvariantCultureIgnoreCase) == 0)
                                    {
                                        allowed = false;
                                        break;
                                    }
                                }
                            }

                            if (allowed && Response != null)
                            {
                                encoding = Response.Headers["Content-Encoding"];

                                if (!string.IsNullOrEmpty(encoding))
                                {
                                    allowed = encoding.IndexOf("gzip", StringComparison.InvariantCultureIgnoreCase) < 0 &&
                                        encoding.IndexOf("deflate", StringComparison.InvariantCultureIgnoreCase) < 0;
                                }
                            }
                        }

                        _canFilter = allowed;
                    }

                    if (_canFilter == null || !_canFilter.HasValue)
                        _canFilter = true;
                }

                return _canFilter.Value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public CompressionFilter() { }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        public override void Initialize()
        {
            string encoding = string.Empty;

            base.Initialize();

            if (CanFilter && CompressionAlgorithm != ContentCompressionAlgorithm.None)
            {
                if (Response != null)
                {
                    encoding = Enum.GetName(typeof(ContentCompressionAlgorithm), CompressionAlgorithm).ToLowerInvariant();

                    Response.Headers.Remove("Content-Encoding");
                    Response.Headers.Add("Content-Encoding", encoding);
                }
            }
        }

        /// <summary>
        /// Filters the data from one stream and writes the filtered data into another stream.
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="outputStream">Output stream.</param>
        public override void Filter(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            Stream s = null;
            int bytesRead = 0;
            byte[] buffer = null;
            const int bufferSize = 10000;

            if (inputStream != null && outputStream != null)
            {
                if (CompressionAlgorithm == ContentCompressionAlgorithm.Gzip)
                    s = new GZipStream(outputStream, CompressionMode.Compress);
                else if (CompressionAlgorithm == ContentCompressionAlgorithm.Deflate)
                    s = new DeflateStream(outputStream, CompressionMode.Compress);

                if (s == null)
                    s = outputStream;

                using (s)
                {
                    do
                    {
                        if (inputStream.CanRead)
                        {
                            buffer = new byte[bufferSize];
                            bytesRead = inputStream.Read(buffer, 0, bufferSize);

                            s.Write(buffer, 0, bytesRead);
                        }
                    }
                    while (bytesRead == bufferSize);
                }
            }
        }
    }
}