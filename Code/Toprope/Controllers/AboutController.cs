using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents an about controller.
    /// </summary>
    public class AboutController : Infrastructure.LocalizableController
    {
        /// <summary>
        /// Serves the default content.
        /// </summary>
        /// <returns>The default content.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Serves the content of "Search" view.
        /// </summary>
        /// <returns>The content of "Search" view.</returns>
        public ActionResult Search()
        {
            return View();
        }

    }
}
