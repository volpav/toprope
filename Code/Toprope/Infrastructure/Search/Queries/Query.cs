using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a strongly-typed search query.
    /// </summary>
    public class Query
    {
        #region Properties

        /// <summary>
        /// Gets the query nodes.
        /// </summary>
        public IList<QueryNode> Nodes { get; private set; }
        
        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Query() : this(new QueryNode[] { }) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="nodes">Query nodes.</param>
        public Query(IEnumerable<QueryNode> nodes) : this(nodes != null ? nodes.ToArray() : new QueryNode[] { }) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="nodes">Query nodes.</param>
        public Query(params QueryNode[] nodes)
        {
            this.Nodes = new List<QueryNode>();

            if (nodes != null)
            {
                foreach (QueryNode n in nodes.Where(node => node != null))
                    this.Nodes.Add(n);
            }
        }

        /// <summary>
        /// Returns the expected result type passed within the query.
        /// </summary>
        /// <returns>The expected result type passed within the query.</returns>
        public System.Type GetExpectedResultType()
        {
            System.Type ret = null;
            string typeName = string.Empty;
            SingleValueParameterQueryNode n = null;

            if (Nodes != null && Nodes.Any())
            {
                n = Nodes.OfType<SingleValueParameterQueryNode>()
                    .Where(c => string.Compare(c.Name, "in", System.StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();

                if (n != null)
                {
                    typeName = Utilities.Input.GetString(n.Value);

                    if (typeName.Length > 1 && typeName.EndsWith("s", System.StringComparison.InvariantCultureIgnoreCase))
                        typeName = typeName.Substring(0, typeName.Length - 1);

                    ret = Utilities.CodeMeta.FindType(typeName);
                }
            }

            return ret;
        }

        #region Static methods

        /// <summary>
        /// Parses the query from the higher level representation.
        /// </summary>
        /// <returns>Parsed query.</returns>
        public static Query Parse(SearchQuery query)
        {
            int currentCount = 0;
            Queries.Query ret = null;
            string[] valueComponents = null;
            object v = null, lb = null, ub = null;
            List<object> values = new List<object>();
            List<Queries.QueryNode> nodes = new List<Queries.QueryNode>();

            if (query != null && query.Criteria != null)
            {
                foreach (Criterion c in query.Criteria.Where(cr => cr != null && !string.IsNullOrWhiteSpace(cr.Value)))
                {
                    currentCount = nodes.Count;

                    if (string.IsNullOrWhiteSpace(c.Name))
                        nodes.Add(new Queries.LiteralQueryNode(c.Value));
                    else
                    {
                        if (c.Value.Trim().IndexOf(' ') > 0)
                        {
                            v = GetValue(c.Value);

                            if (v != null)
                                nodes.Add(new Queries.SingleValueParameterQueryNode(c.Name, v));
                        }
                        else
                        {
                            valueComponents = c.Value.Split(new char[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries);
                            
                            if (valueComponents != null && valueComponents.Length == 2 && c.Value.IndexOf(',') < 0 &&
                                c.Value.Where(s => s == '-').Count() == 1 && string.Compare(c.Name ?? string.Empty, "tags", System.StringComparison.InvariantCultureIgnoreCase) != 0)
                            {
                                lb = null;
                                ub = null;

                                if (string.Compare((valueComponents[0] ?? string.Empty).Trim(), "*") != 0)
                                    lb = GetValue(valueComponents[0]);

                                if (string.Compare((valueComponents[1] ?? string.Empty).Trim(), "*") != 0)
                                    ub = GetValue(valueComponents[1]);

                                if (lb != null || ub != null)
                                    nodes.Add(new Queries.RangeParameterQueryNode(c.Name, lb, ub));
                            }

                            if (currentCount == nodes.Count)
                            {
                                valueComponents = c.Value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                                if (valueComponents != null && valueComponents.Length > 1 &&
                                    string.Compare(c.Name ?? string.Empty, "location", System.StringComparison.InvariantCultureIgnoreCase) != 0)
                                {
                                    values.Clear();

                                    foreach (string vc in valueComponents)
                                    {
                                        v = GetValue(vc);

                                        if (v != null)
                                            values.Add(v);
                                    }

                                    if (values.Any())
                                        nodes.Add(new Queries.ListParameterQueryNode(c.Name, values));
                                }

                                if (currentCount == nodes.Count)
                                {
                                    v = GetValue(c.Value);

                                    if (v != null)
                                        nodes.Add(new Queries.SingleValueParameterQueryNode(c.Name, v));
                                }
                            }
                        }
                    }
                }
            }

            if (nodes.Any())
                ret = new Query(nodes);

            return ret;
        }

        /// <summary>
        /// Returns the actual value.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>The actual value.</returns>
        private static object GetValue(string value)
        {
            object ret = null;
            Models.RouteGrade grade = null;
            Models.Location location = null;

            if (value != null)
            {
                value = value.Trim();
                if (value.Length > 0)
                {
                    if (value.IndexOf(' ') > 0)
                        ret = value;
                    else
                    {
                        grade = Models.RouteGrade.Parse(value);
                        
                        if (grade != null)
                            ret = grade.Value;

                        if (ret == null)
                        {
                            location = Models.Location.Parse(value);

                            if (location != null)
                                ret = location;

                            if (ret == null)
                            {
                                if (Utilities.Input.IsBool(value))
                                    ret = Utilities.Input.GetBool(value);
                                else if (Utilities.Input.IsDouble(value))
                                    ret = Utilities.Input.GetDouble(value);
                                else if (Utilities.Input.IsInt(value))
                                    ret = Utilities.Input.GetInt(value);
                                else if (Utilities.Input.IsGuid(value))
                                    ret = Utilities.Input.GetGuid(value);
                                else
                                    ret = value;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        #endregion
    }
}