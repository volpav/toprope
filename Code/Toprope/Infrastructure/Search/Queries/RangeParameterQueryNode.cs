using System;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a range parameter query node. This class cannot be inherited.
    /// </summary>
    public sealed class RangeParameterQueryNode : ParameterQueryNode
    {
        #region Properties

        /// <summary>
        /// Gets the lower bound.
        /// </summary>
        public object LowerBound { get; private set; }

        /// <summary>
        /// Gets the upper bound.
        /// </summary>
        public object UpperBound { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="name">Paramter name.</param>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        public RangeParameterQueryNode(string name, object lowerBound, object upperBound) : base(name, ParameterQueryNodeType.Range)
        {
            if ((lowerBound == null && upperBound == null) || lowerBound is QueryNode || upperBound is QueryNode)
                throw new ArgumentException("Range bounds are invalid.", "bounds");
            else
            {
                this.LowerBound = lowerBound;
                this.UpperBound = upperBound;
            }
        }
    }
}