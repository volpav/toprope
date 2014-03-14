using System.Collections.Generic;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a climbing area.
    /// </summary>
    public class Area : Place
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Area() { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public Area(Area copyFrom) : base(copyFrom) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public Area(Place copyFrom) : base(copyFrom) { }
    }
}