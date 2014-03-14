using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Toprope.Infrastructure
{
    /// <summary>
    /// Provides various URL extensions.
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Builds URL by finding the best matching route that corresponds to the current URL.
        /// </summary>
        /// <param name="helper">URL helper.</param>
        /// <returns>URL.</returns>
        public static IHtmlString Current(this UrlHelper helper)
        {
            return Current(helper, null);
        }

        /// <summary>
        /// Builds URL by finding the best matching route that corresponds to the current URL, with given parameters added or replaced.
        /// </summary>
        /// <param name="helper">URL helper.</param>
        /// <param name="substitutes">Substitutes.</param>
        /// <returns>URL.</returns>
        public static IHtmlString Current(this UrlHelper helper, object substitutes)
        {
            object value = null;
            NameValueCollection qs = helper.RequestContext.HttpContext.Request.QueryString;
            RouteValueDictionary rd = new RouteValueDictionary(helper.RequestContext.RouteData.Values);

            foreach (string param in qs)
            {
                if (!string.IsNullOrEmpty(qs[param]))
                    rd[param] = qs[param];
            }

            if (substitutes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
                {
                    value = property.GetValue(substitutes);

                    if (value == null || string.IsNullOrEmpty(value.ToString()))
                        rd.Remove(property.Name);
                    else
                        rd[property.Name] = value.ToString();
                }
            }

            return new MvcHtmlString(helper.RouteUrl(rd));
        }
    }
}