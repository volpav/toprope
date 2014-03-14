using System.Collections.Generic;

namespace Toprope.Models.Search
{
    /// <summary>
    /// Represents a search result group.
    /// </summary>
    public class SearchResultGroup : SearchResultBase
    {
        #region Properties

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
        /// <param name="id">Grou Id.</param>
        /// <param name="name">Group name.</param>
        /// <param name="query">Current search query.</param>
        public SearchResultGroup(string id, string name, Infrastructure.Search.SearchQuery query)
        {
            this.Name = name;
            this.Excerpt = string.Empty;
            this.Url = url => { return url.Action("Index", "Home", new { q = query.WithoutCriterion("skip").WithoutCriterion("take").WithCriterion("in", id).ToString() }); };

            this.Results = new List<SearchResultBase>();
        }
    }
}