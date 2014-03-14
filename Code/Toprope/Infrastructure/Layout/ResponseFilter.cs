using System;
using System.IO;
using System.Web;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Represents a compression algorithm.
    /// </summary>
    public enum ContentCompressionAlgorithm
    {
        /// <summary>
        /// No compression.
        /// </summary>
        None = 0,

        /// <summary>
        /// Apply Gzip compression.
        /// </summary>
        Gzip = 1,

        /// <summary>
        /// Apply Deflate compression.
        /// </summary>
        Deflate = 2
    }

    /// <summary>
    /// Represents a response filter.
    /// </summary>
    public abstract class ResponseFilter : MemoryStream, IComparable<ResponseFilter>, IComparable
    {
        #region Properties

        /// <summary>
        /// Gets value indicating whether filter can execute.
        /// </summary>
        public virtual bool CanFilter { get { return true; } }

        /// <summary>
        /// Gets the current request.
        /// </summary>
        protected HttpRequest Request
        {
            get { return Chain != null ? Chain.Request : null; }
        }

        /// <summary>
        /// Gets the current response.
        /// </summary>
        protected HttpResponse Response
        {
            get { return Chain != null ? Chain.Response : null; }
        }

        /// <summary>
        /// Gets the server information.
        /// </summary>
        protected HttpServerUtility Server
        {
            get { return Chain != null ? Chain.Server : null; }
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        protected System.Web.SessionState.HttpSessionState Session
        {
            get { return Chain != null ? Chain.Session : null; }
        }

        /// <summary>
        /// Gets the owning chain.
        /// </summary>
        protected ResponseFilterChain Chain { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected ResponseFilter() { }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Adds filter to the given chain.
        /// </summary>
        /// <param name="chain">Owning chain.</param>
        public virtual void AddToChain(ResponseFilterChain chain)
        {
            this.Chain = chain;
        }

        /// <summary>
        /// Removes the filter from the current chain.
        /// </summary>
        public virtual void RemoveFromChain()
        {
            this.Chain = null;
        }

        /// <summary>
        /// Writes a block of bytes to the current stream using data read from buffer.
        /// </summary>
        /// <param name="buffer">Data to write.</param>
        /// <param name="offset">Data offset.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] bufferToWrite = null;

            if (CanFilter)
            {
                if (buffer != null && offset >= 0 && offset < buffer.Length)
                {
                    if (offset + count >= buffer.Length)
                        count = buffer.Length - offset - 1;

                    bufferToWrite = (byte[])Array.CreateInstance(typeof(byte), count);
                    Array.Copy(buffer, offset, bufferToWrite, 0, count);

                    if (Response != null && Response.OutputStream != null)
                    {
                        using(MemoryStream s = new MemoryStream(bufferToWrite))
                            Filter(s, Response.OutputStream);
                    }
                }
            }
        }

        /// <summary>
        /// Filters the data from one stream and writes the filtered data into another stream.
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="outputStream">Output stream.</param>
        public abstract void Filter(Stream inputStream, Stream outputStream);

        /// <summary>
        /// Compares the current filter to the given one.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>Comparison result.</returns>
        int IComparable.CompareTo(object obj)
        {
            int ret = -1;

            if (obj != null)
            {
                if (Type.ReferenceEquals(this, obj))
                    ret = 0;
                else if (obj is ResponseFilter)
                    ret = CompareTo(obj as ResponseFilter);
            }

            return ret;
        }

        /// <summary>
        /// Compares the current filter to the given one.
        /// </summary>
        /// <param name="other">Filter to compare to.</param>
        /// <returns>Comparion result.</returns>
        public int CompareTo(ResponseFilter other)
        {
            int ret = -1;

            if (other != null)
            {
                if (Type.ReferenceEquals(this, other))
                    ret = 0;
                else
                    ret = string.Compare(this.GetType().FullName, other.GetType().FullName, StringComparison.InvariantCultureIgnoreCase);
            }

            return ret;
        }

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}