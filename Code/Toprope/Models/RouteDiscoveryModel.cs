using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a discovery item.
    /// </summary>
    public class DiscoveryItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the search query.
        /// </summary>
        public string SearchQuery { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public DiscoveryItem() : this(string.Empty, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <param name="searchQuery">Search query.</param>
        public DiscoveryItem(string name, string searchQuery)
        {
            this.Name = name;
            this.SearchQuery = searchQuery;
        }
    }

    /// <summary>
    /// Represents a route discovery model.
    /// </summary>
    public class RouteDiscoveryModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the total number of routes.
        /// </summary>
        public long TotalRoutes { get; set; }

        /// <summary>
        /// Gets or sets the list of countries.
        /// </summary>
        public List<DiscoveryItem> Countries { get; set; }

        /// <summary>
        /// Gets or sets the list of areas.
        /// </summary>
        public List<DiscoveryItem> Areas { get; set; }

        /// <summary>
        /// Gets or sets the list of "Now searching" items.
        /// </summary>
        public List<DiscoveryItem> NowSearching { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RouteDiscoveryModel()
        {
            Countries = new List<DiscoveryItem>();
            Areas = new List<DiscoveryItem>();
            NowSearching = new List<DiscoveryItem>();
        }
    }
}