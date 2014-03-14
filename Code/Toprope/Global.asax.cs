using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;

namespace Toprope
{
    /// <summary>
    /// Represents an application.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Fires on application start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);

            Infrastructure.Application.Current.Initialize();
        }

        /// <summary>
        /// Handles application "PostReleaseRequestState" event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Application_PostReleaseRequestState(object sender, System.EventArgs e)
        {
            System.Web.HttpApplication app = sender as System.Web.HttpApplication;

            if (Infrastructure.Layout.ResponseFilterChain.CanApply(app.Request))
                app.Response.Filter = new Infrastructure.Layout.ResponseFilterChain(app.Context, app.Response.Filter);
        }

        /// <summary>
        /// Registers global filters.
        /// </summary>
        /// <param name="filters">Filters collection.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// Registers routes.
        /// </summary>
        /// <param name="routes">Route collection.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            string allLanguages = string.Empty;
            string defaultLanguageCulture = string.Empty;
            Infrastructure.Configuration.ConfigurationFile config = Infrastructure.Configuration.ConfigurationFile.Open();

            defaultLanguageCulture = config.Localization.Languages.GetDefault().CultureName;
            allLanguages = string.Join("|", config.Localization.Languages.OfType<Infrastructure.Configuration.CultureConfiguration>().Select(c => string.Format("({0})|({1})", c.Shortcut, c.CultureName)));

            #region General

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            // Route for errors
            routes.Add("Errors", new Infrastructure.InternationalRoute("errors/{id}",
                new { controller = "Errors", action = "Details", language = defaultLanguageCulture }));

            // Route for errors (compatibility)
            routes.Add("ErrorsCompatible", new Infrastructure.InternationalRoute("error/{id}",
                new { controller = "Errors", action = "Details", language = defaultLanguageCulture }));

            #endregion

            #region Sectors

            // Route for sectors
            routes.Add("Sectors", new Infrastructure.InternationalRoute("sectors/{id}",
                new { controller = "Sectors", action = "Details", language = defaultLanguageCulture }));

            // Route for sectors (compatibility)
            routes.Add("SectorsCompatible", new Infrastructure.InternationalRoute("sector/{id}",
                new { controller = "Sectors", action = "Details", language = defaultLanguageCulture }));

            // Route for sectors (localized)
            routes.Add("SectorsLocalized", new Infrastructure.InternationalRoute("{language}/sectors/{id}",
                new { controller = "Sectors", action = "Details", language = defaultLanguageCulture },
                new { language = allLanguages }));

            #endregion

            #region Areas

            // Route for areas
            routes.Add("Areas", new Infrastructure.InternationalRoute("areas/{id}",
                new { controller = "Areas", action = "Details", language = defaultLanguageCulture }));

            // Route for areas (compatibility)
            routes.Add("AreasCompatible", new Infrastructure.InternationalRoute("area/{id}",
                new { controller = "Areas", action = "Details", language = defaultLanguageCulture }));

            // Route for areas (localized)
            routes.Add("AreasLocalized", new Infrastructure.InternationalRoute("{language}/areas/{id}",
                new { controller = "Areas", action = "Details", language = defaultLanguageCulture },
                new { language = allLanguages }));

            #endregion

            #region Misc

            // Route for localization
            routes.Add("Localization", new Infrastructure.InternationalRoute("{language}/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", language = defaultLanguageCulture, id = UrlParameter.Optional },
                new { language = allLanguages }));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            #endregion
        }

        /// <summary>
        /// Registers bundles.
        /// </summary>
        /// <param name="bundles">Bundles.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*bundles.Add(new ScriptBundle("~/js").Include(
                "~/Scripts/App.js"
            ));*/

            bundles.Add(new StyleBundle("~/css").Include(
                "~/Content/Styles/Basic.css",
                "~/Content/Styles/Main.css",
                "~/Content/Styles/Responsive.css"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}