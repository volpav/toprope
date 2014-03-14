using System;
using System.Collections.Generic;
using System.Linq;

namespace Toprope.Infrastructure.Search.Specialized
{
    /// <summary>
    /// Represents a database search engine. This class cannot be inherited.
    /// </summary>
    public sealed class DbSearchEngine : SearchEngine
    {
        #region Nested types

        /// <summary>
        /// Represents a search engine whose object shapes and fields are cached.
        /// </summary>
        private sealed class CachedSearchEngine : SearchEngine
        {
            #region Properties

            private static CachedSearchEngine _current = new CachedSearchEngine();

            /// <summary>
            /// Gets the current instnace of the search engine.
            /// </summary>
            public static CachedSearchEngine Current
            {
                get { return _current; }
            }

            #endregion

            /// <summary>
            /// Initializes a new instance of an object.
            /// </summary>
            private CachedSearchEngine() { }

            /// <summary>
            /// Performs a search.
            /// </summary>
            /// <typeparam name="T">Search result type.</typeparam>
            /// <param name="query">Search query.</param>
            /// <returns>Search result.</returns>
            public override SearchResult<T> Search<T>(SearchQuery query)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Creates native query builder.
            /// </summary>
            /// <typeparam name="T">Result type.</typeparam>
            /// <returns>Native query builder.</returns>
            protected override Queries.NativeQueryBuilder<T> CreateQueryBuilder<T>()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Returns matching storage records by using information from the native query builder.
            /// </summary>
            /// <typeparam name="T">Result type.</typeparam>
            /// <param name="builder">Query builder.</param>
            /// <returns>Matching documents.</returns>
            protected override IList<IDictionary<string, object>> GetMatchingRecords<T>(Queries.NativeQueryBuilder<T> builder)
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        private Storage.Repository Repository { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether custom repository is used.
        /// </summary>
        private bool IsCustomRepository { get; set; }

        /// <summary>
        /// Gets object shapes.
        /// </summary>
        public override Metadata.ObjectShapeManager Shapes
        {
            get { return CachedSearchEngine.Current.Shapes; }
        }

        /// <summary>
        /// Gets search fields.
        /// </summary>
        public override Metadata.SearchFieldManager Fields
        {
            get { return CachedSearchEngine.Current.Fields; }
        }

        /// <summary>
        /// Gets transforms.
        /// </summary>
        public override Transformation.TransformationManager Transforms
        {
            get { return CachedSearchEngine.Current.Transforms; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public DbSearchEngine() : this(null) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="repository">Repository.</param>
        public DbSearchEngine(Storage.Repository repository)
        {
            Repository = repository;

            if (Repository == null)
            {
                Repository = new Storage.Repository();
                IsCustomRepository = true;
            }
        }

        /// <summary>
        /// Enables search in entities of a given types.
        /// </summary>
        /// <param name="settings">Search settings.</param>
        public override void EnableSearch(Metadata.SearchSettings settings)
        {
            // Caching settings application-wise
            CachedSearchEngine.Current.EnableSearch(settings);
        }

        /// <summary>
        /// Creates native query builder.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <returns>Native query builder.</returns>
        protected override Queries.NativeQueryBuilder<T> CreateQueryBuilder<T>()
        {
            return new Specialized.SqlQueryBuilder<T>(this);
        }

        /// <summary>
        /// Returns matching storage records by using information from the native query builder.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="builder">Query builder.</param>
        /// <returns>Matching documents.</returns>
        protected override IList<IDictionary<string, object>> GetMatchingRecords<T>(Queries.NativeQueryBuilder<T> builder)
        {
            Specialized.SqlQueryBuilder<T> b = null;
            List<IDictionary<string, object>> ret = new List<IDictionary<string, object>>();

            if (builder != null && builder is Specialized.SqlQueryBuilder<T>)
            {
                b = builder as Specialized.SqlQueryBuilder<T>;

                if (Repository != null)
                {
                    using (System.Data.IDataReader reader = Repository.Select(b.Text, b.Parameters.ToArray()))
                    {
                        while (reader.Read())
                            ret.Add(Storage.Repository.Read(reader));
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Disposes search engine resources.
        /// </summary>
        public override void Dispose()
        {
            if (IsCustomRepository && Repository != null)
            {
                Repository.Dispose();
                Repository = null;
            }
        }

        #region Static methods

        /// <summary>
        /// Enables search on domain objects.
        /// </summary>
        public static void Initialize()
        {
            DbSearchEngine se = new DbSearchEngine();
            Metadata.SearchSettings settings = new Metadata.SearchSettings();

            settings.Entities = new HashSet<Type>(new Type[] { 
                typeof(Models.Area), 
                typeof(Models.Sector), 
                typeof(Models.Route) 
            });

            settings.SearchableProperties.Add(typeof(Models.RouteGrade));
            settings.SearchableProperties.Add(typeof(Models.Location));

            se.EnableSearch(settings);
        }

        #endregion
    }
}