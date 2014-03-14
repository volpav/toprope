using System;
using System.Linq;
using System.Collections.Generic;

namespace Toprope.Aggregator.Parsing.Specialized
{
    /// <summary>
    /// Represents a wiki node at rockclimbing.com.
    /// </summary>
    public class RockClimbingComWikiNode : RockClimbingComWikiNodeBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        public RockClimbingComWikiNode Parent { get; set; }

        /// <summary>
        /// Gets or sets the list of child nodes.
        /// </summary>
        public IList<RockClimbingComWikiNode> ChildNodes { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RockClimbingComWikiNode() { }

        /// <summary>
        /// Accumulates the information on the given node with all parent nodes.
        /// </summary>
        /// <returns>Accumulated result.</returns>
        public RockClimbingComWikiNodeBase AccumulateAll()
        {
            RockClimbingComWikiNodeBase ret = this;

            if (Parent != null)
                ret = MergeAllWith(Parent.AccumulateAll());

            return ret;
        }

        /// <summary>
        /// Accumulates all tags on the given node and all parent nodes.
        /// </summary>
        /// <param name="extra">Extra tags.</param>
        /// <returns>Tags.</returns>
        public IList<string> GetTags(IEnumerable<string> extra)
        {
            string t = string.Empty;
            List<string> ret = new List<string>();

            if (Info != null)
            {
                t = Infrastructure.Utilities.Input.FormatTag(Info.Name);
                if (!string.IsNullOrEmpty(t))
                    ret.Add(t);

                if (Parent != null)
                {
                    foreach (string pt in Parent.GetTags(null).Reverse())
                        ret.Insert(0, pt);
                }

                if (extra != null && extra.Any())
                {
                    foreach (string e in extra.Reverse())
                    {
                        t = Infrastructure.Utilities.Input.FormatTag(e);
                        if (!string.IsNullOrEmpty(t))
                            ret.Insert(0, t);
                    }
                }
            }

            return ret.Distinct().ToList();
        }

        /// <summary>
        /// Accumulates wiki data on the given nodes with all parent nodes.
        /// </summary>
        /// <returns>Accumulated result.</returns>
        public RockClimbingComWiki AccumulateWiki()
        {
            RockClimbingComWiki ret = this.Info;

            if (Parent != null)
                ret = MergeWikiWith(Parent.AccumulateWiki(), true);

            return ret;
        }

        /// <summary>
        /// Merges the current node data with the given node data.
        /// </summary>
        /// <param name="node">Node to merge with.</param>
        /// <returns>Merged node.</returns>
        private RockClimbingComWikiNodeBase MergeAllWith(RockClimbingComWikiNodeBase node)
        {
            RockClimbingComWikiNodeBase ret = this;

            if (node != null)
            {
                if (ret.Info != null && node.Info != null)
                {
                    // Merging wiki info (description is not merged)
                    ret.Info = ret.MergeWikiWith(node.Info, false);
                }

                // Merging contents
                if ((ret.Contents == null || !ret.Contents.Any()) && node.Contents != null && node.Contents.Any())
                    ret.Contents = node.Contents;
                else if (ret.Contents != null && ret.Contents.Any() && node.Contents != null && node.Contents.Any())
                {
                    foreach (string c in node.Contents.Reverse())
                    {
                        if (!ret.Contents.Any(candidate => candidate.Length == c.Length))
                            ret.Contents.Insert(0, c);
                    }
                }
            }

            return ret;
        }
    }
}
