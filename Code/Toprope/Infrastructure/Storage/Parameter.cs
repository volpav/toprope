namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a query parameter.
    /// </summary>
    public class Parameter
    {
        #region Properties

        /// <summary>
        /// Gets or sets parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets parameter value.
        /// </summary>
        public object Value { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Parameter() : this(string.Empty, null) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public Parameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}