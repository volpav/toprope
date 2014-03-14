using System;
using System.Linq;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Search
{
    /// <summary>
    /// Represents a search engine.
    /// </summary>
    public abstract class SearchEngine : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets object shapes.
        /// </summary>
        public virtual Metadata.ObjectShapeManager Shapes { get; private set; }

        /// <summary>
        /// Gets search fields.
        /// </summary>
        public virtual Metadata.SearchFieldManager Fields { get; private set; }

        /// <summary>
        /// Gets transforms.
        /// </summary>
        public virtual Transformation.TransformationManager Transforms { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected SearchEngine()
        {
            Shapes = new Metadata.ObjectShapeManager();
            Fields = new Metadata.SearchFieldManager();
            Transforms = new Transformation.TransformationManager();
        }

        /// <summary>
        /// Enables search in entities of a given types.
        /// </summary>
        /// <param name="settings">Search settings.</param>
        public virtual void EnableSearch(Metadata.SearchSettings settings)
        {
            if(settings != null && settings.Entities != null)
            {
                Shapes.SearchableProperties = settings.SearchableProperties;

                Shapes.RegisterMultipleShapes(settings.Entities.Where(t => t != null));
                Fields.RegisterMultipleFields(Shapes.Shapes);
                Transforms.RegisterMultipleMaterializers(settings.Materializers);
            }
        }

        /// <summary>
        /// Disposes resources used by this search engine.
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// Performs a search.
        /// </summary>
        /// <typeparam name="T">Search result type.</typeparam>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public virtual SearchResult<T> Search<T>(SearchQuery query) where T : new()
        {
            Queries.Query q = null;
            Queries.Range<int> range = null;
            SearchResult<T> ret = new SearchResult<T>();
            Queries.NativeQueryBuilder<T> builder = null;
            IList<IDictionary<string, object>> data = null;
            IEnumerable<Queries.Condition> conditions = null;
            Transformation.ObjectMaterializer materializer = null;

            if (query != null)
            {
                q = Queries.Query.Parse(query);

                if (q != null && q.Nodes != null && q.Nodes.Any())
                {
                    builder = CreateQueryBuilder<T>();

                    if (builder != null)
                    {
                        conditions = new Queries.ConditionParser<T>(this).Parse(q);
                        if (conditions != null)
                            builder.Where(conditions);

                        range = new Queries.RangeParser().Parse(q);
                        if (range != null)
                            builder.Limit(range);

                        if (builder.Complete)
                        {
                            data = GetMatchingRecords<T>(builder);

                            if (data != null && data.Any())
                            {
                                if (data[0].ContainsKey("_total"))
                                    ret.Total = Utilities.Input.GetInt(data[0]["_total"]);

                                materializer = Transforms.GetMaterializerByType<T>();

                                if (materializer == null)
                                    materializer = Activator.CreateInstance(typeof(Transformation.GenericObjectMaterializer<>).MakeGenericType(typeof(T))) as Transformation.ObjectMaterializer;

                                if (materializer != null)
                                    ret.Items = materializer.Materialize(data).OfType<T>().ToList();
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Creates native query builder.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <returns>Native query builder.</returns>
        protected abstract Queries.NativeQueryBuilder<T> CreateQueryBuilder<T>();

        /// <summary>
        /// Returns matching storage records by using information from the native query builder.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="builder">Query builder.</param>
        /// <returns>Matching documents.</returns>
        protected abstract IList<IDictionary<string, object>> GetMatchingRecords<T>(Queries.NativeQueryBuilder<T> builder);
    }
}