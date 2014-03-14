using System;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a single value parameter query node. This class cannot be inherited.
    /// </summary>
    public sealed class SingleValueParameterQueryNode : ParameterQueryNode
    {
        #region Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <exception cref="System.ArgumentException"><paramref name="value">value</paramref> is null.</exception>
        public SingleValueParameterQueryNode(string name, object value) : base(name, ParameterQueryNodeType.SingleValue)
        {
            if (value == null || value is QueryNode)
                throw new ArgumentException("Parameter value must be specified and valid.", "value");
            else
                this.Value = value;
        }
    }
}