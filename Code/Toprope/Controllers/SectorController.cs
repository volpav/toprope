﻿using System.Web.Mvc;

namespace Toprope.Controllers
{
    /// <summary>
    /// Represents a sector controller.
    /// </summary>
    public class SectorController : Infrastructure.LocalizableController
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SectorController() { }

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

            ret = System.Text.RegularExpressions.Regex.Replace(ret, "/sector/", "/sectors/", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return ret;
        }
    }
}
