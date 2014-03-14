using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents areas controller.
    /// </summary>
    public class AreasController : Infrastructure.LocalizableController
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public AreasController() { }

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
            Models.AreaViewModel model = GetModel();

            return model != null ? (ActionResult)View(model) : HttpNotFound();
        }

        /// <summary>
        /// Returns the sector model.
        /// </summary>
        /// <returns>Sector model.</returns>
        private Models.AreaViewModel GetModel()
        {
            return Models.AreaViewModel.Load(Infrastructure.Utilities.Input.GetGuid(base.RouteData.Values["id"]));
        }
    }
}
