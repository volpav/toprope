namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a parameter query node type.
    /// </summary>
    public enum ParameterQueryNodeType
    {
        /// <summary>
        /// Single value.
        /// </summary>
        SingleValue = 0,

        /// <summary>
        /// Range.
        /// </summary>
        Range = 1,

        /// <summary>
        /// List.
        /// </summary>
        List = 2
    }

    /// <summary>
    /// Represents a parameter query node.
    /// </summary>
    public abstract class ParameterQueryNode : QueryNode
    {
        #region Properties

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public ParameterQueryNodeType ParameterType { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="parameterType">Parameter type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="name">name</paramref> is null or empty string.</exception>
        protected ParameterQueryNode(string name, ParameterQueryNodeType parameterType) : base(QueryNodeType.Parameter)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Parameter name must be specified.", "name");
            else
            {
                this.Name = name.ToLowerInvariant();
                this.ParameterType = parameterType;
            }
        }
    }
}