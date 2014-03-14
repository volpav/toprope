using System;
using System.Linq;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents an object shape.
    /// </summary>
    public class ObjectShape : IComparable, IComparable<ObjectShape>
    {
        #region Properties

        private string _name = string.Empty;
        private string _alias = string.Empty;

        /// <summary>
        /// Gets o sets the object name.
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
        /// Gets the shape alias.
        /// </summary>
        public string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(_alias) && !string.IsNullOrEmpty(Name))
                {
                    if (Name.Length > 2)
                        _alias = string.Join(string.Empty, Name[0].ToString(), Name[2].ToString());
                    else
                        _alias = Name[0].ToString();

                    _alias = _alias.ToLowerInvariant();
                }

                return _alias;
            }
        }

        /// <summary>
        /// Gets or sets the type of an anderlying object.
        /// </summary>
        public System.Type Type { get; set; }

        /// <summary>
        /// Gets the object shape properties.
        /// </summary>
        public ObjectPropertyCollection Properties { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ObjectShape()
        {
            Properties = new ObjectPropertyCollection(this);
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
                else if (obj is ObjectShape)
                    ret = CompareTo(obj as ObjectShape);
            }

            return ret;
        }

        /// <summary>
        /// Compares the current object to the given one.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(ObjectShape other)
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
                            if (Type != null && other.Type == null)
                                ret = -1;
                            else if (Type == null && other.Type != null)
                                ret = 1;
                            else if (Type != null && other.Type != null)
                                ret = string.Compare(Type.FullName, other.Type.FullName, System.StringComparison.InvariantCultureIgnoreCase);

                            if (ret == 0)
                            {
                                ret = Properties.Count - other.Properties.Count;
                                if (ret == 0)
                                {
                                    for (int i = 0; i < Properties.Count; i++)
                                    {
                                        if (Properties[i] != null && other.Properties[i] == null)
                                            ret = -1;
                                        else if (Properties[i] == null && other.Properties[i] != null)
                                            ret = 1;
                                        else if (Properties[i] != null && other.Properties[i] != null)
                                            ret = Properties[i].CompareTo(other.Properties[i]);

                                        if (ret != 0)
                                            break;
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