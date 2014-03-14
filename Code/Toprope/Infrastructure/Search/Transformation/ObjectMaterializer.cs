using System.Collections.Generic;

namespace Toprope.Infrastructure.Search.Transformation
{
    /// <summary>
    /// Represents an object materializer.
    /// </summary>
    public abstract class ObjectMaterializer : System.IComparable, System.IComparable<ObjectMaterializer>
    {
        #region Properties

        /// <summary>
        /// Gets the materialized object type.
        /// </summary>
        public abstract System.Type ResultType { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected ObjectMaterializer() { }

        /// <summary>
        /// Materializes objects from the given raw data.
        /// </summary>
        /// <param name="data">Raw data.</param>
        /// <returns>Object instances.</returns>
        public abstract IEnumerable<object> Materialize(IList<IDictionary<string, object>> data);

        /// <summary>
        /// Compares the current object to the given one.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(object obj)
        {
            int ret = -1;

            if (obj != null)
            {
                if (System.Type.ReferenceEquals(this, obj))
                    ret = 0;
                else if (obj is ObjectMaterializer)
                    ret = CompareTo(obj as ObjectMaterializer);
            }

            return ret;
        }

        /// <summary>
        /// Compares the current object to the given one.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(ObjectMaterializer other)
        {
            int ret = -1;

            if (other != null)
            {
                if (System.Type.ReferenceEquals(this, other))
                    ret = 0;
                else
                {
                    if (ResultType != null && other.ResultType == null)
                        ret = -1;
                    else if (ResultType == null && other.ResultType != null)
                        ret = 1;
                    else if (ResultType != null && other.ResultType != null)
                        ret = string.Compare(ResultType.FullName, other.ResultType.FullName, System.StringComparison.InvariantCultureIgnoreCase);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns a hash code of the current object.
        /// </summary>
        /// <returns>A hash code of the current object.</returns>
        public override int GetHashCode()
        {
            return ResultType != null ? ResultType.GetHashCode() : base.GetHashCode();
        }

        /// <summary>
        /// Returns value indicating whether current object is equal to the given one.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>Value indicating whether current object is equal to the given one.</returns>
        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>A string representation of the current object.</returns>
        public override string ToString()
        {
            return ResultType != null ? string.Format("To: {0}", ResultType.FullName) : string.Empty;
        }

        /// <summary>
        /// Returns the value of a given field in a given document.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="document">Document.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>The value of a given field.</returns>
        protected T GetValue<T>(IDictionary<string, object> document, string fieldName)
        {
            T ret = default(T);
            object val = GetValue(document, fieldName);

            if (val != null)
            {
                if (val is T)
                    ret = (T)val;
                else
                {
                    try
                    {
                        ret = (T)System.Convert.ChangeType(val, typeof(T));
                    }
                    catch (System.InvalidCastException) { }
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the value of a given field in a given document.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>The value of a given field.</returns>
        protected object GetValue(IDictionary<string, object> document, string fieldName)
        {
            return document != null && !string.IsNullOrEmpty(fieldName) && document.ContainsKey(fieldName) ? (document[fieldName] != null && document[fieldName] is System.DBNull ? null : document[fieldName]) : null;
        }
    }
}