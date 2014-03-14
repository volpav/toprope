using System;
using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents a sectors controller.
    /// </summary>
    public class SectorsController : Infrastructure.LocalizableController
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SectorsController() { }

        /// <summary>
        /// Serves the default view.
        /// </summary>
        /// <returns>The default view.</returns>
        public ActionResult Index()
        {
            return RedirectToHomePage();
        }

        /// <summary>
        /// Serves the the details view.
        /// </summary>
        /// <returns>The the details view.</returns>
        public ActionResult Details()
        {
            Models.SectorViewModel model = GetModel();

            return model != null && model.Area != null ? (ActionResult)View(model) : HttpNotFound();
        }

        /// <summary>
        /// Returns the sector model.
        /// </summary>
        /// <returns>Sector model.</returns>
        private Models.SectorViewModel GetModel()
        {
            return Models.SectorViewModel.Load(Infrastructure.Utilities.Input.GetGuid(base.RouteData.Values["id"]));
        }
    }
}
