using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Toprope.Aggregator.Storage
{
    /// <summary>
    /// Represents a file system writer.
    /// </summary>
    public class Writer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base directory.
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Gets the location culture.
        /// </summary>
        private static readonly System.Globalization.CultureInfo LocationCulture = new System.Globalization.CultureInfo("en-US");

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Writer()
        {
            BaseDirectory = @"C:\Toprope files\";
        }

        /// <summary>
        /// Writes the given areas.
        /// </summary>
        /// <param name="areas">Areas.</param>
        public void Write(System.Collections.Generic.IEnumerable<Parsing.ParsedArea> areas)
        {
            if (areas != null)
            {
                foreach (Parsing.ParsedArea area in areas)
                    WriteAreaDetails(area, Path.Combine(BaseDirectory, Writer.NormalizePathComponent(area.Name))); 
            }
        }

        /// <summary>
        /// Writes the given area.
        /// </summary>
        /// <param name="area">Area to write.</param>
        /// <param name="path">Area physical path.</param>
        private void WriteAreaDetails(Parsing.ParsedArea area, string path)
        {
            Toprope.Models.Area existing = null;
            string metadataFilePath = string.Empty;

            if (area != null && !string.IsNullOrEmpty(path))
            {
                EnsureDirectory(path);

                metadataFilePath = Path.Combine(path, "metadata.xml");
                existing = Dumper.ReadArea(metadataFilePath);

                if (existing != null)
                    area.Id = existing.Id;

                Writer.WriteArea(area, metadataFilePath);

                if (area.Sectors != null)
                {
                    foreach (Parsing.ParsedSector sector in area.Sectors)
                        WriteSectorDetails(sector, Path.Combine(path, Writer.NormalizePathComponent(sector.Name)));
                }
            }
        }

        /// <summary>
        /// Writes the given sector.
        /// </summary>
        /// <param name="sector">Sector to write.</param>
        /// <param name="path">Sector physical path.</param>
        private void WriteSectorDetails(Parsing.ParsedSector sector, string path)
        {
            Toprope.Models.Sector existing = null;
            Toprope.Models.Route foundRoute = null;
            string metadataFilePath = string.Empty;
            string metadataRoutesFilePath = string.Empty;
            System.Collections.Generic.IList<Toprope.Models.Route> existingRoutes = null;
            
            if (sector != null && !string.IsNullOrEmpty(path))
            {
                EnsureDirectory(path);

                metadataFilePath = Path.Combine(path, "metadata.xml");
                existing = Dumper.ReadSector(metadataFilePath);

                if (existing != null)
                    sector.Id = existing.Id;

                Writer.WriteSector(sector, metadataFilePath);

                if (sector.Image != null && sector.Image.Any())
                {
                    Task(() =>
                    {
                        File.WriteAllBytes(Path.Combine(path, "image.jpg"), sector.Image);
                    });
                }

                if (sector.Routes != null)
                {
                    metadataRoutesFilePath = Path.Combine(path, "routes.xml");
                    existingRoutes = Dumper.ReadRoutes(metadataRoutesFilePath);

                    if (existingRoutes != null && existingRoutes.Any())
                    {
                        foreach (Parsing.ParsedRoute r in sector.Routes)
                        {
                            if (r.Id == Guid.Empty)
                            {
                                foundRoute = existingRoutes.FirstOrDefault(candidate =>
                                    string.Compare(candidate.Name ?? string.Empty, r.Name ?? string.Empty, StringComparison.InvariantCultureIgnoreCase) == 0);

                                if (foundRoute != null)
                                    r.Id = foundRoute.Id;
                            }
                        }
                    }

                    Writer.WriteRoutes(sector.Routes, sector, metadataRoutesFilePath);
                }
                
            }
        }

        /// <summary>
        /// Ensures that the given directory exists.
        /// </summary>
        /// <param name="path">Directory path.</param>
        private void EnsureDirectory(string path)
        {
            Task(() =>
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            });
        }

        #region Static methods

        /// <summary>
        /// Executes the given I/O task.
        /// </summary>
        /// <param name="task">Task to execute.</param>
        public static void Task(System.Action task)
        {
            bool success = false;
            int maxAttempts = 5, delay = 100, currentAttempt = 1;
            System.Func<bool> isLastAttempt = () => { return currentAttempt == maxAttempts; };

            if (task != null)
            {
                for (currentAttempt = 1; currentAttempt <= maxAttempts; currentAttempt++)
                {
                    try
                    {
                        task();
                        success = true;
                    }
                    catch (System.IO.IOException) { if (isLastAttempt()) throw; }
                    catch (System.Security.SecurityException) { if (isLastAttempt()) throw; }

                    if (success)
                        break;
                    else
                        System.Threading.Thread.Sleep(delay);
                }
            }
        }

        /// <summary>
        /// Normalizes path component.
        /// </summary>
        /// <param name="component">Path component.</param>
        /// <returns></returns>
        public static string NormalizePathComponent(string component)
        {
            string ret = component ?? string.Empty;
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();

            if (!string.IsNullOrEmpty(ret) && invalidChars.Any(c => ret.IndexOf(c) >= 0))
            {
                ret = ret.Replace(':', '-');
                ret = ret.Replace('"', '\'');
                ret = ret.Replace("\\", string.Empty);
                ret = ret.Replace("/", string.Empty);
                ret = ret.Replace("*", string.Empty);
                ret = ret.Replace("?", string.Empty);
                ret = ret.Replace(">", string.Empty);
                ret = ret.Replace("<", string.Empty);
                ret = ret.Replace("|", string.Empty);
            }

            return ret;
        }

        /// <summary>
        /// Writes area metadata to a given file.
        /// </summary>
        /// <param name="area">Area.</param>
        /// <param name="path">Path.</param>
        public static void WriteArea(Toprope.Models.Area area, string path)
        {
            WritePlace<Toprope.Models.Area>(area, path, null);
        }

        /// <summary>
        /// Writes sector metadata to a given file.
        /// </summary>
        /// <param name="sector">Sector.</param>
        /// <param name="path">Path.</param>
        public static void WriteSector(Toprope.Models.Sector sector, string path)
        {
            WritePlace<Toprope.Models.Sector>(sector, path, (s) =>
                {
                    return new XElement[] { 
                        new XElement("areaId", s.AreaId.ToString()),
                        new XElement("order", s.Order.ToString())
                    };
                });
        }

        /// <summary>
        /// Writes place metadata to a given file.
        /// </summary>
        /// <param name="place">Place.</param>
        /// <param name="path">Path.</param>
        /// <param name="writer">A writer used to populate additional place details.</param>
        public static void WritePlace<T>(T place, string path, Func<T, System.Collections.Generic.IEnumerable<XElement>> writer) where T: Toprope.Models.Place
        {
            XDocument doc = null;
            System.Collections.Generic.List<XElement> elements = new System.Collections.Generic.List<XElement>();

            if (place != null && !string.IsNullOrEmpty(path))
            {
                elements.AddRange(new XElement[] {
                    new XElement("id", place.Id.ToString()),
                        new XElement("name", place.Name),
                        new XElement("description", place.Description),
                        new XElement("tags", place.Tags != null ? string.Join(",", place.Tags) : string.Empty),
                        new XElement("climbing", ((int)place.Climbing).ToString()),
                        new XElement("season", ((int)place.Season).ToString()),
                        new XElement("location", place.Location != null ? string.Format("{0},{1}", place.Location.Latitude.ToString(LocationCulture), place.Location.Longitude.ToString(LocationCulture)) : string.Empty),
                        new XElement("origin", place.Origin ?? string.Empty)
                });

                if (writer != null)
                    elements.AddRange(writer(place));

                doc = new XDocument(
                    new XElement("metadata",
                        elements
                    )
                );

                WriteDocument(doc, path);
            }
        }

        /// <summary>
        /// Writes sector routes.
        /// </summary>
        /// <param name="routes">Routes.</param>
        /// <param name="sector">Sector.</param>
        /// <param name="path">Routes path.</param>
        public static void WriteRoutes(System.Collections.Generic.IEnumerable<Toprope.Models.Route> routes, Toprope.Models.Sector sector, string path)
        {
            XDocument doc = null;

            if (sector != null && routes != null)
            {
                doc = new XDocument(
                    new XElement("routes",
                        routes.Where(r => r != null && r.Grade != null).OrderBy(r => r.Order).Select(r => new XElement("route",
                            new XElement("id", r.Id.ToString()),
                            new XElement("sectorId", sector.Id.ToString()),
                            new XElement("name", r.Name),
                            new XElement("description", r.Description),
                            new XElement("grade", r.Grade.Value.ToString()),
                            new XElement("order", r.Order.ToString()),
                            new XElement("climbing", r.Climbing.ToString())
                        ))
                    )
                );

                WriteDocument(doc, path);
            }
        }

        /// <summary>
        /// Writes the given XML document to disk.
        /// </summary>
        /// <param name="doc">Document to write.</param>
        /// <param name="path">Path.</param>
        private static void WriteDocument(XDocument doc, string path)
        {
            Task(() =>
            {
                using (XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings() { Indent = true, Encoding = System.Text.Encoding.UTF8 }))
                    doc.WriteTo(writer);
            });
        }

        #endregion
    }
}
