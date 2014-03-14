using System.Configuration;
using System.Web;

namespace Toprope.Infrastructure.Configuration
{
    /// <summary>
    /// Represents configuration file. This class cannot be inherited.
    /// </summary>
    public sealed class ConfigurationFile : ConfigurationSectionGroup
    {
        #region Properties

        /// <summary>
        /// Gets the localization settings.
        /// </summary>
        public LocalizationConfiguration Localization
        {
            get 
            {
                LocalizationConfiguration ret = (LocalizationConfiguration)base.Sections["localization"];

                if (ret == null)
                    ret = new LocalizationConfiguration();

                return ret; 
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        private ConfigurationFile() { }

        #region Static methods

        /// <summary>
        /// Opens configuration file.
        /// </summary>
        /// <returns>Platform configuration or null (Nothing in Visual Basic) if the platform configuration cannot be retrieved.</returns>
        public static ConfigurationFile Open()
        {
            ConfigurationFile ret = null;
            string key = "ConfigurationFile.Instance";
            System.Configuration.Configuration config = null;

            if (HttpContext.Current != null && HttpContext.Current.Items[key] != null)
                ret = HttpContext.Current.Items[key] as ConfigurationFile;

            if (ret == null)
            {
                try
                {
                    config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
                }
                catch (ConfigurationErrorsException) { }

                if (config != null)
                    ret = config.SectionGroups.Get("toprope") as ConfigurationFile;

                if (HttpContext.Current != null)
                    HttpContext.Current.Items[key] = ret;
            }

            return ret;
        }

        #endregion
    }
}