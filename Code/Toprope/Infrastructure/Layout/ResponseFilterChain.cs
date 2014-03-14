using System;
using System.IO;
using System.Web;
using System.Linq;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Represents a response filter chaing. This class cannot be inherited.
    /// </summary>
    public sealed class ResponseFilterChain : MemoryStream
    {
        #region Properties

        private System.IO.Stream _outputStream = null;

        /// <summary>
        /// Gets or sets the current HTTP context.
        /// </summary>
        private HttpContext Context { get; set; }

        /// <summary>
        /// Gets or sets the collection of filters.
        /// </summary>
        private System.Collections.Generic.List<ResponseFilter> Filters { get; set; }

        /// <summary>
        /// Gets or sets the data to be written.
        /// </summary>
        private System.Collections.Generic.List<byte> Data { get; set; }

        /// <summary>
        /// Returns the total number of filters within the chain.
        /// </summary>
        public int Count { get { return Filters.Count; } }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        public System.IO.Stream OutputStream
        {
            get { return _outputStream != null ? _outputStream : Context.Response.OutputStream; }
        }

        /// <summary>
        /// Gets the current request.
        /// </summary>
        public HttpRequest Request
        {
            get { return Context != null ? Context.Request : null; }
        }

        /// <summary>
        /// Gets the current response.
        /// </summary>
        public HttpResponse Response
        {
            get { return Context != null ? Context.Response : null; }
        }

        /// <summary>
        /// Gets the server information.
        /// </summary>
        public HttpServerUtility Server
        {
            get { return Context != null ? Context.Server : null; }
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public System.Web.SessionState.HttpSessionState Session
        {
            get { return Context != null ? Context.Session : null; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="context">Current HTTP context.</param>
        /// <param name="outputStream">Output stream.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context">context</paramref> is null.</exception>
        public ResponseFilterChain(HttpContext context, System.IO.Stream outputStream) 
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else
            {
                Context = context;
                _outputStream = outputStream;
                Filters = new System.Collections.Generic.List<ResponseFilter>();

                AddDefaultFilters();
            }
        }

        /// <summary>
        /// Adds new filter to the chain.
        /// </summary>
        /// <param name="filter">Filter to add.</param>
        public void Add(ResponseFilter filter)
        {
            if (filter != null && !Contains(filter))
            {
                filter.AddToChain(this);
                Filters.Add(filter);
            }
        }

        /// <summary>
        /// Removes existing filter from the chain.
        /// </summary>
        /// <param name="filter">Filter to remove.</param>
        public void Remove(ResponseFilter filter)
        {
            int index = 0;

            if (filter != null && Contains(filter))
            {
                index = Filters.FindIndex(f => f.CompareTo(filter) == 0);
                if (index >= 0 && index < Filters.Count)
                {
                    Filters[index].RemoveFromChain();
                    Filters.Remove(filter);
                }
            }
        }

        /// <summary>
        /// Determines whether chain contains the given filter.
        /// </summary>
        /// <param name="filter">Filter to search for.</param>
        /// <returns>Value indicating whether chain contains the given filter.</returns>
        public bool Contains(ResponseFilter filter)
        {
            return Filters.Contains(filter);
        }

        /// <summary>
        /// Removes all filters from the chain.
        /// </summary>
        public void Clear()
        {
            Filters.Clear();
        }

        /// <summary>
        /// Writes a block of bytes to the current stream using data read from buffer.
        /// </summary>
        /// <param name="buffer">Data to write.</param>
        /// <param name="offset">Data offset.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            bool initialize = false;
            byte[] bufferToWrite = null;

            if (buffer != null && offset >= 0 && offset < buffer.Length)
            {
                if (offset + count > buffer.Length)
                    count = buffer.Length - offset - 1;

                bufferToWrite = (byte[])Array.CreateInstance(typeof(byte), count);
                Array.Copy(buffer, offset, bufferToWrite, 0, count);
                
                if (Context.Response != null && OutputStream != null)
                {
                    if (Data == null)
                    {
                        initialize = true;
                        Data = new System.Collections.Generic.List<byte>();
                    }

                    Data.AddRange(bufferToWrite);

                    if (initialize)
                    {
                        for (int i = 0; i < Filters.Count; i++)
                            Filters[i].Initialize();
                    }
                }
            }
        }

        /// <summary>
        /// Flushes all the buffered data.
        /// </summary>
        public override void Flush()
        {
            byte[] copyData = null;
            Stream inputStream = null, outputStream = null;

            if (Data != null && Data.Count > 0)
            {
                if (Filters.Any(f => f.CanFilter))
                {
                    for (int i = 0; i < Filters.Count; i++)
                    {
                        if (i == 0)
                            inputStream = new MemoryStream(Data.ToArray());
                        else
                        {
                            if (outputStream.CanSeek)
                                outputStream.Seek(0, SeekOrigin.Begin);

                            using (StreamReader reader = new StreamReader(outputStream))
                                copyData = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());

                            inputStream = new MemoryStream(copyData);
                        }

                        if (i < Filters.Count)
                        {
                            if (i == (Filters.Count - 1))
                                outputStream = OutputStream;
                            else
                                outputStream = new MemoryStream();
                        }

                        if (Filters[i].CanFilter)
                        {
                            if (inputStream.CanSeek)
                                inputStream.Seek(0, SeekOrigin.Begin);

                            if (outputStream.CanSeek)
                                outputStream.Seek(0, SeekOrigin.Begin);

                            Filters[i].Filter(inputStream, outputStream);
                        }
                        else
                            new DummyFilter().Filter(inputStream, outputStream);
                    }
                }
                else
                    new DummyFilter().Filter(new MemoryStream(Data.ToArray()), this.OutputStream);
            }

            Data = null;
            copyData = null;

            base.Flush();
        }

        /// <summary>
        /// Adds default filters to the chain.
        /// </summary>
        private void AddDefaultFilters()
        {
            Add(new WhitespaceStripFilter());
            Add(new CommentsRemovalFilter());
            Add(new CompressionFilter());
        }

        #region Static methods

        /// <summary>
        /// Gets value indicating whether the response that corresponds to the given request can be chained.
        /// </summary>
        /// <param name="request">HTTP request.</param>
        /// <returns>value indicating whether filter chain can be applied to the corresponding response.</returns>
        public static bool CanApply(HttpRequest request)
        {
            return request != null && !request.Url.AbsolutePath.EndsWith(".ttf", StringComparison.InvariantCultureIgnoreCase) && request.AcceptTypes != null &&
                request.AcceptTypes.Any(t => t.IndexOf("text/html", StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        #endregion
    }
}