using System.Collections.Generic;

namespace Toprope.Aggregator.Parsing.Specialized
{
    /// <summary>
    /// Represents a state for RockClimbing.com parser.
    /// </summary>
    public class RockClimbingComState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        public IList<RockClimbingComListItem> Regions { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the current region.
        /// </summary>
        public int RegionIndex { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        public IDictionary<string, IList<RockClimbingComListItem>> Countries { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the current country.
        /// </summary>
        public int CountryIndex { get; set; }

        /// <summary>
        /// Gets or sets the country regions.
        /// </summary>
        public IDictionary<string, IList<RockClimbingComListItem>> CountryRegions { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the current country region.
        /// </summary>
        public int CountryRegionIndex { get; set; }

        /// <summary>
        /// Gets or sets the areas.
        /// </summary>
        public IDictionary<string, IList<RockClimbingComListItem>> Areas { get; set; }

        /// <summary>
        /// Gets or sets the zero-based index of the current area.
        /// </summary>
        public int AreaIndex { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RockClimbingComState()
        {
            Regions = new List<RockClimbingComListItem>();
            RegionIndex = 0;

            Countries = new Dictionary<string, IList<RockClimbingComListItem>>();
            CountryIndex = 0;

            CountryRegions = new Dictionary<string, IList<RockClimbingComListItem>>();
            CountryRegionIndex = 0;

            Areas = new Dictionary<string, IList<RockClimbingComListItem>>();
            AreaIndex = 0;
        }
    }
}
