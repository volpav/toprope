using System;
using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a list parameter query node. This class cannot be inherited.
    /// </summary>
    public sealed class ListParameterQueryNode : ParameterQueryNode
    {
        #region Properties

        /// <summary>
        /// Gets the values.
        /// </summary>
        public IEnumerable<object> Values { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="values">Values.</param>
        public ListParameterQueryNode(string name, IEnumerable<object> values) : base(name, ParameterQueryNodeType.List)
        {
            if (values == null || !values.Any(v => v != null && !(v is QueryNode)))
                throw new ArgumentException("List does not contain any valid values.", "values");
            else if (values.Where(v => v != null && !(v is QueryNode)).Count() == 1)
                throw new ArgumentException("List contains only one value. Use SingleParameterQueryNode instead.", "values");
            else
                this.Values = values.Where(v => v != null && !(v is QueryNode));
        }
    }
}