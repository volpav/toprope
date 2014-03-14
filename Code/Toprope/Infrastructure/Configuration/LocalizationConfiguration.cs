using System.Configuration;

namespace Toprope.Infrastructure.Configuration
{
    /// <summary>
    /// Represents a localization settings.
    /// </summary>
    public class LocalizationConfiguration : ConfigurationSection
    {
        #region Properties

        /// <summary>
        /// Gets the collection of supported languages.
        /// </summary>
        [ConfigurationProperty("languages", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CultureConfigurationCollection), AddItemName = "add")]
        public CultureConfigurationCollection Languages
        {
            get
            {
                CultureConfigurationCollection ret = new CultureConfigurationCollection();

                if (base["languages"] != null)
                    ret = (CultureConfigurationCollection)base["languages"];

                return ret;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public LocalizationConfiguration() { }
    }
}