using System;
using System.Collections.Generic;
using System.Linq;

using Toprope.Infrastructure.Search.Queries;

namespace Toprope.Infrastructure.Search.Specialized
{
    /// <summary>
    /// Represents an SQL query builder.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    public class SqlQueryBuilder<T> : NativeQueryBuilder<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the WHERE query text.
        /// </summary>
        private System.Text.StringBuilder WhereQueryText { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        private List<Storage.Parameter> QueryParameters { get; set; }

        /// <summary>
        /// Gets or sets the object shapes.
        /// </summary>
        private Metadata.ObjectShapeManager Shapes { get; set; }

        /// <summary>
        /// Gets or sets the variables.
        /// </summary>
        private HashSet<string> Variables { get; set; }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        private Range<int> Range { get; set; }

        /// <summary>
        /// Gets the SQL statement text generated so far.
        /// </summary>
        public string Text
        {
            get { return GetCompleteQueryText(); }
        }

        /// <summary>
        /// Gets the list of query parameters added so far.
        /// </summary>
        public IEnumerable<Storage.Parameter> Parameters
        {
            get { return QueryParameters.AsEnumerable(); }
        }

        /// <summary>
        /// Gets value indicating whether an appropriate query has been built.
        /// </summary>
        public override bool Complete
        {
            get { return WhereQueryText.Length > 0; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="searchEngine">Search engine.</param>
        public SqlQueryBuilder(SearchEngine searchEngine)
        {
            WhereQueryText = new System.Text.StringBuilder();
            QueryParameters = new List<Storage.Parameter>();
            Variables = new HashSet<string>();

            if (searchEngine != null)
                Shapes = searchEngine.Shapes;
        }

        /// <summary>
        /// Resets the builder.
        /// </summary>
        public override void Reset()
        {
            WhereQueryText.Clear();
            QueryParameters.Clear();

            Range = null;
        }

        /// <summary>
        /// Appends the WHERE statement.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>Builder.</returns>
        public override NativeQueryBuilder<T> Where(IEnumerable<Condition> conditions)
        {
            Type t = typeof(T);
            IList<Metadata.ObjectShape> tables = null;

            WhereQueryText.Clear();
            Variables.Clear();

            if (conditions != null && conditions.Any())
            {
                tables = new TableParser<T>(Shapes).Parse(conditions);

                if (tables != null && tables.Any())
                    WhereQueryText.Append(BuildExists(conditions, tables));
            }
            
            return this;
        }

        /// <summary>
        /// Limits the number of records to be selected.
        /// </summary>
        /// <param name="range">Range.</param>
        /// <returns>Builder.</returns>
        public override NativeQueryBuilder<T> Limit(Range<int> range)
        {
            this.Range = range;

            if (this.Range != null && this.Range.From <= 0 && this.Range.To <= 0)
                this.Range = null;
            
            return this;
        }

        /// <summary>
        /// Converts the given conditions to SQL statement.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <returns>SQL text.</returns>
        private string ConditionsToSql(IEnumerable<Condition> conditions)
        {
            int written = 0;
            string conjunction = " OR ";
            System.Text.StringBuilder ret = new System.Text.StringBuilder();

            if (conditions != null && conditions.Any())
            {
                foreach (Condition c in conditions)
                {
                    if (c != null)
                    {
                        ret.Append(ConditionToSql(c));
                        ret.Append(conjunction);

                        written += 1;
                    }
                }

                if (written > 0)
                    ret.Remove(ret.Length - conjunction.Length, conjunction.Length);
            }

            return ret.ToString();
        }

        /// <summary>
        /// Converts the given condition to SQL statement.
        /// </summary>
        /// <param name="c">Condition.</param>
        /// <returns>SQL text.</returns>
        private string ConditionToSql(Condition c)
        {
            int written = 0;
            Range<object> r = null;
            Storage.Parameter p = null;
            string conjunction = " OR ";
            System.Text.StringBuilder ret = new System.Text.StringBuilder();
            
            if (c != null && c.Value != null)
            {
                if (c.Value is Range<object>)
                {
                    r = c.Value as Range<object>;

                    if (r.From != null || r.To != null)
                    {
                        ret.Append("(");

                        if (r.From != null)
                        {
                            p = new Storage.Parameter(GetParameterName(c.Property, 0), r.From);
                            QueryParameters.Add(p);

                            ret.AppendFormat("{0}.[{1}] >= @{2}", c.Property.Parent.Alias, c.Property.Name, p.Name);
                        }

                        if (r.From != null && r.To != null)
                            ret.Append(" AND ");

                        if (r.To != null)
                        {
                            p = new Storage.Parameter(GetParameterName(c.Property, 1), r.To);
                            QueryParameters.Add(p);

                            ret.AppendFormat("{0}.[{1}] <= @{2}", c.Property.Parent.Alias, c.Property.Name, p.Name);
                        }

                        ret.Append(")");
                    }
                }
                else if (c.Value is IEnumerable<object>)
                {
                    ret.Append("(");

                    foreach (object v in (c.Value as IEnumerable<object>))
                    {
                        ret.Append(ConditionToSql(c.Property, v, written));
                        ret.Append(conjunction);

                        written += 1;
                    }

                    if (written > 0)
                        ret.Remove(ret.Length - conjunction.Length, conjunction.Length);

                    ret.Append(")");
                }
                else
                    ret.Append(ConditionToSql(c.Property, c.Value));
            }

            return ret.ToString();
        }

        /// <summary>
        /// Converts the given condition to SQL statement.
        /// </summary>
        /// <param name="property">Property.</param>
        /// <param name="value">Value.</param>
        /// <returns>SQL text.</returns>
        private string ConditionToSql(Metadata.ObjectProperty property, object value)
        {
            return ConditionToSql(property, value, -1);
        }
        
        /// <summary>
        /// Converts the given condition to SQL statement.
        /// </summary>
        /// <param name="property">Property.</param>
        /// <param name="value">Value.</param>
        /// <param name="index">Value index.</param>
        /// <returns>SQL text.</returns>
        private string ConditionToSql(Metadata.ObjectProperty property, object value, int index)
        {
            Models.Location loc = null;
            string[] components = null;
            string pName = string.Empty;
            Type valueType = property.Type;
            System.Globalization.CultureInfo culture = null;
            string parameterName = GetParameterName(property, index);
            System.Text.StringBuilder ret = new System.Text.StringBuilder();
            List<Tuple<string, object>> parameters = new List<Tuple<string, object>>();
            
            if (value != null)
            {
                if (Utilities.CodeMeta.IsEnumerable(valueType))
                    valueType = Utilities.CodeMeta.GetEnumerableType(valueType);

                if (valueType.Equals(typeof(string)))
                {
                    value = Utilities.Input.GetString(value);
                    components = (value as string).Split(new char[] { ' ', ',', '.', ':', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    if (components.Length > 1)
                    {
                        if (components.Count(c => c.Length > 2) > 0)
                            components = components.Where(c => c.Length > 2).ToArray();

                        ret.Append("(");
                        for (int i = 0; i < components.Length; i++)
                        {
                            pName = string.Format("{0}_{1}", GetParameterName(property, index), i);
                            ret.AppendFormat("CONTAINS({0}.[{1}], @{2})", property.Parent.Alias, property.Name, pName);

                            parameters.Add(new Tuple<string, object>(pName, components[i]));

                            if (i < (components.Length - 1))
                                ret.Append(" AND ");
                        }
                        ret.Append(")");
                    }
                    else
                        ret.AppendFormat("CONTAINS({0}.[{1}], @{2})", property.Parent.Alias, property.Name, parameterName);
                }
                else if (property.HasFlag(Metadata.ObjectPropertyFlags.BitwiseComparison))
                    ret.AppendFormat("({0}.[{1}] IS NOT NULL AND {0}.[{1}] <> 0 AND ({0}.[{1}] & @{2}) = @{2})", property.Parent.Alias, property.Name, parameterName);
                else if (valueType.Equals(typeof(Models.Location)) && value is Models.Location)
                {
                    loc = value as Models.Location;
                    culture = new System.Globalization.CultureInfo("en-US");

                    // 10 kilometers from the given point
                    ret.AppendFormat("({0}.[{1}] IS NOT NULL AND {0}.[{1}].STDistance(geography::STGeomFromText('POINT({2} {3})', 4326)) <= 10000)", 
                        property.Parent.Alias, property.Name, loc.Longitude.ToString(culture), loc.Latitude.ToString(culture));
                }
                else
                    ret.AppendFormat("{0}.[{1}] = @{2}", property.Parent.Alias, property.Name, parameterName);

                if (loc == null)
                {
                    if (parameters.Any())
                    {
                        foreach (Tuple<string, object> p in parameters)
                            QueryParameters.Add(new Storage.Parameter(p.Item1, p.Item2));
                    }
                    else
                        QueryParameters.Add(new Storage.Parameter(parameterName, value));

                }
            }

            return ret.ToString();
        }

        /// <summary>
        /// Returns a complete query text.
        /// </summary>
        /// <returns></returns>
        private string GetCompleteQueryText()
        {
            Metadata.ObjectShape shape = Shapes.GetShapeByType(typeof(T));
            System.Text.StringBuilder ret = new System.Text.StringBuilder();

            if (shape != null && WhereQueryText.Length > 0)
            {
                ret.Append("SELECT * FROM (");
                ret.Append("SELECT *, MAX([_row]) OVER (PARTITION BY [_partition]) AS [_total] FROM (");

                ret.Append("SELECT *, 1 AS [_partition], ROW_NUMBER() OVER (ORDER BY [Id]) AS [_row] FROM (");

                ret.Append(WhereQueryText.ToString());

                ret.Append(") t1");
                ret.Append(") t0");
                ret.Append(") t");

                if (Range != null && (Range.From > 0 || Range.To > 0))
                {
                    ret.Append(" WHERE ");

                    ret.AppendFormat("t.[_row] >= {0}", Range.From > 0 ? Range.From : 1);
                        
                    if (Range.To > 0)
                        ret.AppendFormat(" AND t.[_row] <= {0}", Range.To);
                }
            }

            return ret.ToString();
        }

        /// <summary>
        /// Returns parameter name.
        /// </summary>
        /// <param name="property">Object property.</param>
        /// <returns>Parameter name.</returns>
        private string GetParameterName(Metadata.ObjectProperty property)
        {
            return GetParameterName(property, -1);
        }

        /// <summary>
        /// Returns parameter name.
        /// </summary>
        /// <param name="property">Object property.</param>
        /// <param name="index">Parameter index.</param>
        /// <returns>Parameter name.</returns>
        private string GetParameterName(Metadata.ObjectProperty property, int index)
        {
            string ret = string.Empty;

            if (property != null)
            {
                ret = string.Format("p_{0}{1}", property.Alias, index >= 0 ? index.ToString() : string.Empty);

                if (!Variables.Contains(ret))
                    Variables.Add(ret);
                else
                    ret = GetParameterName(property, index + 1);
            }

            return ret;
        }

        /// <summary>
        /// Returns EXISTS sub-query.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="shapes">Selectable shapes.</param>
        /// <returns>EXISTS sub-query.</returns>
        private string BuildExists(IEnumerable<Condition> conditions, IList<Metadata.ObjectShape> shapes)
        {
            string ret = string.Empty;
            int r = 1, shapeIndex = -1, currentCount = 0;
            Metadata.ObjectShape s = Shapes.GetShapeByType(typeof(T));
            List<Metadata.ObjectShape> reorderedShapes = new List<Metadata.ObjectShape>();

            if (shapes != null && shapes.Any())
            {
                shapeIndex = shapes.IndexOf(s);
                
                if (shapeIndex >= 0)
                {
                    while (true)
                    {
                        currentCount = reorderedShapes.Count;

                        if (shapeIndex - r >= 0)
                            reorderedShapes.Add(shapes[shapeIndex - r]);

                        if (shapeIndex + r < shapes.Count)
                            reorderedShapes.Add(shapes[shapeIndex + r]);

                        if (currentCount == reorderedShapes.Count)
                            break;
                        else
                            r++;
                    }

                    reorderedShapes.Insert(0, shapes[shapeIndex]);

                    ret = ExistsRecursive(conditions, reorderedShapes, 0);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns EXISTS sub-query.
        /// </summary>
        /// <param name="conditions">Conditions.</param>
        /// <param name="shapes">Selectable shapes.</param>
        /// <param name="currentShapeIndex">Current shape index.</param>
        /// <returns>EXISTS sub-query.</returns>
        private string ExistsRecursive(IEnumerable<Condition> conditions, IList<Metadata.ObjectShape> shapes, int currentShapeIndex)
        {
            int index = 0;
            bool hasInnerFiltered = false;
            bool hasSelfConditions = false;
            string innerExists = string.Empty;
            Tuple<string, string> relation = null;
            string innerConditions = string.Empty;
            List<Condition> filtered = new List<Condition>();

            System.Action<int> processFiltered = (i) =>
                {
                    if (i >= 0 && i < shapes.Count)
                    {
                        filtered.AddRange(conditions);
                        ConditionParser<T>.RemoveMatching(filtered);

                        filtered = filtered.Where(c => c.Property.Parent == shapes[i]).ToList();
                    }
                    else
                        filtered.Clear();
                };

            System.Text.StringBuilder ret = new System.Text.StringBuilder();
            
            if (conditions != null && shapes != null && shapes.Any() && currentShapeIndex >= 0 && currentShapeIndex < shapes.Count)
            {
                if (currentShapeIndex == 0)
                        ret.AppendFormat("SELECT * FROM [{0}] AS {1}", shapes[currentShapeIndex].Name, shapes[currentShapeIndex].Alias);

                if (!shapes[currentShapeIndex].Type.Equals(typeof(T)))
                    processFiltered(currentShapeIndex);
                else
                    filtered = conditions.Where(c => c.Property.Parent == shapes[currentShapeIndex]).ToList();
                
                if (filtered.Any())
                {
                    if (currentShapeIndex != 0)
                        hasSelfConditions = true;

                    innerConditions = ConditionsToSql(filtered);

                    if (!string.IsNullOrEmpty(innerConditions))
                    {
                        hasSelfConditions = true;

                        if (currentShapeIndex == 0)
                            ret.AppendFormat(" WHERE ({0})", innerConditions);
                        else
                            ret.AppendFormat(" AND ({0})", innerConditions);
                    }
                }

                if (currentShapeIndex < (shapes.Count - 1))
                {
                    relation = GetRelation(shapes[currentShapeIndex], shapes[currentShapeIndex + 1]);
                    if (relation == null)
                    {
                        relation = GetRelation(shapes[currentShapeIndex + 1], shapes[currentShapeIndex]);
                        if (relation != null)
                            relation = new Tuple<string, string>(relation.Item2, relation.Item1);
                    }

                    if (relation != null)
                    {
                        hasInnerFiltered = false;
                        index = currentShapeIndex + 1;

                        while (index < shapes.Count)
                        {
                            processFiltered(index++);

                            if (filtered.Any())
                            {
                                hasInnerFiltered = true;
                                break;
                            }
                        }

                        if (hasInnerFiltered)
                        {
                            if (hasSelfConditions || currentShapeIndex > 0)
                                ret.Append(" AND ");
                            else
                                ret.Append(" WHERE ");

                            ret.AppendFormat("EXISTS (SELECT * FROM [{0}] {1} WHERE {1}.[{2}] = {3}.[{4}]",
                                shapes[currentShapeIndex + 1].Name, shapes[currentShapeIndex + 1].Alias, relation.Item1, shapes[currentShapeIndex].Alias, relation.Item2);

                            ret.Append(ExistsRecursive(conditions, shapes, currentShapeIndex + 1));

                            ret.Append(")");
                        }
                    }
                }
            }

            return ret.ToString();
        }

        /// <summary>
        /// Gets relation between the given objects.
        /// </summary>
        /// <param name="mainObject">Main object.</param>
        /// <param name="dependentObject">Dependent object.</param>
        /// <returns>Relation.</returns>
        private Tuple<string, string> GetRelation(Metadata.ObjectShape mainObject, Metadata.ObjectShape dependentObject)
        {
            Tuple<string, string> ret = null;

            if (mainObject != null && dependentObject != null)
            {
                foreach (Metadata.ObjectProperty p in dependentObject.Properties)
                {
                    if (p.Relation != null && p.Relation.Shape == mainObject)
                    {
                        ret = new Tuple<string, string>(p.Name.EndsWith("Id") ? p.Name : string.Format("{0}Id", p.Name), "Id");
                        break;
                    }
                }
            }

            if (ret != null && (string.IsNullOrEmpty(ret.Item1) || string.IsNullOrEmpty(ret.Item2)))
                ret = null;

            return ret;
        }
    }
}