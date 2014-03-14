using System;
using System.Linq;
using System.Collections.Generic;
using Toprope.Infrastructure.Search;

namespace Toprope.Models.Search
{
    /// <summary>
    /// Represents a search results view model.
    /// </summary>
    public class SearchResultsViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the search query.
        /// </summary>
        public Infrastructure.Search.SearchQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the name of the view where search results are populated.
        /// </summary>
        public string InnerView { get; set; }

        /// <summary>
        /// Gets or sets the inner view model.
        /// </summary>
        public SearchResultContainer InnerViewModel { get; set; }

        /// <summary>
        /// Gets or sets the search hint.
        /// </summary>
        public string Hint { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SearchResultsViewModel()
        {
            PageSize = 20;
            InnerViewModel = new SearchResultContainer();
        }

        #region Static methods

        /// <summary>
        /// Loads the view model.
        /// </summary>
        /// <param name="host">Controller.</param>
        /// <remarks></remarks>
        public static SearchResultsViewModel Load(Controllers.HomeController host)
        {
            System.Type expectedResultType = null;
            Infrastructure.Search.SearchQuery copiedQuery = null;
            SearchResultsViewModel ret = new SearchResultsViewModel();
            Infrastructure.Search.SearchResult<Models.Area> areas = null;
            Infrastructure.Search.SearchResult<Models.Route> routes = null;
            Infrastructure.Search.SearchResult<Models.Sector> sectors = null;
            
            if (host != null)
            {
                if (host.Query != null && host.Query.Criteria != null && host.Query.Criteria.Count > 0)
                {
                    ret.Query = host.Query;

                    if (host.Skip > 0) 
                        ret.Query = ret.Query.WithCriterion("skip", host.Skip.ToString());

                    ret.Query = ret.Query.WithCriterion("take", host.Take > 0 ? host.Take.ToString() : ret.PageSize.ToString());

                    expectedResultType = ret.Query.GetExpectedResultType();

                    if (expectedResultType != null)
                    {
                        if (expectedResultType.Equals(typeof(Models.Area)))
                        {
                            areas = Platform.Endpoint.SearchAreas(ret.Query);
                            CreateFlatListViewModel<Models.Area>(ret, Toprope.Resources.Frontend.Areas, areas, (i => new AreaSearchResult(i)));
                        }
                        else if (expectedResultType.Equals(typeof(Models.Sector)))
                        {
                            sectors = Platform.Endpoint.SearchSectors(ret.Query);
                            CreateFlatListViewModel<Models.Sector>(ret, Toprope.Resources.Frontend.Sectors, sectors, (i => new SectorSearchResult(i)));
                        }
                        else if (expectedResultType.Equals(typeof(Models.Route)))
                        {
                            routes = Platform.Endpoint.SearchRoutes(ret.Query);
                            CreateFlatListViewModel<Models.Route>(ret, Toprope.Resources.Frontend.Routes, routes, (i => new RouteSearchResult(i)));
                        }
                    }
                    else
                    {
                        ret.Query = ret.Query.WithCriterion("skip", "0").WithCriterion("take", ret.PageSize.ToString());

                        areas = Platform.Endpoint.SearchAreas(ret.Query);
                        sectors = Platform.Endpoint.SearchSectors(ret.Query);
                        routes = Platform.Endpoint.SearchRoutes(ret.Query);

                        ret.InnerViewModel.Total = areas.Total + sectors.Total + routes.Total;

                        if (areas.Total > 0 && sectors.Total == 0 && routes.Total == 0)
                            CreateFlatListViewModel<Models.Area>(ret, Toprope.Resources.Frontend.Areas, areas, (i => new AreaSearchResult(i)));
                        else if (areas.Total == 0 && sectors.Total > 0 && routes.Total == 0)
                            CreateFlatListViewModel<Models.Sector>(ret, Toprope.Resources.Frontend.Sectors, sectors, (i => new SectorSearchResult(i)));
                        else if (areas.Total == 0 && sectors.Total == 0 && routes.Total > 0)
                            CreateFlatListViewModel<Models.Route>(ret, Toprope.Resources.Frontend.Routes, routes, (i => new RouteSearchResult(i)));
                        else
                        {
                            // "Areas" group
                            ret.InnerViewModel.Results.Add(new SearchResultGroup("areas", Toprope.Resources.Frontend.Areas, ret.Query)
                            {
                                Results = areas.Items.Take(10).Select(i => new AreaSearchResult(i)).OfType<SearchResultBase>().ToList(),
                                Total = areas.Total
                            });

                            // "Sectors" group
                            ret.InnerViewModel.Results.Add(new SearchResultGroup("sectors", Toprope.Resources.Frontend.Sectors, ret.Query)
                            {
                                Results = sectors.Items.Take(10).Select(i => new SectorSearchResult(i)).OfType<SearchResultBase>().ToList(),
                                Total = sectors.Total
                            });

                            // "Routes" group
                            ret.InnerViewModel.Results.Add(new SearchResultGroup("routes", Toprope.Resources.Frontend.Routes, ret.Query)
                            {
                                Results = routes.Items.Take(10).Select(i => new RouteSearchResult(i)).OfType<SearchResultBase>().ToList(),
                                Total = routes.Total
                            });

                            ret.InnerView = ret.InnerViewModel.Total > 0 ? "_SearchNested" : "_SearchEmpty";
                        }
                    }
                }

                ret.Query = ret.Query.WithoutCriterion("take").WithoutCriterion("skip");
                copiedQuery = new Infrastructure.Search.SearchQuery(ret.Query.Criteria);

                if (expectedResultType == null)
                {
                    ret.Hint = string.Format(Toprope.Resources.Frontend.NarrowDown, 
                        host.Url.Action("Index", "Home", new { q = copiedQuery.WithCriterion("in", "areas").ToString() }),
                        host.Url.Action("Index", "Home", new { q = copiedQuery.WithCriterion("in", "sectors").ToString() }),
                        host.Url.Action("Index", "Home", new { q = copiedQuery.WithCriterion("in", "routes").ToString() }));
                }

                ret.InnerViewModel.PageSize = ret.PageSize;
                ret.InnerViewModel.Query = ret.Query;
            }

            return ret;
        }

        /// <summary>
        /// Initializes flat list result model.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="model">Model.</param>
        /// <param name="title">Title.</param>
        /// <param name="results">Search results.</param>
        /// <param name="searchResultCreator">Search result item creator.</param>
        private static void CreateFlatListViewModel<T>(SearchResultsViewModel model, string title, Toprope.Infrastructure.Search.SearchResult<T> results, System.Func<T, SearchResultBase> searchResultCreator)
        {
            model.InnerViewModel.Title = title;
            model.InnerViewModel.Results = results.Items.Select(searchResultCreator).ToList();
            model.InnerViewModel.Total = results.Total;

            model.InnerView = results.Total > 0 ? "_SearchFlat" : "_SearchEmpty";
        }

        #endregion
    }
}