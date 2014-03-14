namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a range parser.
    /// </summary>
    internal class RangeParser : QueryVisitor
    {
        #region Properties

        /// <summary>
        /// Gets or sets how many items to skip.
        /// </summary>
        private int Skip { get; set; }

        /// <summary>
        /// Gets or sets how many items to take.
        /// </summary>
        private int Take { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RangeParser() { }

        /// <summary>
        /// Parses a range from the given query.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <returns>Range.</returns>
        public Range<int> Parse(Query query)
        {
            Range<int> ret = null;

            Skip = -1;
            Take = -1;

            Visit(query);

            if (Skip > 0 || Take > 0)
            {
                ret = new Range<int>();

                if (Skip <= 0 && Take > 0)
                    ret.To = Take;
                else if (Skip > 0 && Take <= 0)
                    ret.From = Skip;
                else 
                {
                    ret.From = Skip + 1;
                    ret.To = System.Math.Abs(Skip + Take);
                }
            }
            else
                ret = new Range<int>(-1, -1);

            return ret;
        }

        /// <summary>
        /// Visits the given single value parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected override SingleValueParameterQueryNode VisitSingleValueParameter(SingleValueParameterQueryNode node)
        {
            if (node != null)
            {
                if (string.Compare(node.Name, "skip", System.StringComparison.InvariantCultureIgnoreCase) == 0)
                    Skip = Utilities.Input.GetInt(node.Value);
                else if (string.Compare(node.Name, "take", System.StringComparison.InvariantCultureIgnoreCase) == 0)
                    Take = Utilities.Input.GetInt(node.Value);
            }

            return base.VisitSingleValueParameter(node);
        }
    }
}