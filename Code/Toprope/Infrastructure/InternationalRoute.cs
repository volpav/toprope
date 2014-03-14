using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Toprope.Infrastructure
{
    /// <summary>
    /// Represents a rounte that automatically converts references to localized website versions so they are consistent.
    /// </summary>
    public class InternationalRoute : LowercaseRoute
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public InternationalRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use for any parameters that are missing in the URL.</param>
        public InternationalRoute(string url, object defaults) : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler()) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use for any parameters that are missing in the URL.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        public InternationalRoute(string url, object defaults, object constraints) : base(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler()) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use for any parameters that are missing in the URL.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public InternationalRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public InternationalRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="dataTokens">Custom values that are passed to the route handler, but which are not used to determine whether the route matches a specific URL pattern. These values are passed to the route handler, where they can be used for processing the request.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public InternationalRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        /// <summary>
        /// Returns information about the URL that is associated with the route.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the requested route.</param>
        /// <param name="values">An object that contains the parameters for a route.</param>
        /// <returns>An object that contains information about the URL that is associated with the route.</returns>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            int originalLength = 0;
            string defaultLanguagePrefix = string.Empty;
            Dictionary<string, string> languageReplacements = null;
            VirtualPathData path = base.GetVirtualPath(requestContext, values);
            Configuration.ConfigurationFile config = Configuration.ConfigurationFile.Open();

            if (path != null)
            {
                defaultLanguagePrefix = config.Localization.Languages.GetDefault().Shortcut + "/";

                languageReplacements = new Dictionary<string, string>();

                foreach (Configuration.CultureConfiguration culture in config.Localization.Languages)
                    languageReplacements.Add(culture.CultureName, culture.Shortcut);

                foreach (string locale in languageReplacements.Keys)
                {
                    originalLength = path.VirtualPath.Length;

                    path.VirtualPath = Regex.Replace(path.VirtualPath, string.Format("(/?){0}(/?)", 
                        Regex.Escape(locale)), (m) => {
                            return string.Format("{0}{1}{2}", m.Groups[1].Value, languageReplacements[locale], m.Groups[2].Value).Trim(); 
                        }, RegexOptions.IgnoreCase);

                    if (originalLength != path.VirtualPath.Length)
                        break;
                }

                // Ommitting default language (English) in URLs
                if (path.VirtualPath.StartsWith(defaultLanguagePrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (path.VirtualPath.Length == defaultLanguagePrefix.Length)
                        path.VirtualPath = "/";
                    else
                        path.VirtualPath = path.VirtualPath.Substring(defaultLanguagePrefix.Length);
                }
            }

            return path;
        }
    }
}