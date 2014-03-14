namespace Toprope.Aggregator.Parsing.Specialized
{
    /// <summary>
    /// Represents a list item on RockClimbing.Com.
    /// </summary>
    public class RockClimbingComListItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the item URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the total amount of records for this item.
        /// </summary>
        public int Total { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RockClimbingComListItem() { }

        /// <summary>
        /// Returns string representation of the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Total);
        }
    }
}
