using System.Collections.Generic;

namespace Toprope.Models.Search
{
    /// <summary>
    /// Represents a search results container.
    /// </summary>
    public class SearchResultContainer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the search query.
        /// </summary>
        public Infrastructure.Search.SearchQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the custom title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the search results.
        /// </summary>
        public IList<SearchResultBase> Results { get; set; }

        /// <summary>
        /// Gets or sets the total number of search results available.
        /// </summary>
        public long Total { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchResultContainer()
        {
            Results = new List<SearchResultBase>();
        }
    }
}