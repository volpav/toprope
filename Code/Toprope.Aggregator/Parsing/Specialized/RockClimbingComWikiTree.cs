using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toprope.Aggregator.Parsing.Specialized
{
    /// <summary>
    /// Represents a rockclimbing.com wiki tree.
    /// </summary>
    public class RockClimbingComWikiTree : RockClimbingComWikiNode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of sector nodes.
        /// </summary>
        public IList<RockClimbingComWikiNode> SectorNodes { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RockClimbingComWikiTree() { }
    }
}
