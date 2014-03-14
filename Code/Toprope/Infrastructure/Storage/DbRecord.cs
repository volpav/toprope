using System;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a database record.
    /// </summary>
    public abstract class DbRecord
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Id of this record.
        /// </summary>
        public Guid Id { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected DbRecord() { }
    }
}