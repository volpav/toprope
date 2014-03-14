using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Toprope.Infrastructure.Search.Metadata
{
    /// <summary>
    /// Represents an object shape manager.
    /// </summary>
    public class ObjectShapeManager
    {
        #region Properties

        private IEnumerable<Type> _searchableProperties = null;

        /// <summary>
        /// Gets or sets the properties mapped to their type names.
        /// </summary>
        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _typeProperties = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// Gets or sets the relation paths.
        /// </summary>
        private IDictionary<ObjectShape, IDictionary<ObjectShape, int>> _relationPaths = new Dictionary<ObjectShape, IDictionary<ObjectShape, int>>();

        /// <summary>
        /// Gets or sets object shapes list.
        /// </summary>
        private List<ObjectShape> _shapes = new List<ObjectShape>();

        /// <summary>
        /// Gets all currently registered object shapes.
        /// </summary>
        public IEnumerable<ObjectShape> Shapes
        {
            get { return _shapes.AsEnumerable(); }
        }

        /// <summary>
        /// Gets or sets the list of searchable property types.
        /// </summary>
        public IEnumerable<Type> SearchableProperties
        {
            get { return _searchableProperties; }
            set
            {
                _searchableProperties = value;

                UpdatePropertyAttributes();
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ObjectShapeManager()
        {
            SearchableProperties = SearchSettings.DefaultSearchableProperties;
        }

        /// <summary>
        /// Returns object shape by object type.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <returns>Object shape.</returns>
        public ObjectShape GetShapeByType(Type type)
        {
            return type != null ? _shapes.FirstOrDefault(s => s.Type.Equals(type)) : null;
        }

        /// <summary>
        /// Returns all relations defined on a given object shape.
        /// </summary>
        /// <param name="shape">Object shape.</param>
        /// <returns>Relations.</returns>
        public IEnumerable<ObjectPropertyRelation> GetRelations(ObjectShape shape)
        {
            return shape != null ? shape.Properties.Where(p => p.Relation != null).Select(p => p.Relation) : new List<ObjectPropertyRelation>().AsEnumerable();
        }

        /// <summary>
        /// Returns value indicating whether there's a direct relation between two object shapes.
        /// </summary>
        /// <param name="shape1">First shape.</param>
        /// <param name="shape2">Second shape.</param>
        /// <returns>Value indicating whether there's a direct relation between two object shapes.</returns>
        public bool RelationExists(ObjectShape shape1, ObjectShape shape2)
        {
            return shape1 != null && shape2 != null &&
                (_relationPaths.ContainsKey(shape1) && _relationPaths[shape1].ContainsKey(shape2)) ||
                (_relationPaths.ContainsKey(shape2) && _relationPaths[shape2].ContainsKey(shape1));
        }

        /// <summary>
        /// Returns the first matching relation chain between two shapes.
        /// </summary>
        /// <param name="fromShape">Shape to begin with.</param>
        /// <param name="toShape">Shape to end with.</param>
        /// <returns>Relation chain.</returns>
        public IList<ObjectShape> GetRelationChain(ObjectShape fromShape, ObjectShape toShape)
        {
            List<ObjectShape> ret = new List<ObjectShape>();

            if (fromShape != null && toShape != null && fromShape != toShape)
            {
                AddRelationsToChainRecursive(ret, fromShape, toShape);

                if (ret.Any())
                {
                    if (ret[0] != fromShape)
                        ret.Insert(0, fromShape);
                }
                else
                {
                    AddRelationsToChainRecursive(ret, toShape, fromShape);

                    if (ret.Any() && ret[0] != toShape)
                        ret.Insert(0, toShape);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns relation distance between two shapes.
        /// </summary>
        /// <param name="fromShape">Shape 1.</param>
        /// <param name="toShape">Shape 2.</param>
        /// <returns>Relation distance.</returns>
        public int GetRelationDistance(ObjectShape fromShape, ObjectShape toShape)
        {
            int ret = 0;

            if (fromShape != null && toShape != null)
            {
                if (_relationPaths.ContainsKey(fromShape) && _relationPaths[fromShape].ContainsKey(toShape))
                    ret = _relationPaths[fromShape][toShape];
            }

            return ret;
        }

        /// <summary>
        /// Recursively adds relations to a given 
        /// </summary>
        /// <param name="addTo">List to add relations to.</param>
        /// <param name="startWith">Start with shape.</param>
        /// <param name="endWith">End with shape.</param>
        private void AddRelationsToChainRecursive(List<ObjectShape> addTo, ObjectShape startWith, ObjectShape endWith)
        {
            List<ObjectShape> newAddTo = null;
            IEnumerable<ObjectPropertyRelation> relations = null;

            if (addTo != null && startWith != null && endWith != null && startWith != endWith)
            {
                relations = GetRelations(startWith);
                if (relations != null && relations.Any())
                {
                    if (relations.Any(r => r.Shape == endWith))
                    {
                        if (!addTo.Any())
                            addTo.Add(startWith);

                        addTo.Add(endWith);
                    }
                    else
                    {
                        foreach (ObjectPropertyRelation relation in relations)
                        {
                            newAddTo = new List<ObjectShape>(addTo);

                            AddRelationsToChainRecursive(newAddTo, relation.Shape, endWith);
                            
                            if (newAddTo.Any() && newAddTo[newAddTo.Count - 1] == endWith)
                            {
                                addTo.Clear();
                                addTo.AddRange(newAddTo);
                                
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers new objet shape that is represented by a given type.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        public void RegisterShape<T>()
        {
            RegisterShape(typeof(T));
        }

        /// <summary>
        /// Registers new objet shape that is represented by a given type.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="type">type</paramref> is null.</exception>
        public void RegisterShape(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            else
                RegisterShapeInternal(type, true);
        }

        /// <summary>
        /// Registers several object shapes that are represented by a given types.
        /// </summary>
        /// <param name="types">Object types.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="types">types</paramref> is null.</exception>
        public void RegisterMultipleShapes(IEnumerable<Type> types)
        {
            List<Type> list = null;

            if (types == null)
                throw new ArgumentNullException("types");
            else
            {
                list = new List<Type>(types.Where(t => t != null).Distinct());
             
                for (int i = 0; i < list.Count; i++)
                    RegisterShapeInternal(list[i], i == list.Count - 1);
            }
        }

        /// <summary>
        /// Registers new object shapes that is represented by a given type.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="updateRelations">Value indicating whether to update relations.</param>
        private void RegisterShapeInternal(Type type, bool updateRelations)
        {
            ObjectShape shape = null;

            if(type != null)
            {
                shape = PartialParseShape(type);

                if (shape != null && !_shapes.Any(s => s.Type.Equals(shape.Type)))
                {
                    _shapes.Add(shape);

                    if (updateRelations)
                        UpdateRelations();
                }
            }
        }

        /// <summary>
        /// Parses partial shape information from the given type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Shape information (partial).</returns>
        private ObjectShape PartialParseShape(Type type)
        {
            ObjectShape ret = null;

            if (type != null)
            {
                ret = new ObjectShape();

                ret.Name = type.Name;
                ret.Type = type;

                foreach (ObjectProperty prop in PartialParseProperties(type))
                {
                    if (prop != null)
                        ret.Properties.Add(prop);
                }
            }

            return ret;
        }

        /// <summary>
        /// Parses partial shape property information from the given type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Shape property information (partial).</returns>
        private IEnumerable<ObjectProperty> PartialParseProperties(Type type)
        {
            ObjectProperty p = null;
            IEnumerable<PropertyInfo> properties = null;
            List<ObjectProperty> ret = new List<ObjectProperty>();
            
            if (type != null)
            {
                if (!_typeProperties.TryGetValue(type, out properties))
                    _typeProperties.AddOrUpdate(type, type.GetProperties(), (k, e) => { return type.GetProperties(); });

                if (_typeProperties.TryGetValue(type, out properties))
                {
                    foreach (PropertyInfo prop in properties)
                    {
                        p = PartialParseProperty(prop);

                        if (p != null)
                            ret.Add(p);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Parses partial shape property information from the given property.
        /// </summary>
        /// <param name="property">Property.</param>
        /// <returns>Shape property information (partial).</returns>
        private ObjectProperty PartialParseProperty(PropertyInfo property)
        {
            ObjectProperty ret = null;

            if (property != null)
            {
                ret = new ObjectProperty();

                ret.Name = property.Name;
                ret.Type = property.PropertyType;
                
                // ret.Relation is not parsed, needs to be updated when other shapes are available
            }

            return ret;
        }

        /// <summary>
        /// Updates relations between shapes.
        /// </summary>
        private void UpdateRelations()
        {
            ObjectShape shape = null;
            ObjectProperty property = null;
            ObjectShape relatedShape = null;
            string possibleRelatedShape = string.Empty;

            if (_shapes.Count > 1)
            {
                for (int i = 0; i < _shapes.Count; i++)
                {
                    shape = _shapes[i];

                    for (int j = 0; j < shape.Properties.Count; j++)
                    {
                        relatedShape = null;
                        property = shape.Properties[j];
                        
                        if(property.Type != null)
                        {
                            if (!string.IsNullOrEmpty(property.Name) && property.Type.Equals(typeof(Guid)))
                            {
                                if (property.Name.EndsWith("Id", StringComparison.InvariantCulture) && property.Name.Length > 2)
                                {
                                    possibleRelatedShape = property.Name.Substring(0, property.Name.Length - 2);
                                    relatedShape = _shapes.FirstOrDefault(s => s != shape && string.Compare(s.Name, possibleRelatedShape, StringComparison.InvariantCulture) == 0);
                                }
                            }
                            else if (Utilities.CodeMeta.IsEnumerable(property.Type))
                                relatedShape = GetShapeByType(Utilities.CodeMeta.GetEnumerableType(property.Type));
                            else
                                relatedShape = GetShapeByType(property.Type);
                        }

                        if (relatedShape != null && relatedShape != shape)
                        {
                            property.Relation = new ObjectPropertyRelation();

                            property.Relation.Parent = property;
                            property.Relation.Shape = relatedShape;
                        }
                    }
                }
            }

            UpdateRelationPaths();
            UpdatePropertyAttributes();
        }

        /// <summary>
        /// Updates relation paths.
        /// </summary>
        private void UpdateRelationPaths()
        {
            int chainSize = 0;
            IList<ObjectShape> chain = null;

            if (_shapes.Count > 1)
            {
                _relationPaths.Clear();

                for (int i = 0; i < _shapes.Count; i++)
                {
                    for (int j = 0; j < _shapes.Count; j++)
                    {
                        if(_shapes[i] != _shapes[j])
                        {
                            chainSize = 0;

                            if (!_relationPaths.ContainsKey(_shapes[i]))
                                _relationPaths.Add(_shapes[i], new Dictionary<ObjectShape, int>());

                            chain = GetRelationChain(_shapes[i], _shapes[j]);

                            if (chain != null && chain.Any())
                            {
                                chainSize = chain.Count;

                                if (chainSize > 1)
                                    chainSize -= 1;

                                if (chain[0] != _shapes[i])
                                    chainSize = -1 * chainSize;
                            }

                            if (_relationPaths[_shapes[i]].ContainsKey(_shapes[j]))
                                _relationPaths[_shapes[i]][_shapes[j]] = chainSize;
                            else
                                _relationPaths[_shapes[i]].Add(_shapes[j], chainSize);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates selectable properties.
        /// </summary>
        private void UpdatePropertyAttributes()
        {
            IEnumerable<Type> seaTypes = SearchableProperties;

            if (seaTypes == null)
                seaTypes = SearchSettings.DefaultSearchableProperties;

            foreach (ObjectShape s in _shapes)
            {
                foreach (ObjectProperty p in s.Properties)
                {
                    if (seaTypes.Any(t => p.Type.Equals(t) || t.IsAssignableFrom(p.Type)))
                        p.SetFlag(ObjectPropertyFlags.Searchable);

                    if (p.Type.IsEnum && p.Type.GetCustomAttributes(false).FirstOrDefault(a => a is FlagsAttribute) != null)
                        p.SetFlag(ObjectPropertyFlags.BitwiseComparison);
                }
            }
        }
    }
}