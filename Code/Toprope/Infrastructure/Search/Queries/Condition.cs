namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a query condition.
    /// </summary>
    public class Condition
    {
        #region Properties

        /// <summary>
        /// Gets or sets the target objet shape property.
        /// </summary>
        public Metadata.ObjectProperty Property { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Condition() { }
    }
}