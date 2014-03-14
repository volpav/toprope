using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Toprope.Aggregator.Storage
{
    /// <summary>
    /// Dumps parsed data into the database.
    /// </summary>
    public class Dumper : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base directory.
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Gets or sets the images base directory.
        /// </summary>
        public string ImagesBaseDirectory { get; set; }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        private Toprope.Infrastructure.Storage.Repository Repository { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Dumper()
        {
            BaseDirectory = @"C:\Toprope files\";
            ImagesBaseDirectory = @"C:\Toprope files\_dumped\Content\Images\Sectors";

            Repository = new Infrastructure.Storage.Repository();
        }

        /// <summary>
        /// Dumps all parsed data to the database.
        /// </summary>
        public void Dump()
        {
            if (System.IO.Directory.Exists(BaseDirectory))
            {
                foreach (string dir in System.IO.Directory.EnumerateDirectories(BaseDirectory))
                    DumpArea(dir);
            }
        }

        /// <summary>
        /// Dumps the area.
        /// </summary>
        /// <param name="path">Area physical path.</param>
        private void DumpArea(string path)
        {
            int index = 1;
            bool isNewArea = false;
            Toprope.Models.Area area = null;
            string metaPhysicalPath = string.Empty;
            
            if (System.IO.Directory.Exists(path))
            {
                metaPhysicalPath = System.IO.Path.Combine(path, "metadata.xml");
                area = ReadArea(metaPhysicalPath);

                if (area != null)
                {
                    isNewArea = area.Id == Guid.Empty;

                    Repository.Update(area);

                    if (isNewArea)
                        Writer.WriteArea(area, metaPhysicalPath);

                    foreach (string dir in System.IO.Directory.EnumerateDirectories(path))
                        DumpSector(area, isNewArea, index++, dir);
                }
            }
        }

        /// <summary>
        /// Dumps the sector.
        /// </summary>
        /// <param name="area">Area.</param>
        /// <param name="isNewArea">Value indicating whether it was a newly created area.</param>
        /// <param name="order">Sector order.</param>
        /// <param name="path">Sector physical path.</param>
        private void DumpSector(Toprope.Models.Area area, bool isNewArea, int order, string path)
        {
            bool isSameArea = false;
            bool isNewSector = false;
            Toprope.Models.Sector sector = null;
            string metaPhysicalPath = string.Empty;
            string imagePhysicalPath = string.Empty;
            string sectorImageDirectory = string.Empty;
            string sectorImagePhysicalPath = string.Empty;

            if (area != null && System.IO.Directory.Exists(path))
            {
                metaPhysicalPath = System.IO.Path.Combine(path, "metadata.xml");
                sector = ReadSector(metaPhysicalPath);

                if (sector != null)
                {
                    isSameArea = sector.AreaId == area.Id;
                    isNewSector = sector.Id == Guid.Empty;

                    sector.AreaId = area.Id;
                    sector.Order = order;

                    if (sector.Tags == null || !sector.Tags.Any())
                        sector.Tags = area.Tags;

                    Repository.Update(sector);

                    if (isNewArea || isNewSector || !isSameArea)
                        Writer.WriteSector(sector, metaPhysicalPath);

                    DumpRoutes(sector, isNewSector, System.IO.Path.Combine(path, "routes.xml"));
                }

                imagePhysicalPath = System.IO.Path.Combine(path, "image.jpg");
                
                if (System.IO.File.Exists(imagePhysicalPath))
                {
                    sectorImageDirectory = System.IO.Path.Combine(ImagesBaseDirectory, sector.Id.ToString());
                    
                    if (!System.IO.Directory.Exists(sectorImageDirectory))
                    {
                        Writer.Task(() =>
                            {
                                System.IO.Directory.CreateDirectory(sectorImageDirectory);
                            });
                    }

                    sectorImagePhysicalPath = System.IO.Path.Combine(sectorImageDirectory, "image.jpg");
                    
                    Writer.Task(() =>
                        {
                            System.IO.File.WriteAllBytes(sectorImagePhysicalPath,
                                System.IO.File.ReadAllBytes(imagePhysicalPath));
                        });
                }
            }
        }

        /// <summary>
        /// Dumps routes.
        /// </summary>
        /// <param name="sector">Sector.</param>
        /// <param name="isNewSector">Value indicating whether it was a new sector.</param>
        /// <param name="path">Routes physical path.</param>
        private void DumpRoutes(Toprope.Models.Sector sector, bool isNewSector, string path)
        {
            int index = 1;
            bool hasNewRoute = false;
            bool hasDifferentSector = false;
            IEnumerable<Toprope.Models.Route> routes = null;

            if (sector != null && System.IO.File.Exists(path))
            {
                routes = ReadRoutes(path);
                
                if (routes != null)
                {
                    foreach (Toprope.Models.Route r in routes)
                    {
                        if (!hasNewRoute)
                            hasNewRoute = r.Id == Guid.Empty;

                        if (!hasDifferentSector)
                            hasDifferentSector = r.SectorId != sector.Id;

                        r.Order = index++;
                        r.SectorId = sector.Id;

                        Repository.Update(r);
                    }

                    if (hasNewRoute || hasDifferentSector || isNewSector)
                        Writer.WriteRoutes(routes, sector, path);
                }
            }
        }

        /// <summary>
        /// Disposes all resources used by this dumber.
        /// </summary>
        public void Dispose()
        {
            if (Repository != null)
            {
                Repository.Dispose();
                Repository = null;
            }
        }

        #region Static methods

        /// <summary>
        /// Reads the area information from the given XML file.
        /// </summary>
        /// <param name="path">Physical path to the file.</param>
        /// <returns>Area information.</returns>
        public static Toprope.Models.Area ReadArea(string path) 
        {
            return ReadPlace<Toprope.Models.Area>(path, null);
        }

        /// <summary>
        /// Reads the sector information from the given XML file.
        /// </summary>
        /// <param name="path">Physical path to the file.</param>
        /// <returns>Sector information.</returns>
        public static Toprope.Models.Sector ReadSector(string path)
        {
            return ReadPlace<Toprope.Models.Sector>(path, (s, d) =>
                {
                    Guid idParsed = Guid.Empty;

                    foreach (XElement e in d.Element("metadata").Descendants())
                    {
                        if (string.Compare(e.Name.LocalName, "areaId", StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            if (Guid.TryParse(e.Value, out idParsed))
                                s.AreaId = idParsed;
                        }
                        else if (string.Compare(e.Name.LocalName, "order", StringComparison.InvariantCultureIgnoreCase) == 0)
                            s.Order = Toprope.Infrastructure.Utilities.Input.GetInt(e.Value);
                    }
                });
        }

        /// <summary>
        /// Reads the place information from the given XML file.
        /// </summary>
        /// <param name="path">Physical path to the file.</param>
        /// <param name="reader">A reader used to fill additional place properties.</param>
        /// <returns>Place information.</returns>
        public static T ReadPlace<T>(string path, Action<T, XDocument> reader) where T: Toprope.Models.Place, new()
        {
            T ret = default(T);
            Match originMatch = null;
            string[] location = null;
            XDocument document = null;
            Guid idParsed = Guid.Empty;
            System.IO.FileInfo info = null;
            double latitude = 0, longitude = 0;
            Toprope.Models.Seasons seasons = Models.Seasons.NotSpecified;
            Toprope.Models.ClimbingTypes climbing = Models.ClimbingTypes.NotSpecified;
            
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                info = new System.IO.FileInfo(path);
                
                if (info.Length > 0)
                {
                    try
                    {
                        document = XDocument.Load(path);
                    }
                    catch (System.IO.IOException) { }
                    catch (System.Xml.XmlException) { }
                    catch (UnauthorizedAccessException) { }
                    catch (System.Security.SecurityException) { }

                    if (document != null)
                    {
                        ret = new T();

                        foreach (XElement e in document.Element("metadata").Descendants())
                        {
                            if (string.Compare(e.Name.LocalName, "id", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                if (Guid.TryParse(e.Value, out idParsed))
                                    ret.Id = idParsed;
                            }
                            else if (string.Compare(e.Name.LocalName, "name", StringComparison.InvariantCultureIgnoreCase) == 0)
                                ret.Name = Parsing.Specialized.RockClimbingComWiki.NormalizeName(e.Value, true);
                            else if (string.Compare(e.Name.LocalName, "description", StringComparison.InvariantCultureIgnoreCase) == 0)
                                ret.Description = e.Value;
                            else if (string.Compare(e.Name.LocalName, "tags", StringComparison.InvariantCultureIgnoreCase) == 0)
                                ret.Tags = new List<string>((e.Value ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => Toprope.Infrastructure.Utilities.Input.FormatTag(t)));
                            else if (string.Compare(e.Name.LocalName, "climbing", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                if (Enum.TryParse<Toprope.Models.ClimbingTypes>(e.Value, true, out climbing))
                                    ret.Climbing = climbing;
                            }
                            else if (string.Compare(e.Name.LocalName, "season", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                if (Enum.TryParse<Toprope.Models.Seasons>(e.Value, true, out seasons))
                                    ret.Season = seasons;
                            }
                            else if (string.Compare(e.Name.LocalName, "location", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                latitude = 0; longitude = 0;
                                location = (e.Value ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                if (location != null && location.Length > 1)
                                {
                                    latitude = Toprope.Infrastructure.Utilities.Input.GetDouble(location[0]);
                                    longitude = Toprope.Infrastructure.Utilities.Input.GetDouble(location[1]);

                                    ret.Location = new Models.Location(latitude, longitude);

                                    if (ret.Location.Latitude < -90 || ret.Location.Latitude > 90)
                                        ret.Location = null;
                                }
                            }
                            else if (string.Compare(e.Name.LocalName, "origin", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                ret.Origin = e.Value;

                                if (!string.IsNullOrEmpty(ret.Origin) && !ret.Origin.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                                    ret.Origin = "http://rockclimbing.com";
                            }
                        }

                        if (reader != null)
                            reader(ret, document);

                        if (!string.IsNullOrEmpty(ret.Description))
                        {
                            originMatch = Regex.Match(ret.Description, "via\\s+([^\\s]+)", RegexOptions.IgnoreCase);

                            if (originMatch != null && originMatch.Success)
                            {
                                ret.Origin = originMatch.Groups[1].Value;
                                ret.Description = ret.Description.Remove(originMatch.Index, originMatch.Length).Trim();
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Reads the route information from the given XML file.
        /// </summary>
        /// <param name="path">Physical path to the file.</param>
        /// <returns>Route information.</returns>
        public static IList<Toprope.Models.Route> ReadRoutes(string path)
        {
            double grade = 0;
            XDocument document = null;
            Guid idParsed = Guid.Empty;
            System.IO.FileInfo info = null;
            Toprope.Models.Route route = null;
            List<Toprope.Models.Route> ret = new List<Models.Route>();
            Toprope.Models.ClimbingTypes climbing = Models.ClimbingTypes.NotSpecified;

            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                info = new System.IO.FileInfo(path);

                if (info.Length > 0)
                {
                    try
                    {
                        document = XDocument.Load(path);
                    }
                    catch (System.IO.IOException) { }
                    catch (System.Xml.XmlException) { }
                    catch (UnauthorizedAccessException) { }
                    catch (System.Security.SecurityException) { }

                    if (document != null)
                    {
                        foreach (XElement r in document.Descendants("route"))
                        {
                            route = new Models.Route();

                            foreach (XElement e in r.Descendants())
                            {
                                if (string.Compare(e.Name.LocalName, "id", StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    if (Guid.TryParse(e.Value, out idParsed))
                                        route.Id = idParsed;
                                }
                                else if (string.Compare(e.Name.LocalName, "sectorId", StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    if (Guid.TryParse(e.Value, out idParsed))
                                        route.SectorId = idParsed;
                                }
                                else if (string.Compare(e.Name.LocalName, "name", StringComparison.InvariantCultureIgnoreCase) == 0)
                                    route.Name = Parsing.Specialized.RockClimbingComWiki.NormalizeName(e.Value, false);
                                else if (string.Compare(e.Name.LocalName, "description", StringComparison.InvariantCultureIgnoreCase) == 0)
                                    route.Description = e.Value;
                                else if (string.Compare(e.Name.LocalName, "grade", StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    grade = Toprope.Infrastructure.Utilities.Input.GetDouble(e.Value);

                                    if (grade > 0)
                                        route.Grade = new Models.RouteGrade(grade);
                                }
                                else if (string.Compare(e.Name.LocalName, "order", StringComparison.InvariantCultureIgnoreCase) == 0)
                                    route.Order = Toprope.Infrastructure.Utilities.Input.GetInt(e.Value);
                                else if (string.Compare(e.Name.LocalName, "climbing", StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    if (Enum.TryParse<Toprope.Models.ClimbingTypes>(e.Value, true, out climbing))
                                        route.Climbing = climbing;
                                }
                            }

                            ret.Add(route);
                        }
                    }
                }
            }

            ret.Sort((x, y) => x.Order - y.Order);

            return ret;
        }

        #endregion
    }
}
