using System;
using System.Collections.Generic;
using System.Linq;
using Toprope.Infrastructure.Storage;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a sector view model.
    /// </summary>
    public class SectorViewModel : Sector
    {
        #region Properties

        /// <summary>
        /// Gets the sector area.
        /// </summary>
        public Area Area { get; set; }

        /// <summary>
        /// Gets or sets sector routes.
        /// </summary>
        public IList<Route> Routes { get; set; }

        /// <summary>
        /// Gets or sets sector images.
        /// </summary>
        public IList<Image> Images { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SectorViewModel()
        {
            Routes = new List<Route>();
            Images = new List<Image>();
        }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="copyFrom">Object to copy state from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="copyFrom">copyFrom</paramref> is null.</exception>
        public SectorViewModel(Sector copyFrom) : base(copyFrom)
        {
            Routes = new List<Route>();
            Images = new List<Image>();
        }

        #region Static methods

        /// <summary>
        /// Loads model by sector Id.
        /// </summary>
        /// <param name="sectorId">Sector Id.</param>
        /// <returns>View model.</returns>
        public static SectorViewModel Load(Guid sectorId)
        {
            Route route = null;
            Sector sector = null;
            SectorViewModel ret = null;

            if (sectorId != Guid.Empty)
            {
                ret = new SectorViewModel();

                using (Repository repo = new Repository())
                {
                    using (System.Data.IDataReader reader = repo.Select(SqlBuilder.SelectAll("s.[Id] = @sectorId", "r.[Order]"), new Infrastructure.Storage.Parameter("sectorId", sectorId)))
                    {
                        while (reader.Read())
                        {
                            if (sector == null)
                            {
                                sector = Infrastructure.Storage.Repository.Read<Sector>(reader, "Sector");

                                if (sector != null)
                                    sector.CopyTo(ret);

                                ret.Area = Infrastructure.Storage.Repository.Read<Area>(reader, "Area");
                            }

                            route = Repository.Read<Route>(reader, "Route");

                            if (route != null && (!string.IsNullOrEmpty(route.Name) || route.Grade != null || !string.IsNullOrEmpty(route.Description) || route.Order > 0))
                                ret.Routes.Add(route);
                        }
                    }
                }

                if (sector != null)
                {
                    if (ret.Routes.Any())
                        ret.Routes = ret.Routes.OrderBy(r => r.Order).ToList();

                    ret.Images = Platform.Endpoint.GetSectorImages(sectorId).ToList();
                }
            }

            return ret;
        }

        #endregion
    }
}