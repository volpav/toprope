using System;
using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents a search field manager.
    /// </summary>
    public class SearchFieldManager
    {
        #region Properties

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        private List<SearchField> _fields = new List<SearchField>();

        /// <summary>
        /// Gets or sets the "Full field name -> Occured in list" mapping.
        /// </summary>
        private Dictionary<string, int> _fullNameToField = new Dictionary<string, int>();

        /// <summary>
        /// Gets or sets the "Field name -> Occured in list" mapping.
        /// </summary>
        private Dictionary<string, HashSet<int>> _nameToFields = new Dictionary<string, HashSet<int>>();

        /// <summary>
        /// Gets or sets the "Shape name -> Occured in fields" mapping.
        /// </summary>
        private Dictionary<string, HashSet<int>> _nameToShapes = new Dictionary<string, HashSet<int>>();

        /// <summary>
        /// Gets all currently registered search fields.
        /// </summary>
        public IEnumerable<SearchField> Fields
        {
            get { return _fields.AsEnumerable(); }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchFieldManager() { }

        /// <summary>
        /// Registers multiple search fields for a given object shapes.
        /// </summary>
        /// <param name="shapes">Object shapes.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="shapes">shapes</paramref> is null.</exception>
        public void RegisterMultipleFields(IEnumerable<ObjectShape> shapes)
        {
            List<ObjectShape> list = null;

            if (shapes == null)
                throw new ArgumentNullException("shapes");
            else
            {
                list = new List<ObjectShape>(shapes.Where(s => s != null));

                for (int i = 0; i < list.Count; i++)
                    RegisterMultipleFieldsInternal(list[i], i == list.Count - 1);
            }
        }

        /// <summary>
        /// Registers multiple search fields for a given object shape.
        /// </summary>
        /// <param name="shape">Object shape.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="shape">shape</paramref> is null.</exception>
        public void RegisterMultipleFields(ObjectShape shape)
        {
            if (shape == null)
                throw new ArgumentNullException("shape");
            else
                RegisterMultipleFieldsInternal(shape, true);
        }

        /// <summary>
        /// Registers multiple search fields for a given object shape properties.
        /// </summary>
        /// <param name="properties">Object shape properties.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="properties">properties</paramref> is null.</exception>
        public void RegisterMultipleFields(IEnumerable<ObjectProperty> properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");
            else
                RegisterMultipleFieldsInternal(properties, true);
        }

        /// <summary>
        /// Registers new search field that corresponds to a given object shape property.
        /// </summary>
        /// <param name="property">Object shape property.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="property">property</paramref> is null.</exception>
        public void RegisterField(ObjectProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            else
                RegisterFieldInternal(property, true);
        }

        /// <summary>
        /// Returns all fields with the given parameter name.
        /// </summary>
        /// <param name="name">Field parameter name.</param>
        /// <returns>Fields.</returns>
        public IEnumerable<SearchField> GetFieldsByName(string name)
        {
            HashSet<int> indexes = null;
            List<SearchField> ret = new List<SearchField>();

            if (name != null && _nameToFields.ContainsKey(name))
            {
                indexes = _nameToFields[name];

                if (indexes != null)
                {
                    foreach (int index in indexes)
                    {
                        if (index >= 0 && index < _fields.Count)
                            ret.Add(_fields[index]);
                    }
                }
            }

            return ret.AsEnumerable();
        }

        /// <summary>
        /// Registers multiple search fields for a given object shape.
        /// </summary>
        /// <param name="shape">Object shape.</param>
        /// <param name="updateIndexes">Value indicating whether to update indexes.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="shape">shape</paramref> is null.</exception>
        private void RegisterMultipleFieldsInternal(ObjectShape shape, bool updateIndexes)
        {
            string[] nameSearchableProperties = { "Id", "Name", "Description" };

            if (shape != null)
            {
                RegisterMultipleFieldsInternal(shape.Properties, false);

                foreach (ObjectProperty p in shape.Properties.Where(p => p != null && !string.IsNullOrEmpty(p.Name) &&
                    nameSearchableProperties.Any(sp => string.Compare(sp, p.Name, StringComparison.InvariantCultureIgnoreCase) == 0)))
                {
                    RegisterFieldInternal(p, shape.Name, false);
                }

                if (updateIndexes)
                    UpdateIndexes();
            }
        }

        /// <summary>
        /// Registers multiple search fields for a given object shape properties.
        /// </summary>
        /// <param name="properties">Object shape properties.</param>
        /// <param name="updateIndexes">Value indicating whether to update indexes.</param>
        private void RegisterMultipleFieldsInternal(IEnumerable<ObjectProperty> properties, bool updateIndexes)
        {
            List<ObjectProperty> list = null;

            if (properties != null)
            {
                list = new List<ObjectProperty>(properties.Where(p => p != null));

                for (int i = 0; i < list.Count; i++)
                    RegisterFieldInternal(list[i], updateIndexes);
            }
        }

        /// <summary>
        /// Registers new search field that corresponds to a given object shape property.
        /// </summary>
        /// <param name="property">Object shape property.</param>
        /// <param name="updateIndexes">Value indicating whether to update indexes.</param>
        private void RegisterFieldInternal(ObjectProperty property, bool updateIndexes)
        {
            RegisterFieldInternal(property, string.Empty, updateIndexes);
        }

        /// <summary>
        /// Registers new search field that corresponds to a given object shape property.
        /// </summary>
        /// <param name="property">Object shape property.</param>
        /// <param name="name">Field name.</param>
        /// <param name="updateIndexes">Value indicating whether to update indexes.</param>
        private void RegisterFieldInternal(ObjectProperty property, string name, bool updateIndexes)
        {
            SearchField field = null;
            string[] freeTextSearchableProperties = { "Name", "Description", "Tags" };

            System.Action tryAddField = () =>
                {
                    string fullName = GetFullName(field);

                    if (!_fullNameToField.ContainsKey(fullName))
                    {
                        _fields.Add(field);
                        _fullNameToField.Add(fullName, _fields.Count - 1);

                        if (updateIndexes)
                            UpdateIndexes();
                    }
                };

            if (property != null && IsValidSearchFieldProperty(property))
            {
                field = new SearchField();

                field.Name = name;

                if (string.IsNullOrEmpty(name))
                    field.Name = property.Name;

                field.Property = property;

                tryAddField();

                if (string.IsNullOrEmpty(name) && freeTextSearchableProperties.Any(p => string.Compare(p, property.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                {
                    field = new SearchField();

                    field.Name = string.Empty;
                    field.Property = property;

                    tryAddField();
                }
            }
        }

        /// <summary>
        /// Updates indexes.
        /// </summary>
        private void UpdateIndexes()
        {
            string fullName = string.Empty;

            _fullNameToField.Clear();

            for (int i = 0; i < _fields.Count; i++)
            {
                fullName = GetFullName(_fields[i]);

                if (!string.IsNullOrEmpty(fullName) && !_fullNameToField.ContainsKey(fullName))
                    _fullNameToField.Add(fullName, i);
            }

            _nameToFields.Clear();

            for (int i = 0; i < _fields.Count; i++)
            {
                if (!_nameToFields.ContainsKey(_fields[i].Name))
                    _nameToFields.Add(_fields[i].Name, new HashSet<int>());

                _nameToFields[_fields[i].Name].Add(i);
            }

            _nameToShapes.Clear();

            for (int i = 0; i < _fields.Count; i++)
            {
                if (_fields[i].Property.Parent != null)
                {
                    if (!_nameToShapes.ContainsKey(_fields[i].Property.Parent.Name))
                        _nameToShapes.Add(_fields[i].Property.Parent.Name, new HashSet<int>());

                    _nameToShapes[_fields[i].Property.Parent.Name].Add(i);
                }
            }
        }

        /// <summary>
        /// Returns value indicating whether the given property is valid for being assigned to a search field.
        /// </summary>
        /// <param name="property">Object property to examine.</param>
        /// <returns>Value indicating whether the given property is valid for being assigned to a search field.</returns>
        private bool IsValidSearchFieldProperty(ObjectProperty property)
        {
            return property != null && property.Type != null && property.HasFlag(ObjectPropertyFlags.Searchable);
        }

        /// <summary>
        /// Returns the full name of a field.
        /// </summary>
        /// <param name="field">Field.</param>
        /// <returns>Full name of the field.</returns>
        private string GetFullName(SearchField field)
        {
            return field != null ? (field.Property.Parent != null ? string.Format("{0}:{1}.{2}", string.IsNullOrEmpty(field.Name) ? "*" : field.Name, field.Property.Parent.Name, field.Property.Name) : string.Format("{0}:{1}", field.Name, field.Property.Name)) : string.Empty;
        }
    }
}