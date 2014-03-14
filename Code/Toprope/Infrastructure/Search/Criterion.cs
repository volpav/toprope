namespace Toprope.Infrastructure.Search
{
    /// <summary>
    /// Represents a search criterion.
    /// </summary>
    public class Criterion
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the criterion.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the criterion value.
        /// </summary>
        public string Value { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Criterion() : this(string.Empty, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="name">The name of the criterion.</param>
        /// <param name="value">The criterion value.</param>
        public Criterion(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Criterion to copy state from.</param>
        public Criterion(Criterion copyFrom)
        {
            if (copyFrom != null)
            {
                this.Name = copyFrom.Name;
                this.Value = copyFrom.Value;
            }
        }
    }
}