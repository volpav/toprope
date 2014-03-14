using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a native query builder.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    public abstract class NativeQueryBuilder<T>
    {
        #region Properties

        /// <summary>
        /// Gets value indicating whether an appropriate query has been built.
        /// </summary>
        public virtual bool Complete
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected NativeQueryBuilder() { }

        /// <summary>
        /// Resets the builder state.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Appends the CONDITIONS statement.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Builder.</returns>
        public abstract NativeQueryBuilder<T> Where(IEnumerable<Condition> conditions);

        /// <summary>
        /// Limits the number of records to be selected.
        /// </summary>
        /// <param name="range">Range.</param>
        /// <returns>Builder.</returns>
        public abstract NativeQueryBuilder<T> Limit(Range<int> range);
    }
}