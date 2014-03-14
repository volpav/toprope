using System;

namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents an object property relation to other object shape.
    /// </summary>
    public class ObjectPropertyRelation : IChildElement<ObjectProperty>, IComparable, IComparable<ObjectPropertyRelation>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the parent property.
        /// </summary>
        public ObjectProperty Parent { get; set; }

        /// <summary>
        /// Gets or sets the shape the property relates to.
        /// </summary>
        public ObjectShape Shape { get; set; }
        
        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ObjectPropertyRelation() { }

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
                else if (obj is ObjectPropertyRelation)
                    ret = CompareTo(obj as ObjectPropertyRelation);
            }

            return ret;
        }

        /// <summary>
        /// Compares the current object to the given one.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(ObjectPropertyRelation other)
        {
            int ret = -1;

            if (other != null)
            {
                if (Type.ReferenceEquals(this, other))
                    ret = 0;
                else
                {
                    if (Parent != null && other.Parent == null)
                        ret = -1;
                    else if (Parent == null && other.Parent != null)
                        ret = 1;
                    else if (Parent != null && other.Parent != null)
                        ret = Parent.CompareTo(other.Parent);

                    if (ret == 0)
                    {
                        if (Shape != null && other.Shape == null)
                            ret = -1;
                        else if (Shape == null && other.Shape != null)
                            ret = 1;
                        else if (Shape != null && other.Shape != null)
                            ret = Shape.CompareTo(other.Shape);
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
            return (Parent != null ? Parent.GetHashCode() : 0) ^ (Shape != null ? Shape.GetHashCode() : 0);
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
            return Shape != null ? Shape.ToString() : string.Empty;
        }
    }
}