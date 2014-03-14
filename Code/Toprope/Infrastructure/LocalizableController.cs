using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Toprope.Infrastructure
{
    /// <summary>
    /// Represents a controller that supports localization.
    /// </summary>
    public abstract class LocalizableController : ControllerBase
    {
        #region Properties

        private Infrastructure.Configuration.CultureConfiguration _defaultLanguage = null;

        /// <summary>
        /// Gets the default language settings,
        /// </summary>
        public Infrastructure.Configuration.CultureConfiguration DefaultLanguage
        {
            get
            {
                if (_defaultLanguage == null)
                    _defaultLanguage = Infrastructure.Configuration.ConfigurationFile.Open().Localization.Languages.GetDefault();

                return _defaultLanguage;
            }
        }

        /// <summary>
        /// Gets value indicating whether async support is disabled.
        /// </summary>
        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected LocalizableController() { }

        /// <summary>
        /// Redirects to the home page.
        /// </summary>
        /// <returns>Action result.</returns>
        protected ActionResult RedirectToHomePage()
        {
            string languageCode = Infrastructure.Application.Current.Session.RegionalSettings.LanguageCode;

            if (!string.IsNullOrEmpty(languageCode) && (string.Compare(languageCode, DefaultLanguage.Shortcut, System.StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(languageCode, DefaultLanguage.CultureName, System.StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                languageCode = string.Empty;
            }

            return new RedirectToRouteResult("Localization", new System.Web.Routing.RouteValueDictionary(new
            {
                controller = "Home",
                action = "Index",
                language = languageCode
            }), false);
        }

        /// <summary>
        /// Invokes the action in the current controller context.
        /// </summary>
        protected override void ExecuteCore()
        {
            CultureInfo culture = null;
            Models.RegionalSettings settings = null;
            System.Collections.Generic.IEnumerable<HttpCookie> cookies = null;

            settings = GetSettingsFromUrl(null);
            settings = GetSettingsFromCookie(settings);
            settings = GetSettingsFromUserAgentLanguage(settings);
            settings = GetSettingsFromFallbackScheme(settings);

            if (settings == null)
                settings = new Models.RegionalSettings();

            Infrastructure.Application.Current.Session.RegionalSettings = settings;

            if (settings != null)
            {
                culture = TryCreateCulture(settings.LanguageCode);

                if (culture != null)
                    Thread.CurrentThread.CurrentUICulture = culture;

                if (HttpContext != null && HttpContext.Response != null)
                {
                    cookies = CreateCookies(settings);

                    if (cookies != null)
                    {
                        foreach (HttpCookie cookie in cookies)
                            HttpContext.Response.SetCookie(cookie);
                    }
                }
            }

            base.ExecuteCore();
        }
        
        /// <summary>
        /// Creates a set of HTTP cookies that must be given to the client according to the given regional settings.
        /// </summary>
        /// <param name="settings">Regional settings.</param>
        /// <returns>A set of HTTP cookies.</returns>
        private System.Collections.Generic.IEnumerable<HttpCookie> CreateCookies(Models.RegionalSettings settings)
        {
            HttpCookie cookie = null;
            System.Collections.Generic.List<HttpCookie> ret = new System.Collections.Generic.List<HttpCookie>();

            Func<string, string, HttpCookie> createCookie = (name, value) =>
                {
                    HttpCookie result = null;

                    if (!string.IsNullOrWhiteSpace(name))
                        result = new HttpCookie(name, Infrastructure.Utilities.Input.GetString(value)) { Expires = DateTime.Now.AddYears(1) };

                    return result;
                };

            if (settings != null)
            {
                if (!string.IsNullOrWhiteSpace(settings.LanguageCode))
                {
                    cookie = createCookie("Toprope.Language", settings.LanguageCode);
                    
                    if (cookie != null)
                        ret.Add(cookie);
                }

                cookie = createCookie("Toprope.Grades", Enum.GetName(typeof(Models.RouteGradeSystem), settings.Grades));

                if (cookie != null)
                    ret.Add(cookie);
            }

            return ret;
        }

        /// <summary>
        /// Retrieves regional settings by using fallback scheme.
        /// </summary>
        /// <param name="mergeWith">An existing regional settings to merge with.</param>
        /// <returns>Regional settings.</returns>
        private Models.RegionalSettings GetSettingsFromFallbackScheme(Models.RegionalSettings mergeWith)
        {
            Models.RegionalSettings ret = null;

            Func<Models.RegionalSettings> getSettings = () =>
            {
                if (ret == null)
                    ret = Infrastructure.Application.Current.Session.RegionalSettings ?? new Models.RegionalSettings();

                return ret;
            };

            if (mergeWith != null && !string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                ret = mergeWith;
            else
            {
                if (mergeWith == null || string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                    getSettings().LanguageCode = DefaultLanguage.Shortcut;
                else
                    getSettings().LanguageCode = mergeWith.LanguageCode;

                if (mergeWith == null)
                    getSettings().Grades = Models.RouteGradeSystem.YDS;
                else
                    getSettings().Grades = mergeWith.Grades;
            }

            return ret;
        }

        /// <summary>
        /// Retrieves regional settings by using information about user agent's supported languages.
        /// </summary>
        /// <param name="mergeWith">An existing regional settings to merge with.</param>
        /// <returns>Regional settings.</returns>
        private Models.RegionalSettings GetSettingsFromUserAgentLanguage(Models.RegionalSettings mergeWith)
        {
            CultureInfo culture = null;
            string language = string.Empty;
            Models.RegionalSettings ret = null;

            Func<Models.RegionalSettings> getSettings = () =>
            {
                if (ret == null)
                    ret = Infrastructure.Application.Current.Session.RegionalSettings ?? new Models.RegionalSettings();

                return ret;
            };

            if (mergeWith != null && !string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                ret = mergeWith;
            else
            {
                if (HttpContext != null && HttpContext.Request != null)
                {
                    if (HttpContext.Request.UserLanguages != null && HttpContext.Request.UserLanguages.Length > 0)
                    {
                        language = HttpContext.Request.UserLanguages[0];

                        if (!string.IsNullOrWhiteSpace(language))
                        {
                            culture = TryCreateCulture(language);

                            if (culture != null)
                            {
                                if (mergeWith == null || string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                                    getSettings().LanguageCode = culture.TwoLetterISOLanguageName;
                                else
                                    getSettings().LanguageCode = mergeWith.LanguageCode;

                                if (mergeWith != null)
                                    getSettings().Grades = mergeWith.Grades;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Retrieves regional settings by using information from the PriceFlurry cookie.
        /// </summary>
        /// <param name="mergeWith">An existing regional settings to merge with.</param>
        /// <returns>Regional settings.</returns>
        private Models.RegionalSettings GetSettingsFromCookie(Models.RegionalSettings mergeWith)
        {
            string url = string.Empty;
            bool canOverrideLangauge = false;
            string urlLanguage = string.Empty;
            Models.RegionalSettings ret = null;
            string resolvedLanguage = string.Empty;

            Func<Models.RegionalSettings> getSettings = () =>
            {
                if (ret == null)
                    ret = Infrastructure.Application.Current.Session.RegionalSettings ?? new Models.RegionalSettings();

                return ret;
            };

            Func<string, string> getCookieValue = (name) =>
                {
                    string result = null;
                    HttpCookie cookie = null;

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        cookie = HttpContext.Request.Cookies[name];

                        if (cookie != null)
                            result = cookie.Value;
                    }

                    return result;
                };

            if (mergeWith != null && !string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                ret = mergeWith;
            else
            {
                if (HttpContext != null && HttpContext.Request != null)
                {
                    url = HttpContext.Request.Url.AbsolutePath.TrimStart('/');

                    if (mergeWith != null && !string.IsNullOrWhiteSpace(mergeWith.LanguageCode) && mergeWith.LanguageCulture != null)
                        resolvedLanguage = mergeWith.LanguageCulture.TwoLetterISOLanguageName;

                    if (!string.IsNullOrWhiteSpace(resolvedLanguage) && string.Compare(resolvedLanguage, url, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        canOverrideLangauge = true;

                        if (url.IndexOf('/') > 0)
                            urlLanguage = url.Substring(0, url.IndexOf('/'));
                        else
                            urlLanguage = url;

                        if (!string.IsNullOrWhiteSpace(urlLanguage) && string.Compare(urlLanguage, "me", StringComparison.InvariantCultureIgnoreCase) != 0 && (Regex.IsMatch(urlLanguage, "^[a-zA-Z]{2}$", RegexOptions.IgnoreCase) || Regex.IsMatch(urlLanguage, "^[a-zA-Z]{2}-[a-zA-Z]{2}$", RegexOptions.IgnoreCase)))
                            canOverrideLangauge = false;
                    }

                    if (mergeWith == null || string.IsNullOrWhiteSpace(mergeWith.LanguageCode) || canOverrideLangauge)
                    {
                        getSettings().LanguageCode = getCookieValue("Toprope.Language");

                        if (string.IsNullOrWhiteSpace(getSettings().LanguageCode) && mergeWith != null)
                            getSettings().LanguageCode = mergeWith.LanguageCode;
                    }
                    else
                        getSettings().LanguageCode = mergeWith.LanguageCode;

                    if (mergeWith == null)
                        getSettings().Grades = Infrastructure.Utilities.Input.GetEnum<Models.RouteGradeSystem>(getCookieValue("Toprope.Grades"));
                    else
                        getSettings().Grades = mergeWith.Grades;
                }
            }

            return ret;
        }

        /// <summary>
        /// Retrieves regional settings by using information from the current URL.
        /// </summary>
        /// <param name="mergeWith">An existing regional settings to merge with.</param>
        /// <returns>Regional settings.</returns>
        private Models.RegionalSettings GetSettingsFromUrl(Models.RegionalSettings mergeWith)
        {
            object language = null;
            CultureInfo culture = null;
            Models.RegionalSettings ret = null;
            string gradesParameter = string.Empty;
            
            Func<Models.RegionalSettings> getSettings = () =>
                {
                    if (ret == null)
                        ret = Infrastructure.Application.Current.Session.RegionalSettings ?? new Models.RegionalSettings();

                    return ret;
                };

            if (mergeWith != null && !string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                ret = mergeWith;
            else
            {
                language = RouteData.Values["language"];

                if (language != null && !string.IsNullOrWhiteSpace(language.ToString()))
                {
                    if (mergeWith == null || string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                    {
                        getSettings().LanguageCode = language.ToString();
                        if (getSettings().LanguageCode.Length > 2)
                        {
                            culture = TryCreateCulture(getSettings().LanguageCode);

                            if (culture != null)
                                getSettings().LanguageCode = culture.TwoLetterISOLanguageName;
                        }
                    }
                    else
                        getSettings().LanguageCode = mergeWith.LanguageCode;
                }
                else if (mergeWith != null && !string.IsNullOrWhiteSpace(mergeWith.LanguageCode))
                    getSettings().LanguageCode = mergeWith.LanguageCode;

                if (HttpContext != null && HttpContext.Request != null)
                {
                    if (mergeWith == null)
                    {
                        gradesParameter = HttpContext.Request["grades"];
                        if (!string.IsNullOrWhiteSpace(gradesParameter))
                            getSettings().Grades = Infrastructure.Utilities.Input.GetEnum<Models.RouteGradeSystem>(gradesParameter);
                    }
                    else
                        getSettings().Grades = mergeWith.Grades;
                }
                else if (mergeWith != null)
                    getSettings().Grades = mergeWith.Grades;
            }

            return ret;
        }

        /// <summary>
        /// Creates the CultureInfo object based on the given language code.
        /// </summary>
        /// <param name="languageCode">Two-letter language code.</param>
        /// <returns>Culture info.</returns>
        private CultureInfo TryCreateCulture(string languageCode)
        {
            CultureInfo ret = null;
            CultureInfo defaultCulture = !string.IsNullOrEmpty(DefaultLanguage.CultureName) ? CultureInfo.CreateSpecificCulture(DefaultLanguage.CultureName) : null;

            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                try
                {
                    ret = CultureInfo.CreateSpecificCulture(languageCode);
                }
                catch (CultureNotFoundException) { }
            }

            if (ret == null)
                ret = defaultCulture;

            return ret;
        }
    }
}