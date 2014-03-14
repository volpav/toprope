namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a range.
    /// </summary>
    /// <typeparam name="T">Bounds type.</typeparam>
    public class Range<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the lower bound.
        /// </summary>
        public T From { get; set; }

        /// <summary>
        /// Gets or sets the upper bound.
        /// </summary>
        public T To { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Range() { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="from">Lower bound.</param>
        /// <param name="to">Upper bound.</param>
        public Range(T @from, T to)
        {
            From = from;
            To = to;
        }
    }
}