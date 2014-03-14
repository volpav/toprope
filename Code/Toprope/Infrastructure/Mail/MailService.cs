using System;
using System.Net;
using System.Linq;
using System.Net.Mail;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a mail service.
    /// </summary>
    public class MailService : IDisposable
    {
        #region Properties

        private Logger _logger = null;
        private Dispatcher _dispatcher = null;
        private Settings _settings = null;
        private bool _dispatcherSettingsInSync = true;
        private bool _loggerSettingsInSync = true;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public Logger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value;

                if (_logger != null && _logger.Settings == null)
                {
                    _loggerSettingsInSync = true;
                    _logger.Settings = Settings;
                }
                else
                    _loggerSettingsInSync = false;
            }
        }

        /// <summary>
        /// Gets or sets the dispatcher.
        /// </summary>
        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            set
            {
                if (_dispatcher != null)
                    _dispatcher.Dispatching -= Dispatcher_Dispatching;

                _dispatcher = value;

                if (_dispatcher != null)
                {
                    if (_dispatcher.Settings == null)
                    {
                        _dispatcherSettingsInSync = true;
                        _dispatcher.Settings = Settings;
                    }
                    else
                        _dispatcherSettingsInSync = false;

                    _dispatcher.Dispatching += Dispatcher_Dispatching;
                }
                else
                    _dispatcherSettingsInSync = true;
            }
        }

        /// <summary>
        /// Gets or sets the mail service settings.
        /// </summary>
        public Settings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;

                if (_logger != null && (_logger.Settings == null || _loggerSettingsInSync))
                    _logger.Settings = _settings;

                if (_dispatcher != null && (_dispatcher.Settings == null || _dispatcherSettingsInSync))
                    _dispatcher.Settings = _settings;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public MailService() : this(null, null, null) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// <param name="settings">Mail service settings.</param>
        /// </summary>
        public MailService(Settings settings) : this(null, null, settings) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="dispatcher">Dispatcher.</param>
        /// <param name="settings">Mail service settings.</param>
        public MailService(Dispatcher dispatcher, Settings settings) : this(dispatcher, null, settings) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="dispatcher">Dispatcher.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Mail service settings.</param>
        public MailService(Dispatcher dispatcher, Logger logger, Settings settings)
        {
            this.Settings = settings;
            this.Dispatcher = dispatcher;
            this.Logger = logger;
        }

        /// <summary>
        /// Sends out the given message.
        /// </summary>
        /// <param name="to">Message recipient.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="body">Message body.</param>
        public void Send(string to, string subject, string body)
        {
            Send(new string[] { to }, subject, body);
        }

        /// <summary>
        /// Sends out the given message.
        /// </summary>
        /// <param name="to">Message recipients.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="body">Message body.</param>
        public void Send(string[] to, string subject, string body)
        {
            Send(CreateMessage(Settings != null ? Settings[Settings.FromAddress] : string.Empty, to, subject, body));
        }

        /// <summary>
        /// Sends out the given message.
        /// </summary>
        /// <param name="from">Message sender.</param>
        /// <param name="to">Message recipient.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="body">Message body.</param>
        public void Send(string from, string to, string subject, string body)
        {
            Send(from, new string[] { to }, subject, body);
        }

        /// <summary>
        /// Sends out the given message.
        /// </summary>
        /// <param name="from">Message sender.</param>
        /// <param name="to">Message recipients.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="body">Message body.</param>
        public void Send(string from, string[] to, string subject, string body)
        {
            Send(CreateMessage(from, to, subject, body));
        }

        /// <summary>
        /// Queues the given message for delivary.
        /// </summary>
        /// <param name="message">Message to queue.</param>
        public void Send(MailMessage message)
        {
            if (Dispatcher != null)
                Dispatcher.Dispatch(new MailMessage[] { message });
            else
                Dispatcher_Dispatching(null, new DispatchEventArgs(message, new LogEntry("No dispatcher specified.", LogEntryLevel.Error)));
        }

        /// <summary>
        /// Disposes all managed resources used by this service.
        /// </summary>
        public void Dispose()
        {
            if (Logger != null && Logger is IDisposable)
                (Logger as IDisposable).Dispose();

            if (Dispatcher != null && Dispatcher is IDisposable)
                (Dispatcher as IDisposable).Dispose();
        }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="from">From address.</param>
        /// <param name="to">To addresses.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="body">Message body.</param>
        /// <returns>Mail message.</returns>
        protected MailMessage CreateMessage(string from, string[] to, string subject, string body)
        {
            MailMessage ret = null;

            if (!string.IsNullOrEmpty(from) && to != null && to.Any(a => !string.IsNullOrEmpty(a)))
            {
                ret = new MailMessage();

                ret.From = new MailAddress(from);

                foreach (string addr in to.Where(a => !string.IsNullOrEmpty(a)))
                    ret.To.Add(new MailAddress(addr));

                ret.Body = body;
                ret.IsBodyHtml = true;
                ret.BodyEncoding = System.Text.Encoding.UTF8;

                ret.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body,
                    new System.Net.Mime.ContentType("text/html; charset=UTF-8")));

                ret.Subject = subject;
                ret.SubjectEncoding = System.Text.Encoding.UTF8;
            }

            return ret;
        }

        /// <summary>
        /// Handles Dispatcher "Dispatching" event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Dispatcher_Dispatching(object sender, DispatchEventArgs e)
        {
            LogEntry entry = null;

            if (e != null && Logger != null)
            {
                if (e.Success)
                {
                    if (e.Event != null)
                        entry = e.Event;
                    else
                        entry = new LogEntry("Message has been successfully sent.", LogEntryLevel.Information);
                }
                else
                    entry = e.Event;

                if (entry != null)
                {
                    if (e.Message != null)
                    {
                        entry.Message += string.Format(" Message details (from, to[0], subject): ({0}, {1}, \"{2}\").",
                            e.Message.From.Address, e.Message.To.Any() ? e.Message.To.First().Address : "---", e.Message.Subject);
                    }

                    if (!string.IsNullOrEmpty(entry.Message))
                        entry.Message = entry.Message.Trim();

                    Logger.Log(new LogEntry[] { entry });
                }
            }
        }

        #region Static methods

        /// <summary>
        /// Returns the default mail service.
        /// </summary>
        /// <returns>Default mail service.</returns>
        public static MailService GetDefaultMailService()
        {
            MailService ret = null;
            string prefix = "Mail:";
            Settings settings = new Settings();
            LogOrganizeMode logMode = LogOrganizeMode.Daily;

            foreach (string key in System.Configuration.ConfigurationManager.AppSettings.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) && key.Length > prefix.Length)
                    settings[key.Substring(prefix.Length)] = System.Configuration.ConfigurationManager.AppSettings[key];
            }

            Enum.TryParse<LogOrganizeMode>(settings["LogMode"] ?? string.Empty, true, out logMode);

            ret = new MailService();

            ret.Settings = settings;
            ret.Dispatcher = new SmtpServerDispatcher();
            ret.Logger = new MultiTextFileLogger(settings["LogDirectory"], logMode);

            return ret;
        }

        #endregion
    }
}
