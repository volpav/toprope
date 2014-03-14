using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a table name parser.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    internal class TableParser<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets shape manager.
        /// </summary>
        private Metadata.ObjectShapeManager Shapes { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="shapes">Object shapes.</param>
        public TableParser(Metadata.ObjectShapeManager shapes)
        {
            Shapes = shapes;
        }

        /// <summary>
        /// Parses the table names.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Table names.</returns>
        public IList<Metadata.ObjectShape> Parse(IEnumerable<Condition> conditions)
        {
            Metadata.ObjectShape shape = null;
            List<Metadata.ObjectShape> ret = null;
            IEnumerable<Metadata.ObjectShape> chain = null;

            ret = ParseFromConditions(conditions).ToList();
            shape = Shapes.GetShapeByType(typeof(T));

            if (shape != null && !ret.Contains(shape))
                ret.Add(shape);

            if (ret.Count > 1)
            {
                ret.Sort((x, y) => { return Shapes.GetRelationDistance(x, y); });
                chain = Shapes.GetRelationChain(ret[0], ret[ret.Count - 1]);

                foreach (Metadata.ObjectShape s in chain)
                {
                    if (!ret.Contains(s))
                        ret.Add(s);
                }
            }

            ret = ret.Distinct().ToList();
            ret.Sort((x, y) => { return Shapes.GetRelationDistance(x, y); });

            return ret;
        }

        /// <summary>
        /// Parses shapes from conditions.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        private IList<Metadata.ObjectShape> ParseFromConditions(IEnumerable<Condition> conditions)
        {
            List<Metadata.ObjectShape> ret = new List<Metadata.ObjectShape>();

            if (conditions != null)
            {
                foreach (Condition c in conditions)
                {
                    if (!ret.Contains(c.Property.Parent))
                        ret.Add(c.Property.Parent);
                }
            }

            return ret;
        }

        /// <summary>
        /// Finds dependent tables by looking at object property types.
        /// </summary>
        /// <returns>Dependent tables.</returns>
        private IEnumerable<Metadata.ObjectShape> FindTablesByDependencies()
        {
            System.Type t = null;
            Metadata.ObjectShape shape = null;
            Metadata.ObjectShape otherShape = null;
            List<Metadata.ObjectShape> ret = new List<Metadata.ObjectShape>();

            if (Shapes != null)
            {
                shape = Shapes.GetShapeByType(typeof(T));

                if (shape != null)
                {
                    foreach (Metadata.ObjectProperty p in shape.Properties)
                    {
                        t = p.Type;

                        if (Utilities.CodeMeta.IsEnumerable(t))
                            t = Utilities.CodeMeta.GetEnumerableType(t);

                        otherShape = Shapes.GetShapeByType(t);

                        if (otherShape != null && !t.Equals(typeof(T)))
                            ret.Add(otherShape);
                    }
                }
            }

            return ret;
        }
    }
}