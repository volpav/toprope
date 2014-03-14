using System.Collections.Generic;

namespace Toprope.Aggregator.Parsing.Specialized
{
    /// <summary>
    /// Represents a base class for wiki nodes at rockclimbing.com.
    /// </summary>
    public abstract class RockClimbingComWikiNodeBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the information about the given node.
        /// </summary>
        public RockClimbingComWiki Info { get; set; }

        /// <summary>
        /// Gets or sets the routes.
        /// </summary>
        public IList<ParsedRoute> Routes { get; set; }

        /// <summary>
        /// Gets or sets the raw node contents.
        /// </summary>
        public IList<string> Contents { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected RockClimbingComWikiNodeBase() { }

        /// <summary>
        /// Merges the current node wiki with the given node wiki.
        /// </summary>
        /// <param name="wiki">Wiki to merge with.</param>
        /// <param name="mergeDescription">Value indicating whether to merge description.</param>
        /// <returns>Merged wiki.</returns>
        public RockClimbingComWiki MergeWikiWith(RockClimbingComWiki wiki, bool mergeDescription)
        {
            RockClimbingComWiki ret = this.Info;

            if (ret != null && wiki != null)
            {
                // Assigning location, if missing
                if (ret.Location == null && wiki.Location != null)
                    ret.Location = wiki.Location;

                // Assigning climbing type, if missing
                if (ret.Climbing == Models.ClimbingTypes.NotSpecified && wiki.Climbing != Models.ClimbingTypes.NotSpecified)
                    ret.Climbing = wiki.Climbing;

                // Assigning climbing season, if missing
                if (ret.Season == Models.Seasons.NotSpecified && wiki.Season != Models.Seasons.NotSpecified)
                    ret.Season = wiki.Season;

                if (mergeDescription)
                {
                    // Overriding description if the name changes
                    if (string.IsNullOrWhiteSpace(ret.Name) && !string.IsNullOrWhiteSpace(wiki.Name))
                    {
                        ret.Name = wiki.Name;
                        ret.Description = wiki.Description;
                    }
                    else if (ret.Description.Length * 3 < wiki.Description.Length)
                    {
                        // Candidate wiki has substantially more rich description - overriding
                        ret.Description = wiki.Description;
                    }
                }
            }

            return ret;
        }
    }
}
