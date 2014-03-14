using System.Collections.Generic;

namespace Toprope.Infrastructure.Search
{
    /// <summary>
    /// Represents a basic search result.
    /// </summary>
    public class SearchResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the total amount of search results available.
        /// </summary>
        public long Total { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchResult() { }
    }

    /// <summary>
    /// Represents a search result.
    /// </summary>
    public class SearchResult<T> : SearchResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the item that have been found.
        /// </summary>
        public IList<T> Items { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchResult()
        {
            Items = new List<T>();
        }
    }
}