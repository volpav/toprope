using System;
using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a condition parser.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    internal class ConditionParser<T> : QueryVisitor
    {
        #region Properties

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        private IList<Condition> Conditions { get; set; }

        /// <summary>
        /// Gets or sets the number of free text conditions.
        /// </summary>
        private int FreeTextConditions { get; set; }

        /// <summary>
        /// Gets or sets the target shape.
        /// </summary>
        private Metadata.ObjectShape Shape { get; set; }

        /// <summary>
        /// Gets or sets the search field manager.
        /// </summary>
        public Metadata.SearchFieldManager Fields { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="searchEngine">Search engine.</param>
        public ConditionParser(SearchEngine searchEngine)
        {
            Conditions = new List<Condition>();

            if (searchEngine != null)
            {
                this.Fields = searchEngine.Fields;
                this.Shape = searchEngine.Shapes.GetShapeByType(typeof(T));
            }
        }

        /// <summary>
        /// Parses conditions.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <returns>Conditions.</returns>
        public IEnumerable<Condition> Parse(Query query)
        {
            Conditions.Clear();
            FreeTextConditions = 0;

            Visit(query);

            return Conditions;
        }

        /// <summary>
        /// Visits the given literal query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected override LiteralQueryNode VisitLiteral(LiteralQueryNode node)
        {
            if (node != null && !string.IsNullOrEmpty(node.Value))
            {
                foreach (Metadata.SearchField f in GetFields(string.Empty, null))
                {
                    AddCondition(f.Property, node.Value);
                    FreeTextConditions += 1;
                }
            }

            return base.VisitLiteral(node);
        }

        /// <summary>
        /// Visits the given single value parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected override SingleValueParameterQueryNode VisitSingleValueParameter(SingleValueParameterQueryNode node)
        {
            Metadata.SearchField guidField = null;

            if (node != null)
            {
                if (node.Value is Guid) // For GUIDs adding only one condition, no need to examine name, description, etc.
                {
                    guidField = GetFields(node.Name, typeof(Guid)).FirstOrDefault();

                    if (guidField != null)
                        AddCondition(guidField.Property, node.Value);
                }
                else
                {
                    foreach (Metadata.SearchField f in GetFields(node.Name, node.Value.GetType()))
                        AddCondition(f.Property, node.Value);
                }
            }

            return base.VisitSingleValueParameter(node);
        }

        /// <summary>
        /// Visits the given range parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected override RangeParameterQueryNode VisitRangeParameter(RangeParameterQueryNode node)
        {
            Type t = null;

            if (node != null && (node.LowerBound != null || node.UpperBound != null))
            {
                foreach (Metadata.SearchField f in GetFields(node.Name, null))
                {
                    t = f.Property != null ? f.Property.Type : null;
                    AddCondition(f.Property, new Range<object>(TransformValue(t, node.LowerBound), TransformValue(t, node.UpperBound)));
                }
            }

            return base.VisitRangeParameter(node);
        }

        /// <summary>
        /// Visits the given list parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected override ListParameterQueryNode VisitListParameter(ListParameterQueryNode node)
        {
            Type t = null;

            if (node != null)
            {
                foreach (Metadata.SearchField f in GetFields(node.Name, null))
                {
                    t = f.Property != null ? f.Property.Type : null;
                    AddCondition(f.Property, node.Values.Select(v => TransformValue(t, v)).AsEnumerable());
                }
            }

            return base.VisitListParameter(node);
        }

        /// <summary>
        /// Adds new condition.
        /// </summary>
        /// <param name="property">Object property.</param>
        /// <param name="value">Value.</param>
        private void AddCondition(Metadata.ObjectProperty property, object value)
        {
            if (property != null)
            {
                Conditions.Add(new Condition()
                {
                    Property = property,
                    Value = TransformValue(property.Type, value)
                });
            }
        }

        /// <summary>
        /// Returns fields for the given parameter name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="valueType">Value type.</param>
        /// <returns>Fields.</returns>
        private IEnumerable<Metadata.SearchField> GetFields(string name, Type valueType)
        {
            Type enumType = typeof(Enum);
            IEnumerable<Metadata.SearchField> ret = null;
            
            if (Fields != null)
                ret = Fields.GetFieldsByName(name);

            if (ret == null)
                ret = new List<Metadata.SearchField>().AsEnumerable();
            else if (valueType != null && ret.Any(f => !f.Property.Type.Equals(enumType) && !enumType.IsAssignableFrom(f.Property.Type)) &&
                ret.Any(f => !Infrastructure.Utilities.CodeMeta.IsEnumerable(f.Property.Type)))
            {
                ret = ret.Where(f => f.Property.Type.Equals(valueType) || Infrastructure.Utilities.CodeMeta.IsAssignableFrom(f.Property.Type, valueType));
            }

            return ret;
        }

        /// <summary>
        /// Transforms value.
        /// </summary>
        /// <param name="propertyType">Corresponding property type.</param>
        /// <param name="value">Value.</param>
        /// <returns>Transformed value.</returns>
        private object TransformValue(Type propertyType, object value)
        {
            object ret = value;

            if (ret != null && propertyType != null && propertyType.IsEnum)
            {
                try
                {
                    ret = (int)Enum.Parse(propertyType, ret.ToString(), true);
                }
                catch (ArgumentException) { }
            }

            return ret;
        }

        #region Static methods

        /// <summary>
        /// Filters the given conditions by removing those that exist on both current type and other shape types.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        public static void RemoveMatching(List<Condition> conditions)
        {
            int offset = 0;
            Type t = typeof(T);
            List<int> matchingIndexes = new List<int>();

            if (conditions != null && t != null && !t.Equals(typeof(object)))
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (!conditions[i].Property.Parent.Type.Equals(t) && conditions.Any(c => string.Compare(c.Property.Name, conditions[i].Property.Name, StringComparison.InvariantCultureIgnoreCase) == 0 && c.Property.Parent.Type.Equals(t)))
                        matchingIndexes.Add(i);

                    /*if (this.Shape != null)
                    {
                        if (!conditions[i].Property.Parent.Type.Equals(t) && !this.Shape.Properties.Any(p => string.Compare(p.Name, Conditions[i].Property.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                            matchingIndexes.Add(i);
                    }*/
                }

                if (matchingIndexes.Any())
                {
                    foreach (int index in matchingIndexes.Distinct())
                    {
                        conditions.RemoveAt(index - offset);
                        offset += 1;
                    }
                }
            }
        }

        #endregion
    }
}