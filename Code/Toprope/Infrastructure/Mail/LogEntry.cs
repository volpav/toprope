using System;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a log entry level.
    /// </summary>
    public enum LogEntryLevel
    {
        /// <summary>
        /// Information.
        /// </summary>
        Information = 0,

        /// <summary>
        /// Warning.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error.
        /// </summary>
        Error = 3
    }

    /// <summary>
    /// Represents a log entry.
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        #region Properties

        /// <summary>
        /// Gets or sets the log entry level.
        /// </summary>
        public LogEntryLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the contents of the log entry.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time that corresponds to the contents of the given log entry.
        /// </summary>
        public DateTime Timestamp { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public LogEntry() : this(string.Empty, LogEntryLevel.Information) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="message">Log entry message.</param>
        public LogEntry(string message) : this(message, LogEntryLevel.Information) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="message">Log entry message.</param>
        /// <param name="level">Log entry level.</param>
        public LogEntry(string message, LogEntryLevel level)
        {
            this.Level = level;
            this.Message = message;
            this.Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Writes the message using the given text writer.
        /// </summary>
        /// <param name="writer">Text writer.</param>
        public virtual void WriteTo(System.IO.TextWriter writer)
        {
            if (writer != null && !string.IsNullOrEmpty(Message))
                writer.WriteLine("[{0}] [{1}] {2}", Timestamp.ToString("R"), Enum.GetName(typeof(LogEntryLevel), Level), Message);
        }
    }
}
