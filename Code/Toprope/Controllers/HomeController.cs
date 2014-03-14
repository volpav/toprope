using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents a home controller.
    /// </summary>
    public class HomeController : Infrastructure.LocalizableController
    {
        #region Properties

        private Infrastructure.Search.SearchQuery _query = null;
        private bool _queryParsed = false;

        /// <summary>
        /// Gets the amount of search results to skip.
        /// </summary>
        public int Skip
        {
            get { return Infrastructure.Utilities.Input.GetInt(Request.QueryString["skip"]); }
        }

        /// <summary>
        /// Gets the amount of search results to take.
        /// </summary>
        public int Take
        {
            get { return Infrastructure.Utilities.Input.GetInt(Request.QueryString["take"]); }
        }

        /// <summary>
        /// Gets the search query.
        /// </summary>
        public Infrastructure.Search.SearchQuery Query
        {
            get
            {
                if (_query == null && !_queryParsed)
                {
                    _query = Infrastructure.Search.SearchQuery.Parse(Request.QueryString["q"]);
                    _queryParsed = true;
                }

                return _query;
            }
        }

        #endregion

        /// <summary>
        /// Serves the default content.
        /// </summary>
        /// <returns>The default content.</returns>
        public ActionResult Index()
        {
            ViewResult ret = null;

            if (Query != null)
                ret = View("Search", GetModel());
            else
                ret = View();

            return ret;
        }

        /// <summary>
        /// Returns a view.
        /// </summary>
        /// <param name="name">View name.</param>
        /// <param name="model">View model.</param>
        /// <returns>View.</returns>
        public IView GetView(string name, object model)
        {
            return View(name, model).View;
        }

        /// <summary>
        /// Returns search results view model.
        /// </summary>
        /// <returns>Search results view model.</returns>
        private Models.Search.SearchResultsViewModel GetModel()
        {
            return Models.Search.SearchResultsViewModel.Load(this);
        }
    }
}
