using System.Collections.Generic;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a mail logger.
    /// </summary>
    public abstract class Logger
    {
        #region Properties

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public Settings Settings { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected Logger() { }

        /// <summary>
        /// Logs all entries.
        /// </summary>
        /// <param name="entries">Entries to log.</param>
        public abstract void Log(IEnumerable<LogEntry> entries);
    }
}
