using System.Collections.Generic;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a logger that writes to a single text file.
    /// </summary>
    public class SingleTextFileLogger : FileLoggerBase
    {
        #region Properties

        /// <summary>
        /// Gets the physical path to the log file.
        /// </summary>
        public string Path { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="path">The physical path to the log file.</param>
        public SingleTextFileLogger(string path)
        {
            this.Path = (path ?? string.Empty).Trim();

            if (!string.IsNullOrEmpty(Path) && !Path.EndsWith(".txt", System.StringComparison.InvariantCultureIgnoreCase))
                Path += ".txt";
        }

        /// <summary>
        /// Logs all entries.
        /// </summary>
        /// <param name="entries">Entries to log.</param>
        public override void Log(IEnumerable<LogEntry> entries)
        {
            base.Log(entries, Path);    
        }
    }
}
