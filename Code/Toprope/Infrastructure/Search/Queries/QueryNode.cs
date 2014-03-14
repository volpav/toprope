namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a query node type.
    /// </summary>
    public enum QueryNodeType
    {
        /// <summary>
        /// Literal node.
        /// </summary>
        Literal = 1,

        /// <summary>
        /// Parameter.
        /// </summary>
        Parameter = 2
    }

    /// <summary>
    /// Represents a query node.
    /// </summary>
    public abstract class QueryNode
    {
        #region Properties

        /// <summary>
        /// Gets the query node type.
        /// </summary>
        public QueryNodeType Type { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="type">Query node type.</param>
        protected QueryNode(QueryNodeType type)
        {
            this.Type = type;
        }
    }
}