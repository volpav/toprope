using System;
using System.Xml;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;

namespace Toprope.Aggregator.Parsing.Specialized
{
    /// <summary>
    /// Represents a parser for "rockclimbing.com" website.
    /// </summary>
    public class RockClimbingComParser : Parser
    {
        #region Properties

        private RockClimbingComState _state = null;

        /// <summary>
        /// Gets the state file location.
        /// </summary>
        private const string StateFileLocation = @"C:\Toprope files\rockclimbingcom.xml";

        /// <summary>
        /// Gets the list item pattern.
        /// </summary>
        private const string ListItemPattern = "<dt>[^<]+<a\\s+href=\"([^\"]+)\">([^<]+)</a>([^<]+)</dt>";

        /// <summary>
        /// Gets or sets the list of areas currently parsed.
        /// </summary>
        private List<Parsing.ParsedArea> Areas { get; set; }

        /// <summary>
        /// Gets or sets the current total number of parsed areas.
        /// </summary>
        private int AreasCount { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether full region was parsed.
        /// </summary>
        private bool FullRegion { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether full country was parsed.
        /// </summary>
        private bool FullCountry { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether full country region was parsed.
        /// </summary>
        private bool FullCountryRegion { get; set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        private RockClimbingComState State
        {
            get
            {
                EnsureState();
                return _state;
            }
        }

        /// <summary>
        /// Gets the base URL of this parser.
        /// </summary>
        public override string BaseUrl
        {
            get { return "http://www.rockclimbing.com/routes/"; }
        }
        
        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RockClimbingComParser()
        {
            Areas = new List<ParsedArea>();
            AreasCount = 0;
        }

        /// <summary>
        /// Begin parsing items.
        /// </summary>
        public override void BeginParse()
        {
            int index = 0;
            IList<RockClimbingComListItem> regions = null;

            Areas.Clear();
            AreasCount = 0;

            if (State.Regions == null || !State.Regions.Any())
                State.Regions = GetListItems(Load(string.Empty)).ToList();

            regions = State.Regions;
            index = State.RegionIndex;

            if (index < 0) 
                index = 0;

            for (int i = index; i < regions.Count; i++)
            {
                Trace("Region: {0}...", regions[i].Name);

                ParseRegion(regions[i].Name, regions[i].Url);

                if (FullRegion)
                    State.RegionIndex = i + 1;

                if (AreasCount == Settings.MaxResults)
                    break;
            }

            if (Areas.Count > 0)
            {
                OnParsing(new ParserResultEventArgs(Areas));
                Areas.Clear();
            }
        }

        /// <summary>
        /// Begin parsing region.
        /// </summary>
        /// <param name="name">Region name.</param>
        /// <param name="url">Region URL.</param>
        private void ParseRegion(string name, string url)
        {
            int index = 0;
            IList<RockClimbingComListItem> countries = null;

            if (State.Countries == null || !State.Countries.ContainsKey(name))
            {
                if (State.Countries == null)
                    State.Countries = new Dictionary<string, IList<RockClimbingComListItem>>();

                if (!State.Countries.ContainsKey(name))
                    State.Countries.Add(name, GetListItems(Load(url)).ToList());
                else
                {
                    index = State.CountryIndex;
                    if (index < 0)
                        index = 0;
                }
            }
            else
            {
                index = State.CountryIndex;
                if (index < 0)
                    index = 0;
            }

            countries = State.Countries[name];

            for (int i = index; i < countries.Count; i++)
            {
                Trace("Country: {0} -> {1}...", name, countries[i].Name);

                FullRegion = i == (countries.Count - 1);
                ParseCountry(countries[i].Name, countries[i].Url, name);

                if (FullCountry)
                    State.CountryIndex = i + 1;

                if (AreasCount == Settings.MaxResults)
                    break;


            }
        }

        /// <summary>
        /// Begin parsing country.
        /// </summary>
        /// <param name="name">Country name.</param>
        /// <param name="url">Country URL.</param>
        /// <param name="regionName">Region name.</param>
        private void ParseCountry(string name, string url, string regionName)
        {
            int index = 0;
            IList<RockClimbingComListItem> countryRegions = null;

            if (State.CountryRegions == null || !State.CountryRegions.ContainsKey(name))
            {
                if (State.CountryRegions == null)
                    State.CountryRegions = new Dictionary<string, IList<RockClimbingComListItem>>();

                if (!State.CountryRegions.ContainsKey(name))
                    State.CountryRegions.Add(name, GetListItems(Load(url)).ToList());
                else
                {
                    index = State.CountryRegionIndex;
                    if (index < 0)
                        index = 0;
                }
            }
            else
            {
                index = State.CountryRegionIndex;
                if (index < 0)
                    index = 0;
            }

            countryRegions = State.CountryRegions[name];

            for (int i = index; i < countryRegions.Count; i++)
            {
                Trace("Country region: {0} -> {1} -> {2}...", regionName, name, countryRegions[i].Name);

                FullCountry = i == (countryRegions.Count - 1);
                ParseCountryRegion(countryRegions[i].Name, countryRegions[i].Url, name, regionName);

                if (FullCountryRegion)
                    State.CountryRegionIndex = i + 1;

                if (AreasCount == Settings.MaxResults)
                    break;
            }
        }

        /// <summary>
        /// Begins parsing country region.
        /// </summary>
        /// <param name="name">Region name.</param>
        /// <param name="url">Region URL.</param>
        /// <param name="countryName">Country name.</param>
        /// <param name="regionName">Country region name.</param>
        private void ParseCountryRegion(string name, string url, string countryName, string regionName)
        {
            int index = 0;
            RockClimbingComWikiTree tree = null;
            IList<RockClimbingComListItem> areas = null;
            
            // Restoring from the state
            if (State.Areas == null || !State.Areas.ContainsKey(name))
            {
                if (State.Areas == null)
                    State.Areas = new Dictionary<string, IList<RockClimbingComListItem>>();

                if (!State.Areas.ContainsKey(name))
                    State.Areas.Add(name, GetListItems(Load(url)).ToList());
                else
                {
                    index = State.AreaIndex;
                    if (index < 0)
                        index = 0;
                }
            }
            else
            {
                index = State.AreaIndex;
                if (index < 0)
                    index = 0;
            }

            areas = State.Areas[name];

            for (int i = index; i < areas.Count; i++)
            {
                // Writing to the console about what we're doing
                Trace("Tree: {0} -> {1} -> {2} -> {3}...", regionName, countryName, name, areas[i].Name);

                // Indicates whether a calling method must update the country region state
                FullCountryRegion = i == (areas.Count - 1);

                // Parsing a tree structure starting with the given URL
                tree = ParseTree(areas[i].Url);

                // Converting the parsed wiki tree to a list of areas
                ConvertTreeToAreas(tree, new string[] { regionName, countryName, name });

                // Saving state
                State.AreaIndex = i + 1;

                // Checking whether the limit of how many areas to parse is reached
                if (AreasCount == Settings.MaxResults)
                    break;
            }
        }

        /// <summary>
        /// Converts the given tree to areas.
        /// </summary>
        /// <param name="tree">Tree to convert.</param>
        /// <param name="tags">Parent tags, if any.</param>
        private void ConvertTreeToAreas(RockClimbingComWikiTree tree, IEnumerable<string> tags)
        {
            int order = 0;
            ParsedArea area = null;
            ParsedSector sector = null;
            IList<string> areaTags = null;
            IList<string> sectorTags = null;
            RockClimbingComWiki areaWiki = null;
            RockClimbingComWikiNodeBase accumulatedSector = null;
            Dictionary<string, ParsedArea> areas = new Dictionary<string, ParsedArea>();
            
            if (tree != null && tree.SectorNodes.Any())
            {
                // Traversing only sector nodes - nodes where we found routes
                foreach (RockClimbingComWikiNode actualSector in tree.SectorNodes)
                {
                    // Accumulating details and routes
                    accumulatedSector = actualSector.AccumulateAll();

                    // Accumulating details for area
                    if (actualSector.Parent != null)
                        areaWiki = actualSector.Parent.AccumulateWiki();
                    else
                        areaWiki = actualSector.AccumulateWiki();

                    // Accumulating tags for area and sector
                    sectorTags = actualSector.GetTags(tags);

                    if (actualSector.Parent != null)
                        areaTags = actualSector.Parent.GetTags(tags);
                    else
                        areaTags = new List<string>(sectorTags);

                    if (accumulatedSector != null && areaWiki != null)
                    {
                        sector = new ParsedSector();

                        if (accumulatedSector.Info != null)
                        {
                            sector.Name = accumulatedSector.Info.Name;
                            sector.Description = accumulatedSector.Info.Description;
                            sector.Climbing = accumulatedSector.Info.Climbing;
                            sector.Location = accumulatedSector.Info.Location;
                            sector.Season = accumulatedSector.Info.Season;
                            sector.Origin = "http://rockclimbing.com";
                            sector.Tags = sectorTags;
                            sector.Order = (++order);
                        }

                        // Assigning all routes from the tree
                        sector.Routes = accumulatedSector.Routes;
                        
                        // Getting image
                        if (accumulatedSector.Contents != null && accumulatedSector.Contents.Any())
                        {
                            foreach (string sectorContent in accumulatedSector.Contents)
                            {
                                sector.Image = ParseImage(sectorContent);
                                
                                if (sector.Image != null && sector.Image.Any())
                                    break;
                            }
                        }

                        if (!areas.ContainsKey(areaWiki.Name))
                        {
                            area = new ParsedArea();

                            area.Name = areaWiki.Name;
                            area.Description = areaWiki.Description;
                            area.Climbing = areaWiki.Climbing;
                            area.Location = areaWiki.Location;
                            area.Season = areaWiki.Season;
                            area.Origin = "http://rockclimbing.com";
                            area.Tags = areaTags;

                            areas.Add(areaWiki.Name, area);
                        }

                        if (areas[areaWiki.Name].Sectors == null)
                            areas[areaWiki.Name].Sectors = new List<ParsedSector>();

                        if (sector.Routes != null && sector.Routes.Any())
                        {
                            // Adding sector to area
                            areas[areaWiki.Name].Sectors.Add(sector);
                        }
                    }
                }

                foreach (ParsedArea a in areas.Values)
                {
                    if (a.Sectors != null && a.Sectors.Any())
                        OnAreaParsed(a);
                }
            }
        }

        /// <summary>
        /// Parses the wiki tree.
        /// </summary>
        /// <param name="url">Base URL.</param>
        /// <returns>Tree.</returns>
        private RockClimbingComWikiTree ParseTree(string url)
        {
            RockClimbingComWikiNode n = null;
            RockClimbingComWikiTree ret = new RockClimbingComWikiTree();

            ret.Parent = null;
            ret.SectorNodes = new List<RockClimbingComWikiNode>();

            n = ParseTreeRecursive(url, ret);

            if (n != null)
            {
                ret.Info = n.Info;
                ret.Routes = n.Routes;
                ret.ChildNodes = n.ChildNodes;
            }

            return ret;
        }

        /// <summary>
        /// Parses the wiki tree structure.
        /// </summary>
        /// <param name="url">Target URL.</param>
        /// <param name="root">Root tree.</param>
        /// <returns>Tree structure.</returns>
        private RockClimbingComWikiNode ParseTreeRecursive(string url, RockClimbingComWikiTree root)
        {
            string content = string.Empty;
            RockClimbingComWikiNode ret = null;
            RockClimbingComWikiNode child = null;

            if (!string.IsNullOrEmpty(url))
            {
                Trace("Tree node: {0}", Prettify(url));

                content = Load(url);
                
                if (!string.IsNullOrEmpty(content))
                {
                    ret = new RockClimbingComWikiNode();

                    ret.Contents = new List<string>(new string[] { content });
                    ret.Info = RockClimbingComWiki.Parse(content);
                    ret.Routes = ParseRoutes(content);

                    ret.ChildNodes = new List<RockClimbingComWikiNode>();

                    // Trying to mark node as sector node
                    root.SectorNodes.Add(ret);

                    foreach (RockClimbingComListItem item in GetListItems(content))
                    {
                        if (item.Total > 0)
                        {
                            child = ParseTreeRecursive(item.Url, root);
                            
                            if (child != null)
                            {
                                child.Parent = ret;
                                ret.ChildNodes.Add(child);
                            }
                        }
                    }

                    // Node has no routes - removing from sector nodes
                    if (ret.Routes == null || !ret.Routes.Any())
                        root.SectorNodes.Remove(ret);
                }
            }

            return ret;
        }

        /// <summary>
        /// Processes parsed area.
        /// </summary>
        /// <param name="area">Area.</param>
        private void OnAreaParsed(ParsedArea area)
        {
            if (area != null)
            {
                Areas.Add(area);
                AreasCount += 1;

                if (Areas.Count >= Settings.ChunkSize)
                {
                    OnParsing(new ParserResultEventArgs(Areas));
                    Areas.Clear();
                }
            }
        }

        /// <summary>
        /// Raises "Parsing" event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnParsing(ParserResultEventArgs e)
        {
            SaveState();

            base.OnParsing(e);
        }

        /// <summary>
        /// Makes a pretty path out of the given URL.
        /// </summary>
        /// <param name="url">URL to process.</param>
        /// <returns>Path.</returns>
        private string Prettify(string url)
        {
            string ret = url;
            int routesIndex = -1;
            string prefix = "routes/";

            if (!string.IsNullOrEmpty(ret))
            {
                routesIndex = ret.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase);
                if (routesIndex > 0)
                {
                    ret = ret.Substring(routesIndex + prefix.Length);
                    ret = ret.Replace("/", " -> ").Replace("_", " ");
                    ret = Regex.Replace(ret, @"\s+", " ").TrimEnd().TrimEnd('-', '>').Trim();
                }
            }

            return ret;
        }

        /// <summary>
        /// Parses first photo content.
        /// </summary>
        /// <param name="content">HTML content.</param>
        /// <returns>Photo content.</returns>
        private byte[] ParseImage(string content)
        {
            Match m = null;
            byte[] ret = null;
            string imageDetails = string.Empty;

            if (content != null)
            {
                m = Regex.Match(content, "/cgi-bin/photos/jump.cgi\\?Detailed=[0-9]+", RegexOptions.IgnoreCase);
                if (m != null && m.Success)
                {
                    imageDetails = Load(string.Format("..{0}", m.Value));
                    if (!string.IsNullOrEmpty(imageDetails))
                    {
                        m = Regex.Match(imageDetails, "/images/photos/[^\"]+largest[^\"]+", RegexOptions.IgnoreCase);

                        if (m != null && m.Success)
                            ret = LoadBinary(string.Format("..{0}", m.Value));
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Parses routes.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>Parsed routes.</returns>
        private IList<ParsedRoute> ParseRoutes(string content)
        {
            return ParseRoutes(content, 1, 1);
        }

        /// <summary>
        /// Parses routes.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="currentPage">Current page.</param>
        /// <param name="startOrder">Start order.</param>
        /// <returns>Parsed routes.</returns>
        private IList<ParsedRoute> ParseRoutes(string content, int currentPage, int startOrder)
        {
            int end = -1;
            Match m = null;
            ParsedRoute r = null;
            MatchCollection matches = null;
            string subContent = string.Empty;
            string routeDetails = string.Empty;
            List<ParsedRoute> ret = new List<ParsedRoute>();
            
            Func<Match, string> getSubContent = (ms) =>
                {
                    int st = -1, ed = -1;
                    string result = string.Empty;
                    
                    st = ms.Index;
                    ed = content.IndexOf("</td>", st, StringComparison.InvariantCultureIgnoreCase);

                    if (ed > 0)
                        result = content.Substring(st, ed - st);

                    return result;
                };

            m = Regex.Match(content, "<h3[^>]+>Routes</h3>", RegexOptions.IgnoreCase);

            if (m != null && m.Success)
            {
                content = content.Substring(m.Index + m.Length);

                m = Regex.Match(content, "<table[^>]+class=\"ftable\">", RegexOptions.IgnoreCase);
                
                if (m != null && m.Success)
                {
                    content = content.Substring(m.Index);
                    end = content.IndexOf("</table>", StringComparison.InvariantCultureIgnoreCase);

                    if (end > 0)
                    {
                        content = content.Substring(0, end);
                        matches = Regex.Matches(content, "<td", RegexOptions.IgnoreCase);

                        if (matches != null && matches.Count > 0)
                        {
                            for (int i = 0; i < matches.Count; i += 5)
                            {
                                subContent = getSubContent(matches[i + 2]);
                                m = Regex.Match(subContent, "<a\\s+href=\"([^\"]+)\">([^<]+)</a>", RegexOptions.IgnoreCase);

                                if (m != null && m.Success)
                                {
                                    r = new ParsedRoute();
                                    
                                    r.Name = RockClimbingComWiki.NormalizeName(m.Groups[2].Value.Trim(), false);

                                    if (!string.IsNullOrEmpty(r.Name))
                                    {
                                        r.Description = RockClimbingComWiki.FixPunctiation(m.Groups[1].Value.Trim());

                                        subContent = getSubContent(matches[i + 3]);
                                        m = Regex.Match(subContent, ">([^<]+)", RegexOptions.IgnoreCase);

                                        if (m != null && m.Success)
                                            r.Grade = Models.RouteGrade.Parse(m.Groups[1].Value.Trim());

                                        if (!string.IsNullOrEmpty(r.Description))
                                        {
                                            routeDetails = Load(r.Description);
                                            r.Description = string.Empty;

                                            m = Regex.Match(routeDetails, "<span\\s+class=\"description\">([^<]+)</span>", RegexOptions.IgnoreCase);

                                            if (m != null && m.Success)
                                                r.Description = RockClimbingComWiki.FixPunctiation(m.Groups[1].Value.Trim());
                                        }

                                        if (r.Grade != null)
                                        {
                                            r.Order = startOrder;
                                            r.Climbing = r.Grade.ParsedClimbing;

                                            ret.Add(r);

                                            startOrder += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            m = Regex.Match(content, string.Format("href=\"([^\"]+more{0}.html)\"", currentPage + 1), RegexOptions.IgnoreCase);
        
            if (m != null && m.Success)
                ret.AddRange(ParseRoutes(Load(m.Groups[1].Value.Trim()), currentPage + 1, startOrder + 1));

            return ret;
        }

        /// <summary>
        /// Ensures that the state file is loaded.
        /// </summary>
        private void EnsureState()
        {
            XDocument doc = null;
            XElement regions = null;
            
            if (_state == null)
            {
                _state = new RockClimbingComState();

                if (System.IO.File.Exists(StateFileLocation))
                {
                    using (FileStream stream = new FileStream(StateFileLocation, FileMode.Open, FileAccess.Read))
                        doc = XDocument.Load(stream);

                    regions = doc.Descendants("regions").FirstOrDefault();
                    
                    if (regions != null)
                    {
                        _state.Regions = regions.Descendants("item").Select(e => new RockClimbingComListItem() 
                        {
                            Name = e.Attribute("name").Value, 
                            Url = e.Attribute("url").Value, 
                            Total = Toprope.Infrastructure.Utilities.Input.GetInt(e.Attribute("total").Value) 
                        }).ToList();

                        _state.RegionIndex = Toprope.Infrastructure.Utilities.Input.GetInt(regions.Attribute("current").Value);
                    }

                    LoadGroup(doc, "countries", (groups, total) =>
                        {
                            _state.Countries = groups;
                            _state.CountryIndex = total;
                        });

                    LoadGroup(doc, "countryRegions", (groups, total) =>
                    {
                        _state.CountryRegions = groups;
                        _state.CountryRegionIndex = total;
                    });

                    LoadGroup(doc, "areas", (groups, total) =>
                    {
                        _state.Areas = groups;
                        _state.AreaIndex = total;
                    });
                }
            }

            if (_state == null)
                _state = new RockClimbingComState();
        }

        /// <summary>
        /// Loads groupped state.
        /// </summary>
        /// <param name="doc">Document.</param>
        /// <param name="name">Section name.</param>
        /// <param name="loader">Loader.</param>
        private void LoadGroup(XDocument doc, string name, Action<IDictionary<string, IList<RockClimbingComListItem>>, int> loader)
        {
            int current = -1;
            XElement elm = null;
            string groupName = string.Empty;
            Dictionary<string, IList<RockClimbingComListItem>> groups = new Dictionary<string, IList<RockClimbingComListItem>>();

            elm = doc.Descendants(name).FirstOrDefault();

            if (elm != null)
            {
                current = Toprope.Infrastructure.Utilities.Input.GetInt(elm.Attribute("current").Value);

                foreach (XElement g in elm.Descendants("group"))
                {
                    groupName = g.Attribute("name").Value;
                    if (!groups.ContainsKey(groupName))
                    {
                        groups.Add(groupName, g.Descendants("item").Select(e => new RockClimbingComListItem() 
                        {
                            Name = e.Attribute("name").Value, 
                            Url = e.Attribute("url").Value, 
                            Total = Toprope.Infrastructure.Utilities.Input.GetInt(e.Attribute("total").Value) 
                        }).ToList());
                    }
                }
            }

            if (loader != null)
                loader(groups, current);
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        private void SaveState()
        {
            XDocument doc = null;
            XElement root = null;
            string directory = string.Empty;

            if (_state != null)
            {
                root = new XElement("state");

                if (_state.Regions != null && _state.Regions.Any())
                {
                    root.Add(new XElement("regions",
                        new XAttribute("current", _state.RegionIndex.ToString()),
                        _state.Regions.Select(r => new XElement("item", 
                            new XAttribute("name", r.Name),
                            new XAttribute("url", r.Url),
                            new XAttribute("total", r.Total.ToString())))));

                    SaveGroup(root, "countries", _state.Countries, State.CountryIndex);
                    SaveGroup(root, "countryRegions", _state.CountryRegions, _state.CountryRegionIndex);
                    SaveGroup(root, "areas", _state.Areas, _state.AreaIndex);
                }

                doc = new XDocument(root);

                directory = System.IO.Path.GetDirectoryName(StateFileLocation);

                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                using (XmlWriter writer = XmlWriter.Create(StateFileLocation, new XmlWriterSettings() { Indent = true, Encoding = System.Text.Encoding.UTF8 }))
                    doc.WriteTo(writer);
            }
        }

        /// <summary>
        /// Saves the group.
        /// </summary>
        /// <param name="addTo">Element to append to.</param>
        /// <param name="name">Section name.</param>
        /// <param name="groups">Groups.</param>
        /// <param name="current">Current group name.</param>
        private void SaveGroup(XElement addTo, string name, IDictionary<string, IList<RockClimbingComListItem>> groups, int current)
        {
            string[] keys = null;

            if (addTo != null && groups != null && groups.Any())
            {
                keys = groups.Keys.ToArray();
                
                addTo.Add(new XElement(name,
                    new XAttribute("current", current.ToString()),
                    keys.Select(k => new XElement("group",
                        new XAttribute("name", k),
                        groups.Where(p => string.Compare(p.Key, k) == 0).SelectMany(p => p.Value.Select(v => new XElement("item",
                            new XAttribute("name", v.Name),
                            new XAttribute("url", v.Url),
                            new XAttribute("total", v.Total))))))));
            }
        }

        /// <summary>
        /// Parses list items.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>List items.</returns>
        private IEnumerable<RockClimbingComListItem> GetListItems(string content)
        {
            int totalParsed = 0;
            RockClimbingComListItem item = null;
            List<RockClimbingComListItem> ret = new List<RockClimbingComListItem>();

            foreach (Match m in Regex.Matches(content, ListItemPattern, RegexOptions.IgnoreCase))
            {
                totalParsed = 0;

                item = new RockClimbingComListItem();

                item.Name = RockClimbingComWiki.NormalizeName(m.Groups[2].Value.Trim(), true);
                item.Url = m.Groups[1].Value.Trim();

                if (int.TryParse(m.Groups[3].Value.Trim().Trim('(', ')').Trim(), out totalParsed) && totalParsed > 0)
                {
                    item.Total = totalParsed;

                    if (!string.IsNullOrEmpty(item.Name))
                        ret.Add(item);
                }
            }

            return ret;
        }
    }
}
