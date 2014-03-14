using System;

namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents object property flags.
    /// </summary>
    [Flags]
    public enum ObjectPropertyFlags
    {
        /// <summary>
        /// No flags set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Searchable property.
        /// </summary>
        Searchable = 1,

        /// <summary>
        /// Property value comparison is bitwise flag check.
        /// </summary>
        BitwiseComparison = 2
    }

    /// <summary>
    /// Represents an object property.
    /// </summary>
    public class ObjectProperty : IChildElement<ObjectShape>, IComparable, IComparable<ObjectProperty>
    {
        #region Properties

        private string _name = string.Empty;
        private string _alias = string.Empty;
        private ObjectShape _parent = null;

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _alias = string.Empty;
            }
        }

        /// <summary>
        /// Gets the property alias.
        /// </summary>
        public string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(_alias))
                {
                    _alias = Name;

                    if (Parent != null)
                        _alias = Parent.Name + Name;
                }

                return _alias;
            }
        }

        /// <summary>
        /// Gets or sets the property type.
        /// </summary>
        public System.Type Type { get; set; }

        /// <summary>
        /// Gets or sets the parent shape element.
        /// </summary>
        public ObjectShape Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                _alias = string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the relation of this property to other shape.
        /// </summary>
        public ObjectPropertyRelation Relation { get; set; }

        /// <summary>
        /// Gets or sets the object property flags.
        /// </summary>
        public ObjectPropertyFlags Flags { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ObjectProperty() { }

        /// <summary>
        /// Returns value indicating whether the given flag is set.
        /// </summary>
        /// <param name="flag">Flag to check.</param>
        /// <returns>Value indicating whether the given flag is set.</returns>
        public bool HasFlag(ObjectPropertyFlags flag)
        {
            return Flags.HasFlag(flag);
        }

        /// <summary>
        /// Sets the given flag.
        /// </summary>
        /// <param name="flag">Flag to set.</param>
        public void SetFlag(ObjectPropertyFlags flag)
        {
            if (Flags == ObjectPropertyFlags.None)
                Flags = flag;
            else
                Flags |= flag;
        }

        /// <summary>
        /// Removes the given flag.
        /// </summary>
        /// <param name="flag">Flag to remove.</param>
        public void RemoveFlag(ObjectPropertyFlags flag)
        {
            if (Flags == flag)
                Flags = ObjectPropertyFlags.None;
            else
                Flags &= ~flag;
        }

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
                else if (obj is ObjectProperty)
                    ret = CompareTo(obj as ObjectProperty);
            }

            return ret;
        }

        /// <summary>
        /// Compares the current object to the given one.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(ObjectProperty other)
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
                        ret = string.Compare(Alias ?? string.Empty, other.Alias ?? string.Empty, System.StringComparison.InvariantCultureIgnoreCase);
                        if (ret == 0)
                        {
                            ret = (int)Flags - (int)other.Flags;

                            if (ret == 0)
                            {
                                if (Type != null && other.Type == null)
                                    ret = -1;
                                else if (Type == null && other.Type != null)
                                    ret = 1;
                                else if (Type != null && other.Type != null)
                                    ret = string.Compare(Type.FullName, other.Type.FullName, System.StringComparison.InvariantCultureIgnoreCase);

                                if (ret == 0)
                                {
                                    if (Parent != null && other.Parent == null)
                                        ret = -1;
                                    else if (Parent == null && other.Parent != null)
                                        ret = 1;
                                    else if (Parent != null && other.Parent != null)
                                        ret = Type.ReferenceEquals(Parent, other.Parent) ? 0 : -1;

                                    if (ret == 0)
                                    {
                                        if (Relation != null && other.Relation == null)
                                            ret = -1;
                                        else if (Relation == null && other.Relation != null)
                                            ret = 1;
                                        else if (Relation != null && other.Relation != null)
                                        {
                                            if (Relation.Shape != null && other.Relation.Shape == null)
                                                ret = -1;
                                            else if (Relation.Shape == null && other.Relation.Shape != null)
                                                ret = 1;
                                            else if (Relation.Shape != null && other.Relation.Shape != null)
                                                ret = Type.ReferenceEquals(Relation.Shape, other.Relation.Shape) ? 0 : -1;
                                        }
                                    }
                                }
                            }
                        }
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
            int ret = (Name ?? string.Empty).GetHashCode() ^ (Alias ?? string.Empty).GetHashCode();

            if (Type != null)
                ret ^= Type.GetHashCode();

            return ret;
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