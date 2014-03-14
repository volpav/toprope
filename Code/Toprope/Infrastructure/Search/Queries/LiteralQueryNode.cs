namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a literal query node. This class cannot be inherited.
    /// </summary>
    public sealed class LiteralQueryNode : QueryNode
    {
        #region Properties

        /// <summary>
        /// Gets the node value.
        /// </summary>
        public string Value { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="value">Node value.</param>
        /// <exception cref="System.ArgumentException"><paramref name="value">value</paramref> is null or empty string.</exception>
        public LiteralQueryNode(string value) : base(QueryNodeType.Literal)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new System.ArgumentException("Literal node value must be specified.", "value");
            else
            {
                this.Value = value.Trim();

                if (this.Value.StartsWith("\"") && this.Value.EndsWith("\""))
                    this.Value = this.Value.Substring(1, this.Value.Length - 2).Trim();
            }
        }
    }
}