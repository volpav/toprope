using System.Linq;
using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents a discover controller.
    /// </summary>
    public class DiscoverController : Infrastructure.LocalizableController
    {
        #region Properties

        private static long _totalRoutes = 0;

        /// <summary>
        /// Gets the total number of available routes.
        /// </summary>
        public static long TotalRoutes
        {
            get
            {
                if (_totalRoutes <= 0)
                {
                    using (Infrastructure.Storage.Repository repo = new Infrastructure.Storage.Repository())
                    {
                        using (System.Data.IDataReader reader = repo.Select("SELECT COUNT (*) AS [TotalRoutes] FROM [Route]"))
                        {
                            if (reader.Read())
                                _totalRoutes = Infrastructure.Utilities.Input.GetLong(reader["TotalRoutes"]);
                        }

                    }
                }

                return _totalRoutes;
            }
        }

        #endregion

        /// <summary>
        /// Serves the default view.
        /// </summary>
        /// <returns>The default view.</returns>
        public ActionResult Index()
        {
            return View(GetModel());
        }

        /// <summary>
        /// Returns the model.
        /// </summary>
        /// <returns>Model.</returns>
        private Models.RouteDiscoveryModel GetModel()
        {
            System.Collections.Generic.IEnumerable<string> queries = null;
            Models.RouteDiscoveryModel ret = new Models.RouteDiscoveryModel();

            ret.TotalRoutes = DiscoverController.TotalRoutes;

            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_UnitedStates, "tags:united-states"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Canada, "tags:canada"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Mexico, "tags:mexico"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Argentina, "tags:argentina"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Austria, "tags:austria"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_England, "tags:england"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_France, "tags:france"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Germany, "tags:germany"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Italy, "tags:italy"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Spain, "tags:spain"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Switzerland, "tags:switzerland"));
            ret.Countries.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Country_Turkey, "tags:turkey"));

            ret.Countries.Sort((c1, c2) => string.Compare(c1.Name, c2.Name));

            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Siurana, "tags:siurana"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Frankenjura, "tags:frankenjura"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_YosemiteNationalPark, "tags:yosemite-national-park"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Sardinia, "tags:sardinia"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Yangshuo, "tags:yangshuo"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_JoshuaTreeNationalPark, "tags:joshua-tree-national-park"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_KhaoJeenLair, "tags:khao-jeen-lair"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_KualaLumpur, "tags:kuala-lumpur"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Tirol, "Tirol"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Fontainebleau, "Fontainebleau"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Arco, "Arco"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Kullaberg, "tags:kullaberg"));
            ret.Areas.Add(new Models.DiscoveryItem(Toprope.Resources.Frontend.Area_Ekne, "tags:ekne"));

            ret.Areas.Sort((a1, a2) => string.Compare(a1.Name, a2.Name));

            if (Infrastructure.Application.Current.QueryCache != null)
            {
                queries = Infrastructure.Application.Current.QueryCache.Take(15).Select(i =>
                    {
                        string q = string.Empty;
                        int separatorIndex = -1;
                        Infrastructure.Search.SearchQuery query = null;

                        if (i.Value != null && i.Value is Infrastructure.Search.SearchResult && (i.Value as Infrastructure.Search.SearchResult).Total > 0)
                        {
                            q = i.Key;
                            separatorIndex = q.IndexOf('.');

                            if (separatorIndex > 0)
                                q = q.Substring(separatorIndex + 1);

                            query = Infrastructure.Search.SearchQuery.Parse(q);

                            if (query != null)
                                q = query.WithoutCriterion("take").WithoutCriterion("skip").ToString();

                            if (q.Length > 25)
                                q = string.Empty;
                        }

                        return q;
                    }).Distinct();

                ret.NowSearching.AddRange(queries.Where(k => !string.IsNullOrEmpty(k)).Select(k => new Models.DiscoveryItem(k, k)));

                ret.NowSearching.Sort((a1, a2) => string.Compare(a1.Name, a2.Name));
            }

            return ret;
        }
    }
}
