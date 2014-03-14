namespace Toprope.Aggregator.Parsing
{
    /// <summary>
    /// Represents a parser settings.
    /// </summary>
    public class ParserSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the maximum amount of results to return.
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Gets or sets the amount of areas to parse before notifying the caller.
        /// </summary>
        public int ChunkSize { get; set; }

        /// <summary>
        /// Gets or sets the collection of extended settings.
        /// </summary>
        public System.Collections.Specialized.NameValueCollection ExtendedSettings { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ParserSettings()
        {
            ChunkSize = 10;
            MaxResults = int.MaxValue;
            ExtendedSettings = new System.Collections.Specialized.NameValueCollection();
        }
    }
}
