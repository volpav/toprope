using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents an area controller.
    /// </summary>
    public class AreaController : Infrastructure.LocalizableController
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public AreaController() { }

        /// <summary>
        /// Serves the default view.
        /// </summary>
        /// <returns>The default view.</returns>
        public ActionResult Index()
        {
            return RedirectPermanent(FixUrl());
        }

        /// <summary>
        /// Serves the the details view.
        /// </summary>
        /// <returns>The the details view.</returns>
        public ActionResult Details()
        {
            return RedirectPermanent(FixUrl());
        }

        /// <summary>
        /// Fixes the URL by replacing the misspelled subpath.
        /// </summary>
        /// <returns>Fixed URL.</returns>
        private string FixUrl()
        {
            string ret = ControllerContext.HttpContext.Request.Url.PathAndQuery;

            if (!ret.StartsWith("/"))
                ret = "/" + ret;

            ret = System.Text.RegularExpressions.Regex.Replace(ret, "/area/", "/areas/",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return ret;
        }

    }
}
