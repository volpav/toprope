using System.Web.Mvc;
using System.Web.Routing;

namespace Toprope.Infrastructure
{
    /// <summary>
    /// Represents a route whose virtual path is automatically put into the lower case.
    /// </summary>
    public class LowercaseRoute : System.Web.Routing.Route
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LowercaseRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use for any parameters that are missing in the URL.</param>
        public LowercaseRoute(string url, object defaults) : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler()) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use for any parameters that are missing in the URL.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        public LowercaseRoute(string url, object defaults, object constraints) : base(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler()) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use for any parameters that are missing in the URL.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LowercaseRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="dataTokens">Custom values that are passed to the route handler, but which are not used to determine whether the route matches a specific URL pattern. These values are passed to the route handler, where they can be used for processing the request.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LowercaseRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler) { }

        /// <summary>
        /// Returns information about the URL that is associated with the route.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the requested route.</param>
        /// <param name="values">An object that contains the parameters for a route.</param>
        /// <returns>An object that contains information about the URL that is associated with the route.</returns>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            int queryIndex = -1;
            string p = string.Empty;
            VirtualPathData path = base.GetVirtualPath(requestContext, values);

            if (path != null)
            {
                p = path.VirtualPath;

                if (!string.IsNullOrWhiteSpace(p))
                {
                    queryIndex = p.IndexOf('?');

                    if (queryIndex > 0)
                        path.VirtualPath = p.Substring(0, queryIndex).ToLowerInvariant() + p.Substring(queryIndex);
                    else
                        path.VirtualPath = p.ToLowerInvariant();
                }
            }

            return path;
        }
    }
}