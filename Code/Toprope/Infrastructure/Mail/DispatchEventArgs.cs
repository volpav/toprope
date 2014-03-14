using System;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Provides information about the message being dispatched.
    /// </summary>
    public class DispatchEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the message being dispatched.
        /// </summary>
        public System.Net.Mail.MailMessage Message { get; private set; }

        /// <summary>
        /// Gets or sets the log entry associated with the given dispatch operation.
        /// </summary>
        public LogEntry Event { get; private set; }

        /// <summary>
        /// Gets value indicatin whether message was successfully dispatched.
        /// </summary>
        public bool Success
        {
            get { return Event == null || Event.Level != LogEntryLevel.Error; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="message">Message being dispatched.</param>
        public DispatchEventArgs(System.Net.Mail.MailMessage message) : this(message, null) { }

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="message">Message being dispatched.</param>
        /// <param name="event">The log entry associated with the given dispatch operation.</param>
        public DispatchEventArgs(System.Net.Mail.MailMessage message, LogEntry @event)
        {
            this.Message = message;
            this.Event = @event;
        }
    }
}
