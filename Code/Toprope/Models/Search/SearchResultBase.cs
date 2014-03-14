using System;

namespace Toprope.Models.Search
{
    /// <summary>
    /// Represents a base class for search result.
    /// </summary>
    public abstract class SearchResultBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the search result name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the search result excerpt text.
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// Gets or sets the URL builder.
        /// </summary>
        public Func<System.Web.Mvc.UrlHelper, string> Url { get; set; } 

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected SearchResultBase() { }
    }
}