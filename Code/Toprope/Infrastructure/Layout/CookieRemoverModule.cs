using System;
using System.Web;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Represents a cookie remover module.
    /// </summary>
    public class CookieRemoverModule : IHttpModule
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public CookieRemoverModule() { }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        /// <param name="context">HTTP context.</param>
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += Application_PreSendRequestHeaders;
        }

        /// <summary>
        /// Disposes all resources used by this module.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Handles "PreSendRequestHeader" event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            string name = string.Empty;
            List<string> deletableCookies = new List<string>();

            foreach (object cookie in HttpContext.Current.Response.Cookies)
            {
                if (cookie is HttpCookie)
                    name = (cookie as HttpCookie).Name;
                else
                    name = cookie.ToString();

                if (name.IndexOf("session", StringComparison.InvariantCultureIgnoreCase) < 0)
                    deletableCookies.Add(name);
            }

            foreach (string cookieName in deletableCookies)
                HttpContext.Current.Response.Cookies.Remove(cookieName);
        }
    }
}
