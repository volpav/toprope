using System;

namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents a search field.
    /// </summary>
    public class SearchField : IComparable, IComparable<SearchField>
    {
        #region Properties

        private string _name = string.Empty;

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = (value ?? string.Empty).ToLowerInvariant(); }
        }

        /// <summary>
        /// Gets or sets the associated object property.
        /// </summary>
        public ObjectProperty Property { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchField() { }

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
                if (Type.ReferenceEquals(this, obj))
                    ret = 0;
                else if (obj is SearchField)
                    ret = CompareTo(obj as SearchField);
            }

            return ret;
        }

        /// <summary>
        /// Compares the current object to the given one.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(SearchField other)
        {
            int ret = -1;

            if (other != null)
            {
                if (Type.ReferenceEquals(this, other))
                    ret = 0;
                else
                {
                    ret = string.Compare(Name ?? string.Empty, other.Name ?? string.Empty, System.StringComparison.InvariantCultureIgnoreCase);
                    if (ret == 0)
                    {
                        if (Property != null && other.Property == null)
                            ret = -1;
                        else if (Property == null && other.Property != null)
                            ret = 1;
                        else if (Property != null && other.Property != null)
                            ret = Property.CompareTo(other.Property);
                    }
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
            return (Name ?? string.Empty).GetHashCode() ^ (Property != null ? Property.GetHashCode() : 0);
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
            return Name;
        }
    }
}