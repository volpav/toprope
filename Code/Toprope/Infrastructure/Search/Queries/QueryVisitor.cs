using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Queries
{
    /// <summary>
    /// Represents a query visitor.
    /// </summary>
    public abstract class QueryVisitor
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected QueryVisitor() { }

        /// <summary>
        /// Visits the given query.
        /// </summary>
        /// <param name="query">Query to visit.</param>
        /// <returns>Either the same or a different query.</returns>
        protected virtual Query Visit(Query query)
        {
            Query ret = query;
            QueryNode visited = null;
            bool nodesChanged = false;
            List<QueryNode> nodes = new List<QueryNode>();

            if (query != null)
            {
                if (query.Nodes.Any())
                {
                    foreach (QueryNode n in query.Nodes)
                    {
                        if (n != null)
                        {
                            visited = VisitNode(n);

                            if (visited != null)
                                nodes.Add(visited);
                            else
                                nodesChanged = true;
                        }
                        else
                            nodesChanged = true;
                    }

                    if (!nodesChanged)
                    {
                        nodesChanged = nodes.Count != query.Nodes.Count;

                        if (!nodesChanged)
                        {
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                nodesChanged = nodes[i] != query.Nodes[i];
                                if (nodesChanged)
                                    break;
                            }
                        }
                    }

                    if (nodesChanged)
                        ret = new Query(nodes);
                }
            }

            return ret;
        }

        /// <summary>
        /// Visits the given query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected virtual QueryNode VisitNode(QueryNode node)
        {
            QueryNode ret = node;

            if (node != null)
            {
                switch (node.Type)
                {
                    case QueryNodeType.Literal:
                        ret = VisitLiteral(node as LiteralQueryNode);
                        break;
                    case QueryNodeType.Parameter:
                        ret = VisitParameter(node as ParameterQueryNode);
                        break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Visits the given literal query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected virtual LiteralQueryNode VisitLiteral(LiteralQueryNode node)
        {
            return node;
        }

        /// <summary>
        /// Visits the given parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected virtual ParameterQueryNode VisitParameter(ParameterQueryNode node)
        {
            ParameterQueryNode ret = node;

            if (node != null)
            {
                switch (node.ParameterType)
                {
                    case ParameterQueryNodeType.SingleValue:
                        ret = VisitSingleValueParameter(node as SingleValueParameterQueryNode);
                        break;
                    case ParameterQueryNodeType.Range:
                        ret = VisitRangeParameter(node as RangeParameterQueryNode);
                        break;
                    case ParameterQueryNodeType.List:
                        ret = VisitListParameter(node as ListParameterQueryNode);
                        break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Visits the given single value parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected virtual SingleValueParameterQueryNode VisitSingleValueParameter(SingleValueParameterQueryNode node)
        {
            return node;
        }

        /// <summary>
        /// Visits the given range parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected virtual RangeParameterQueryNode VisitRangeParameter(RangeParameterQueryNode node)
        {
            return node;
        }

        /// <summary>
        /// Visits the given list parameter query node.
        /// </summary>
        /// <param name="node">Query node to visit.</param>
        /// <returns>Either the same or a different query node.</returns>
        protected virtual ListParameterQueryNode VisitListParameter(ListParameterQueryNode node)
        {
            return node;
        }
    }
}