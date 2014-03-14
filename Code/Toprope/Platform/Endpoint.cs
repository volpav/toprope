using System;
using System.Collections.Generic;
using Toprope.Models;

namespace Toprope.Platform
{
    /// <summary>
    /// Represents a platform endpoint. This class cannot be inherited.
    /// </summary>
    public static class Endpoint
    {
        #region Areas

        /// <summary>
        /// Returns the area by its Id.
        /// </summary>
        /// <param name="areaId">Area Id.</param>
        /// <returns>A area by its Id.</returns>
        public static Area GetArea(Guid areaId)
        {
            return new AreasController().GetArea(areaId.ToString());
        }

        /// <summary>
        /// Searches the repository for areas.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public static Infrastructure.Search.SearchResult<Area> SearchAreas(Infrastructure.Search.SearchQuery query)
        {
            return new SearchController().GetAreas(query);
        }

        #endregion

        #region Sectors

        /// <summary>
        /// Returns the sector by its Id.
        /// </summary>
        /// <param name="sectorId">Sector Id.</param>
        /// <returns>A sector by its Id.</returns>
        public static Sector GetSector(Guid sectorId)
        {
            return new SectorsController().GetSector(sectorId.ToString());
        }

        /// <summary>
        /// Returns all images for a given sector.
        /// </summary>
        /// <param name="sectorId">Sector Id.</param>
        /// <returns>All images for a given sector.</returns>
        public static IEnumerable<Image> GetSectorImages(Guid sectorId)
        {
            return new SectorsController().GetSectorImages(sectorId.ToString());
        }

        /// <summary>
        /// Returns all routes for a given sector.
        /// </summary>
        /// <param name="sectorId">Sector Id.</param>
        /// <returns>All routes for a given sector.</returns>
        public static IEnumerable<Route> GetSectorRoutes(Guid sectorId)
        {
            return new SectorsController().GetSectorRoutes(sectorId.ToString());
        }

        /// <summary>
        /// Searches the repository for sectors.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public static Infrastructure.Search.SearchResult<Sector> SearchSectors(Infrastructure.Search.SearchQuery query)
        {
            return new SearchController().GetSectors(query);
        }

        #endregion

        #region Routes

        /// <summary>
        /// Searches the repository for routes.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public static Infrastructure.Search.SearchResult<Route> SearchRoutes(Infrastructure.Search.SearchQuery query)
        {
            return new SearchController().GetRoutes(query);
        }

        #endregion
    }
}