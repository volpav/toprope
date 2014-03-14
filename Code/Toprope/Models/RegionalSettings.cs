using System.Linq;

namespace Toprope.Models
{
    /// <summary>
    /// Represents a regional settings.
    /// </summary>
    public class RegionalSettings
    {
        #region Properties

        private string _languageCode = string.Empty;
        private System.Globalization.CultureInfo _languageCulture = null;

        /// <summary>
        /// Gets or sets two-letter ISO 639-1 langauge code.
        /// </summary>
        public string LanguageCode
        {
            get { return _languageCode; }
            set
            {
                _languageCode = value;
                _languageCulture = null;
            }
        }

        /// <summary>
        /// Gets the culture information for the currently specified language.
        /// </summary>
        public System.Globalization.CultureInfo LanguageCulture
        {
            get
            {
                string cultureName = string.Empty;
                Infrastructure.Configuration.ConfigurationFile config = null;

                if (_languageCulture == null)
                {
                    if (!string.IsNullOrEmpty(LanguageCode))
                    {
                        config = Infrastructure.Configuration.ConfigurationFile.Open();

                        foreach (Infrastructure.Configuration.CultureConfiguration culture in config.Localization.Languages.OfType<Infrastructure.Configuration.CultureConfiguration>())
                        {
                            if (string.Compare(culture.Shortcut, LanguageCode, System.StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                cultureName = culture.CultureName;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(cultureName))
                            _languageCulture = Infrastructure.Utilities.Input.GetCulture(cultureName);
                    }

                    if (_languageCulture == null)
                        _languageCulture = System.Globalization.CultureInfo.CurrentUICulture;
                }

                return _languageCulture;
            }
        }

        /// <summary>
        /// Gets or sets the route grade system to use.
        /// </summary>
        public RouteGradeSystem Grades { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public RegionalSettings() { }
    }
}