using System;
using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents an errors controller.
    /// </summary>
    public class ErrorsController : Infrastructure.LocalizableController
    {
        /// <summary>
        /// Serves error details.
        /// </summary>
        /// <returns>View.</returns>
        public ActionResult Details()
        {
            Models.ErrorViewModel model = GetModel();

            Response.StatusCode = (int)model.Code;

            return View(model);
        }

        /// <summary>
        /// Returns a model.
        /// </summary>
        /// <returns>Model.</returns>
        private Models.ErrorViewModel GetModel()
        {
            Models.ErrorViewModel ret = new Models.ErrorViewModel();
            System.Net.HttpStatusCode code = System.Net.HttpStatusCode.InternalServerError;

            Enum.TryParse<System.Net.HttpStatusCode>(Infrastructure.Utilities.Input.GetString(base.RouteData.Values["id"]), out code);
            ret.Code = code;

            return ret;
        }
    }
}
