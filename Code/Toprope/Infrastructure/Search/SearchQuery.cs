using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Toprope.Infrastructure.Search
{
    /// <summary>
    /// Represents a search query.
    /// </summary>
    public class SearchQuery
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of query criteria.
        /// </summary>
        public IList<Criterion> Criteria { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchQuery() : this(null) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="criteria">Search criteria.</param>
        public SearchQuery(IEnumerable<Criterion> criteria) : this(criteria != null ? criteria.ToArray() : null) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="criteria">Search criteria.</param>
        public SearchQuery(params Criterion[] criteria)
        {
            Criteria = new List<Criterion>();

            if (criteria != null)
            {
                foreach (Criterion c in criteria)
                {
                    if (c != null && !string.IsNullOrEmpty(c.Value))
                        Criteria.Add(new Criterion(c));
                }
            }
        }

        /// <summary>
        /// Returns the expected result type passed within the query.
        /// </summary>
        /// <returns>The expected result type passed within the query.</returns>
        public System.Type GetExpectedResultType()
        {
            Criterion n = null;
            System.Type ret = null;
            string typeName = string.Empty;

            if (Criteria != null && Criteria.Any())
            {
                n = Criteria
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

        /// <summary>
        /// Adds or updates criterion with the given name.
        /// </summary>
        /// <param name="name">Criterion name.</param>
        /// <param name="value">Criterion value.</param>
        /// <returns>New search query.</returns>
        public SearchQuery WithCriterion(string name, string value)
        {
            int index = -1;
            SearchQuery ret = new SearchQuery(Criteria);

            if (name != null && !string.IsNullOrEmpty(value))
            {
                if (ret.Criteria == null)
                    ret.Criteria = new List<Criterion>();

                for (int i = 0; i < ret.Criteria.Count; i++)
                {
                    if (ret.Criteria[i] != null && string.Compare(ret.Criteria[i].Name ?? string.Empty, name, System.StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0)
                    ret.Criteria.Add(new Criterion(name, value));
                else
                    ret.Criteria[index].Value = value;
            }

            return ret;
        }

        /// <summary>
        /// Removes criterion with the given name.
        /// </summary>
        /// <param name="name">Criterion name.</param>
        /// <returns>New search query.</returns>
        public SearchQuery WithoutCriterion(string name)
        {
            int index = -1;
            SearchQuery ret = new SearchQuery(Criteria);

            if (name != null && ret.Criteria != null)
            {
                for (int i = 0; i < ret.Criteria.Count; i++)
                {
                    if (ret.Criteria[i] != null && string.Compare(ret.Criteria[i].Name ?? string.Empty, name, System.StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                    ret.Criteria.RemoveAt(index);
            }

            return ret;
        }

        /// <summary>
        /// Returns a string representation of the current query.
        /// </summary>
        /// <returns>A string representation of the current query.</returns>
        public override string ToString()
        {
            return Criteria == null ? string.Empty :
                string.Join(" ", Criteria.Where(c => c != null && c.Name != null && !string.IsNullOrEmpty(c.Value)).Select(c => c.Name.Length > 0 ? string.Format("{0}:{1}", c.Name, c.Value.IndexOf(' ') > 0 ? string.Format("\"{0}\"", c.Value) : c.Value) : c.Value));
        }

        #region Static methods

        /// <summary>
        /// Parses the given search query from its raw representation.
        /// </summary>
        /// <param name="query">Raw query.</param>
        /// <returns>Search query.</returns>
        public static SearchQuery Parse(string query)
        {
            Match m = null;
            Criterion c = null;
            SearchQuery ret = null;
            string currentQuery = query;
            
            if (!string.IsNullOrEmpty(currentQuery))
            {
                ret = new SearchQuery();

                while((m = Regex.Match(currentQuery, "([a-zA-Z0-9]+):((\"[^\"]+\")|([^\\s]+))", RegexOptions.IgnoreCase)).Success)
                {
                    c = new Criterion();

                    c.Name = m.Groups[1].Value;
                    c.Value = (m.Groups[2].Value ?? string.Empty).Trim().Trim('\"');

                    if(!string.IsNullOrEmpty(c.Value))
                        ret.Criteria.Add(c);

                    currentQuery = currentQuery.Remove(m.Index, m.Length);
                }

                currentQuery = currentQuery.Trim();

                if (!string.IsNullOrEmpty(currentQuery))
                    ret.Criteria.Add(new Criterion(string.Empty, currentQuery));
            }

            return ret;
        }

        #endregion
    }
}