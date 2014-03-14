using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Toprope.Infrastructure.Storage;
using Toprope.Models;

namespace Toprope.Platform
{
    /// <summary>
    /// Represents a sectors controller.
    /// </summary>
    public class SectorsController
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SectorsController() { }

        /// <summary>
        /// Returns the sector by its Id.
        /// </summary>
        /// <param name="sectorId">Sector Id.</param>
        /// <returns>A sector by its Id.</returns>
        public Sector GetSector(string sectorId)
        {
            Sector ret = null;
            Guid id = Infrastructure.Utilities.Input.GetGuid(sectorId);

            if (id != Guid.Empty)
            {
                using (Repository repo = new Repository())
                    ret = repo.Select<Sector>(id);
            }

            return ret;
        }

        /// <summary>
        /// Returns all routes for a given sector.
        /// </summary>
        /// <param name="sectorId">Sector Id.</param>
        /// <returns>All routes for a given sector.</returns>
        public IEnumerable<Route> GetSectorRoutes(string sectorId)
        {
            Route r = null;
            List<Route> ret = new List<Route>();
            Guid id = Infrastructure.Utilities.Input.GetGuid(sectorId);

            if (id != Guid.Empty)
            {
                using (Repository repo = new Repository())
                {
                    using (System.Data.IDataReader reader = repo.Select(SqlBuilder.Select("Route", "[SectorId] = @id"), new Parameter("id", id)))
                    {
                        while (reader.Read())
                        {
                            r = Repository.Read<Route>(reader);

                            if (r != null && r.Id != Guid.Empty)
                                ret.Add(r);
                        }
                    }
                }
            }

            if (ret.Any())
                ret.Sort((x, y) => { return x.Order - y.Order; });

            return ret;
        }

        /// <summary>
        /// Returns all images of a given sector.
        /// </summary>
        /// <param name="sectorId">Sector Id.</param>
        /// <returns>All images of a given sector.</returns>
        public IEnumerable<Image> GetSectorImages(string sectorId)
        {
            Image scheme = null;
            int schemeIndex = -1;
            string ext = string.Empty;
            string absolutePath = string.Empty;
            string physicalPath = string.Empty;
            List<Image> ret = new List<Image>();
            Guid id = Infrastructure.Utilities.Input.GetGuid(sectorId);
            string[] allowedExtensions = new string[] { "png", "jpg", "svg" };

            if (id != Guid.Empty)
            {
                absolutePath = string.Format("/Content/Images/Sectors/{0}", id.ToString());
                physicalPath = HttpContext.Current.Server.MapPath("~" + absolutePath);

                if (!string.IsNullOrEmpty(physicalPath) && Directory.Exists(physicalPath))
                {
                    foreach (string f in Directory.EnumerateFiles(physicalPath))
                    {
                        ext = Path.GetExtension(f).Trim().Trim('.');

                        if (!string.IsNullOrEmpty(ext) && allowedExtensions.Any(e => string.Compare(e, ext, StringComparison.InvariantCultureIgnoreCase) == 0))
                            ret.Add(new Image() { Url = string.Format("{0}/{1}", absolutePath, Path.GetFileName(f)) });
                    }
                }

                if (ret.Any())
                {
                    // The first image is always sector scheme
                    schemeIndex = ret.FindIndex(i => i.Url.EndsWith("scheme.svg", StringComparison.InvariantCultureIgnoreCase));

                    if (schemeIndex >= 0 && schemeIndex != 0)
                    {
                        scheme = ret[schemeIndex];
                        ret.RemoveAt(schemeIndex);
                        ret.Insert(0, scheme);
                    }
                }
            }

            return ret;
        }
    }
}