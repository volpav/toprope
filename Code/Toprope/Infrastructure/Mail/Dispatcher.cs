using System;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents a message dispatcher.
    /// </summary>
    public abstract class Dispatcher
    {
        #region Events

        /// <summary>
        /// Occurs when the next message is being dispatched.
        /// </summary>
        public event EventHandler<DispatchEventArgs> Dispatching;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public Settings Settings { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected Dispatcher() { }

        /// <summary>
        /// Dispatches the given messages.
        /// </summary>
        /// <param name="messages">Messages to dispatch.</param>
        public abstract void Dispatch(IEnumerable<System.Net.Mail.MailMessage> messages);

        /// <summary>
        /// Raises "Dispatching" event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnDispatching(DispatchEventArgs e)
        {
            if (Dispatching != null)
                Dispatching(this, e);
        }
    }
}
