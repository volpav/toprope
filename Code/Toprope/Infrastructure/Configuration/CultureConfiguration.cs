using System.Configuration;

namespace Toprope.Infrastructure.Configuration
{
    /// <summary>
    /// Represents a culture configuration.
    /// </summary>
    public class CultureConfiguration : ConfigurationElement
    {
        #region Properties

        private static CultureConfiguration _default = null;

        /// <summary>
        /// Gets the default culture.
        /// </summary>
        public static CultureConfiguration Default
        {
            get
            {
                if (_default == null)
                    _default = new CultureConfiguration("en-US", "en");

                return _default;
            }
        }

        /// <summary>
        /// Gets or sets the name of the culture.
        /// </summary>
        [ConfigurationProperty("culture", IsRequired = true, IsKey = true)]
        public string CultureName
        {
            get { return base["culture"] != null ? base["culture"].ToString() : string.Empty; }
            set { base["culture"] = value; }
        }

        /// <summary>
        /// Gets or sets the culture shortcut.
        /// </summary>
        [ConfigurationProperty("shortcut", IsRequired = true, IsKey = false)]
        public string Shortcut
        {
            get { return base["shortcut"] != null ? base["shortcut"].ToString() : string.Empty; }
            set { base["shortcut"] = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether this culture is default.
        /// </summary>
        [ConfigurationProperty("default", IsRequired = false, IsKey = false)]
        public bool IsDefault
        {
            get { return base["default"] != null ? (bool)base["default"] : false; }
            set { base["shortcut"] = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public CultureConfiguration() : this(string.Empty, string.Empty) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="cultureName">Culture name.</param>
        /// <param name="shortcut">Shortcut.</param>
        public CultureConfiguration(string cultureName, string shortcut)
        {
            CultureName = cultureName;
            Shortcut = shortcut;
        }
    }
}