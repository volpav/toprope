using System.Collections.Generic;

namespace Toprope.Aggregator.Parsing
{
    /// <summary>
    /// Represents a parsed sector.
    /// </summary>
    public class ParsedSector : Toprope.Models.Sector
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of routes.
        /// </summary>
        public IList<ParsedRoute> Routes { get; set; }

        /// <summary>
        /// Gets or sets the image associated with this sector.
        /// </summary>
        public byte[] Image { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ParsedSector()
        {
            Routes = new List<ParsedRoute>();
        }
    }
}
