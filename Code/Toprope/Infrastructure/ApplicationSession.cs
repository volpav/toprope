using System.Web;

namespace Toprope.Infrastructure
{
    /// <summary>
    /// Represents a current user session. This class cannot be inherited.
    /// </summary>
    public sealed class ApplicationSession
    {
        #region Properties

        private System.Collections.Specialized.NameValueCollection _items = null;

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static ApplicationSession Current
        {
            get
            {
                ApplicationSession ret = null;
                string sessionKey = "Toprope.ApplicationSession";

                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session[sessionKey] == null || !(HttpContext.Current.Session[sessionKey] is ApplicationSession))
                        HttpContext.Current.Session[sessionKey] = new ApplicationSession();

                    ret = (ApplicationSession)HttpContext.Current.Session[sessionKey];
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets or sets the current regional settings.
        /// </summary>
        public Models.RegionalSettings RegionalSettings { get; set; }

        /// <summary>
        /// Gets or sets the collection of session items.
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Items
        {
            get
            {
                if (_items == null)
                    _items = new System.Collections.Specialized.NameValueCollection();

                return _items;
            }
            set { _items = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        private ApplicationSession()
        {
            RegionalSettings = new Models.RegionalSettings();
        }
    }
}