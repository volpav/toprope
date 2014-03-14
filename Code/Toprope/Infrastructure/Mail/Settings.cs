using System.Collections.Generic;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a collection of settings.
    /// </summary>
    public class Settings
    {
        #region Constants

        /// <summary>
        /// Gets the name of the setting that holds the "From" email address.
        /// </summary>
        public const string FromAddress = "FromAddress";

        /// <summary>
        /// Gets the name of the setting that holds the delivery method.
        /// </summary>
        public const string DeliveryMethod = "DeliveryMethod";

        /// <summary>
        /// Gets the name of the setting that holds the full path to the pickup directory.
        /// </summary>
        public const string PickupDirectory = "PickupDirectory";

        /// <summary>
        /// Gets the name of the setting that holds the IP-address of the dispatcher (usually, SMTP server).
        /// </summary>
        public const string Host = "Host";

        /// <summary>
        /// Gets the name of the setting that holds the port number of the target hots.
        /// </summary>
        public const string Port = "Port";

        /// <summary>
        /// Gets the name of the setting that holds the timeout (in milliseconds) before message sending fails.
        /// </summary>
        public const string Timeout = "Timeout";

        #endregion

        #region Properties

        private IDictionary<string, string> _data = null;

        /// <summary>
        /// Gets or sets the setting with the given key.
        /// </summary>
        /// <param name="key">Setting key.</param>
        /// <returns>Setting value.</returns>
        public string this[string key]
        {
            get { return _data.ContainsKey(key ?? string.Empty) ? _data[key] : null; }
            set
            {
                if (key != null)
                {
                    if (_data.ContainsKey(key))
                        _data[key] = value;
                    else
                        _data.Add(key, value);
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Settings()
        {
            _data = new Dictionary<string, string>();
        }
    }
}
