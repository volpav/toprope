using System.Collections.Generic;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a climbing sector.
    /// </summary>
    public class Sector : Place
    {
        #region Properties

        /// <summary>
        /// Gets or sets the area Id.
        /// </summary>
        public System.Guid AreaId { get; set; }

        /// <summary>
        /// Gets or sets the 1-based order of the sector in a given region.
        /// </summary>
        public int Order { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Sector()
        {
            Tags = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public Sector(Sector copyFrom)
        {
            if (copyFrom == null)
                throw new System.ArgumentNullException("copyFrom");
            else
                copyFrom.CopyTo(this);
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public Sector(Place copyFrom) : base(copyFrom) { }

        /// <summary>
        /// Copies the state of the current object into the given one.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="target">target</paramref> is null.</exception>
        public void CopyTo(Sector target)
        {
            if (target == null)
                throw new System.ArgumentNullException("target");
            else
            {
                base.CopyTo(target);
                
                target.AreaId = this.AreaId;
                target.Order = this.Order;
            }
        }
    }
}