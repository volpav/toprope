namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents a collection of object properties.
    /// </summary>
    public class ObjectPropertyCollection : ChildElementCollection<ObjectProperty, ObjectShape>
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// <param name="shape">Target shape.</param>
        /// </summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="shape">shape</paramref> is null.</exception>
        public ObjectPropertyCollection(ObjectShape shape) : base(shape) { }
    }
}