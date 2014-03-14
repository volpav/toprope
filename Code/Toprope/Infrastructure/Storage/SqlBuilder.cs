using System.Text;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents an SQL query builder. This class cannot be inherited.
    /// </summary>
    internal static class SqlBuilder
    {
        /// <summary>
        /// Returns a SELECT statement that takes column values from the given table.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="criteria">Optional criteria to be included into WHERE clause.</param>
        /// <returns>SQL statement.</returns>
        public static string Select(string tableName, string criteria)
        {
            return Select(tableName, criteria, string.Empty);
        }

        /// <summary>
        /// Returns a SELECT statement that takes column values from the given table.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="criteria">Optional criteria to be included into WHERE clause.</param>
        /// <param name="orderBy">ORDER BY expression.</param>
        /// <returns>SQL statement.</returns>
        public static string Select(string tableName, string criteria, string orderBy)
        {
            StringBuilder ret = new StringBuilder();
            
            if (!string.IsNullOrEmpty(tableName))
            {
                ret.AppendFormat("SELECT * FROM [{0}]", tableName);

                if (!string.IsNullOrEmpty(criteria))
                    ret.Append(string.Format(" WHERE ({0})", criteria));

                if (!string.IsNullOrEmpty(orderBy))
                    ret.Append(string.Format(" ORDER BY {0}", orderBy));
            }

            return ret.ToString();
        }

        /// <summary>
        /// Returns a SELECT statement that takes all column values from [Area], [Sector] and [Route] tables.
        /// </summary>
        /// <param name="criteria">Optional criteria to be included into WHERE clause.</param>
        /// <returns>SQL statement.</returns>
        public static string SelectAll(string criteria)
        {
            return SelectAll(criteria, string.Empty);
        }

        /// <summary>
        /// Returns a SELECT statement that takes all column values from [Area], [Sector] and [Route] tables.
        /// </summary>
        /// <param name="criteria">Optional criteria to be included into WHERE clause.</param>
        /// <param name="orderBy">ORDER BY expression.</param>
        /// <returns>SQL statement.</returns>
        public static string SelectAll(string criteria, string orderBy)
        {
            StringBuilder ret = new StringBuilder();

            ret.Append("SELECT ");
            ret.Append("a.[Id] AS [AreaId], ");
            ret.Append("a.[Name] AS [AreaName], ");
            ret.Append("a.[Description] AS [AreaDescription], ");
            ret.Append("a.[Location] AS [AreaLocation], ");
            ret.Append("a.[Tags] AS [AreaTags], ");
            ret.Append("a.[Season] AS [AreaSeason], ");
            ret.Append("a.[Climbing] AS [AreaClimbing], ");
            ret.Append("a.[Origin] AS [AreaOrigin], ");
            ret.Append("s.[Id] AS [SectorId], ");
            ret.Append("s.[AreaId] AS [SectorAreaId], ");
            ret.Append("s.[Name] AS [SectorName], ");
            ret.Append("s.[Description] AS [SectorDescription], ");
            ret.Append("s.[Location] AS [SectorLocation], ");
            ret.Append("s.[Tags] AS [SectorTags], ");
            ret.Append("s.[Order] AS [SectorOrder], ");
            ret.Append("s.[Season] AS [SectorSeason], ");
            ret.Append("s.[Climbing] AS [SectorClimbing], ");
            ret.Append("s.[Origin] AS [SectorOrigin], ");
            ret.Append("r.[Id] AS [RouteId], ");
            ret.Append("r.[SectorId] AS [RouteSectorId], ");
            ret.Append("r.[Name] AS [RouteName], ");
            ret.Append("r.[Description] AS [RouteDescription], ");
            ret.Append("r.[Grade] AS [RouteGrade], ");
            ret.Append("r.[Order] AS [RouteOrder] ");
            ret.Append("FROM [Area] a ");
            ret.Append("LEFT JOIN [Sector] s ");
            ret.Append("ON a.[Id] = s.[AreaId] ");
            ret.Append("LEFT JOIN [Route] r ");
            ret.Append("ON s.[Id] = r.[SectorId]");

            if (!string.IsNullOrEmpty(criteria))
                ret.Append(string.Format(" WHERE ({0})", criteria));

            if (!string.IsNullOrEmpty(orderBy))
                ret.Append(string.Format(" ORDER BY {0}", orderBy));

            return ret.ToString();
        }
    }
}