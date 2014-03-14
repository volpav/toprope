using System.Linq;

namespace Toprope.Aggregator.Parsing
{
    /// <summary>
    /// Represents a parser.
    /// </summary>
    public abstract class Parser
    {
        #region Events

        /// <summary>
        /// Occurs when next block of areas was parsed.
        /// </summary>
        public event System.EventHandler<ParserResultEventArgs> Parsing;

        /// <summary>
        /// Occurs when the trace is received from the parser.
        /// </summary>
        public event System.EventHandler<ParserTraceEventArgs> TraceReceived;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cached pages.
        /// </summary>
        private System.Collections.Generic.IDictionary<string, string> CachedPages { get; set; }

        /// <summary>
        /// Gets or sets the URLs of last cached pages.
        /// </summary>
        private System.Collections.Generic.List<string> CachedPageUrls { get; set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public ParserSettings Settings { get; private set; }

        /// <summary>
        /// Gets the base URL of this parser.
        /// </summary>
        public abstract string BaseUrl { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected Parser()
        {
            Settings = new ParserSettings();
            CachedPages = new System.Collections.Generic.Dictionary<string, string>();
            CachedPageUrls = new System.Collections.Generic.List<string>();
        }

        /// <summary>
        /// Loads the given resource.
        /// </summary>
        /// <param name="relativeUrl">Relative URL.</param>
        /// <returns>Loaded resource content.</returns>
        protected string Load(string relativeUrl)
        {
            string[] range = null;
            string ret = string.Empty;
            string fullUrl = string.Empty;
            int cacheSize = 100, trimSize = 30;

            using (var client = new System.Net.WebClient())
            {
                fullUrl = relativeUrl.IndexOf("://") >= 0 ? relativeUrl : string.Format("{0}/{1}", BaseUrl.TrimEnd('/'), relativeUrl.TrimStart('/'));

                if (CachedPages.ContainsKey(fullUrl))
                    ret = CachedPages[fullUrl];
                else
                {
                    ret = client.DownloadString(fullUrl);

                    // Cache the last [cacheSize] pages
                    if (CachedPageUrls.Count > cacheSize)
                    {
                        range = CachedPageUrls.Take(trimSize).ToArray();
                        foreach (string url in range)
                        {
                            if (CachedPages.ContainsKey(url))
                                CachedPages.Remove(url);
                        }

                        CachedPageUrls.RemoveRange(0, trimSize);
                    }

                    CachedPages.Add(fullUrl, ret);
                    CachedPageUrls.Add(fullUrl);
                }
            }
                
            return ret;
        }

        /// <summary>
        /// Loads the given resource as binary.
        /// </summary>
        /// <param name="relativeUrl">Relative URL.</param>
        /// <returns>Loaded resource content.</returns>
        protected byte[] LoadBinary(string relativeUrl)
        {
            byte[] ret = null;

            using (var client = new System.Net.WebClient())
                ret = client.DownloadData(relativeUrl.IndexOf("://") >= 0 ? relativeUrl : string.Format("{0}/{1}", BaseUrl.TrimEnd('/'), relativeUrl.TrimStart('/')));

            return ret;
        }

        /// <summary>
        /// Raises "Parsing" event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnParsing(ParserResultEventArgs e)
        {
            if (Parsing != null)
                Parsing(this, e);
        }

        /// <summary>
        /// Raises "TraceReceived" event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnTraceReceived(ParserTraceEventArgs e)
        {
            if (TraceReceived != null)
                TraceReceived(this, e);
        }

        /// <summary>
        /// Begin parsing items.
        /// </summary>
        public abstract void BeginParse();

        /// <summary>
        /// Writes the trace message to the screen.
        /// </summary>
        /// <param name="message">Message.</param>
        protected void Trace(string message)
        {
            Trace(message, null);
        }

        /// <summary>
        /// Writes the trace message to the screen.
        /// </summary>
        /// <param name="format">Message format.</param>
        /// <param name="args">Arguments.</param>
        protected void Trace(string format, params object[] args)
        {
            string msg = args != null ? string.Format(format, args) : format;

            OnTraceReceived(new ParserTraceEventArgs(msg));
        }

        #region Static methods

        /// <summary>
        /// Creates a parser.
        /// </summary>
        /// <param name="alias">Alias.</param>
        /// <returns>Parser.</returns>
        public static Parser Create(string alias)
        {
            Parser ret = null;

            if (!string.IsNullOrEmpty(alias))
            {
                if (string.Compare(alias, "rockclimbing.com", System.StringComparison.InvariantCultureIgnoreCase) == 0)
                    ret = new Specialized.RockClimbingComParser();
            }

            return ret;
        }

        #endregion
    }
}
