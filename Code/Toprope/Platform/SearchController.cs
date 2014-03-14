using Toprope.Infrastructure.Search;
using Toprope.Infrastructure.Storage;
using Toprope.Models;

namespace Toprope.Platform
{
    /// <summary>
    /// Represents a search controller.
    /// </summary>
    public class SearchController
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchController() { }

        /// <summary>
        /// Searches the repository for areas.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public SearchResult<Area> GetAreas(SearchQuery query)
        {
            return GetSearchResults<Area>(query);
        }

        /// <summary>
        /// Searches the repository for sectors.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public SearchResult<Sector> GetSectors(SearchQuery query)
        {
            return GetSearchResults<Sector>(query);
        }

        /// <summary>
        /// Searches the repository for routes.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public SearchResult<Route> GetRoutes(SearchQuery query)
        {
            return GetSearchResults<Route>(query);
        }

        /// <summary>
        /// Searches the repository.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        private SearchResult<T> GetSearchResults<T>(SearchQuery query) where T: Infrastructure.Storage.DbRecord, new()
        {
            SearchResult<T> ret = null;
            string cacheKey = string.Empty;

            if (query != null)
            {
                cacheKey = string.Format("{0}.{1}", typeof(T).Name, query.ToString());

                if (Infrastructure.Application.Current.QueryCache.ContainsKey(cacheKey))
                    ret = Infrastructure.Application.Current.QueryCache[cacheKey] as SearchResult<T>;
                else
                {
                    using (Repository repo = new Repository())
                        ret = repo.Search<T>(query);

                    if (!Infrastructure.Application.Current.QueryCache.ContainsKey(cacheKey))
                        Infrastructure.Application.Current.QueryCache.Add(cacheKey, ret);
                }
            }

            if (ret == null)
                ret = new SearchResult<T>();

            return ret;
        }
    }
}