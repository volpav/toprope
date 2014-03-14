namespace Toprope.Infrastructure.Search
{
    /// <summary>
    /// Represents a child element.
    /// </summary>
    /// <typeparam name="T">Parent element type.</typeparam>
    public interface IChildElement<T>
    {
        /// <summary>
        /// Gets or sets the parent element.
        /// </summary>
        T Parent { get; set; }
    }
}