using System;
using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Transformation
{
    /// <summary>
    /// Represents a generic object materializer.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    public class GenericObjectMaterializer<T> : ObjectMaterializer where T: new()
    {
        #region Properties

        /// <summary>
        /// Gets the materialized object type.
        /// </summary>
        public override Type ResultType
        {
            get { return typeof(T); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public GenericObjectMaterializer() { }

        /// <summary>
        /// Materializes objects from the given raw data.
        /// </summary>
        /// <param name="data">Raw data.</param>
        /// <returns>Object instances.</returns>
        public override IEnumerable<object> Materialize(IList<IDictionary<string, object>> data)
        {
            T item = default(T);
            List<T> ret = new List<T>();

            if (data != null && data.Any())
            {
                foreach (IDictionary<string, object> row in data)
                {
                    item = Storage.Repository.Read<T>(row);

                    if (item != null)
                        ret.Add(item);
                }
            }

            return ret.OfType<object>();
        }
    }
}