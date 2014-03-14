using System.Collections.Generic;

namespace Toprope.Aggregator.Parsing
{
    /// <summary>
    /// Represents an area.
    /// </summary>
    public class ParsedArea : Toprope.Models.Area
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of sectors.
        /// </summary>
        public IList<ParsedSector> Sectors { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ParsedArea()
        {
            Sectors = new List<ParsedSector>();
        }
    }
}
