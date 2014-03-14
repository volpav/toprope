using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Toprope.Infrastructure.Storage;

namespace Toprope.Models
{
    /// <summary>
    /// Represents an area view model.
    /// </summary>
    public class AreaViewModel : Area
    {
        #region Nested types

        /// <summary>
        /// Represents a sector summary.
        /// </summary>
        public class SectorSummary : Sector
        {
            #region Properties

            /// <summary>
            /// Gets or sets sector routes.
            /// </summary>
            public IList<Route> Routes { get; set; }

            #endregion

            /// <summary>
            /// Initializes a new instance of an object.
            /// </summary>
            public SectorSummary()
            {
                Routes = new List<Route>();
            }

            /// <summary>
            /// Initializes a new instance of an object.
            /// </summary>
            /// <param name="copyFrom">Object to copy state from.</param>
            /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
            public SectorSummary(Sector copyFrom) : base(copyFrom)
            {
                Routes = new List<Route>();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets area sectors.
        /// </summary>
        public IList<SectorSummary> Sectors { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public AreaViewModel()
        {
            Sectors = new List<SectorSummary>();
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public AreaViewModel(Area copyFrom) : base(copyFrom)
        {
            Sectors = new List<SectorSummary>();
        }

        #region Static methods

        /// <summary>
        /// Loads model by area Id.
        /// </summary>
        /// <param name="areaId">Area Id.</param>
        /// <returns>View model.</returns>
        public static AreaViewModel Load(Guid areaId)
        {
            Area area = null;
            Route route = null;
            Sector sector = null;
            AreaViewModel ret = null;
            SectorSummary summary = null;
            HashSet<Guid> addedRoutes = null;
            List<SectorSummary> summaries = null;
            IDictionary<Guid, Tuple<Sector, IList<Route>>> sectors = null;
            
            if (areaId != Guid.Empty)
            {
                ret = new AreaViewModel();
                addedRoutes = new HashSet<Guid>();
                summaries = new List<SectorSummary>();
                sectors = new Dictionary<Guid, Tuple<Sector, IList<Route>>>();

                using (Repository repo = new Repository())
                {
                    using (System.Data.IDataReader reader = repo.Select(SqlBuilder.SelectAll("a.[Id] = @areaId"), new Infrastructure.Storage.Parameter("areaId", areaId)))
                    {
                        while (reader.Read())
                        {
                            if (area == null)
                            {
                                area = Infrastructure.Storage.Repository.Read<Area>(reader, "Area");

                                if (area != null)
                                    area.CopyTo(ret);
                            }

                            sector = Infrastructure.Storage.Repository.Read<Sector>(reader, "Sector");
                            route = Infrastructure.Storage.Repository.Read<Route>(reader, "Route");

                            if (sector != null && sector.Id != Guid.Empty)
                            {
                                if (!sectors.ContainsKey(sector.Id))
                                    sectors.Add(sector.Id, new Tuple<Sector, IList<Route>>(sector, new List<Route>()));

                                if (route != null && route.Id != Guid.Empty && !addedRoutes.Contains(route.Id))
                                {
                                    sectors[sector.Id].Item2.Add(route);
                                    addedRoutes.Add(route.Id);
                                }
                            }
                        }
                    }
                }

                foreach (Tuple<Sector, IList<Route>> s in sectors.Values)
                {
                    summary = new SectorSummary(s.Item1);
                    summary.Routes = s.Item2.OrderBy(r => r.Order).ToList();

                    summaries.Add(summary);
                }

                ret.Sectors = summaries.OrderBy(s => s.Order).ToList();
            }

            return ret;
        }

        #endregion
    }
}