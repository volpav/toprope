using System;
using System.Web;

namespace Toprope.Infrastructure.Layout
{
    /// <summary>
    /// Represents a server modifier module.
    /// </summary>
    public class ServerHeaderModifierModule : IHttpModule
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ServerHeaderModifierModule() { }

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
            HttpContext.Current.Response.Headers.Remove("Server");
        }
    }
}
