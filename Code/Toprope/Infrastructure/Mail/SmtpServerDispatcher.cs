using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Toprope.Infrastructure.Mail
{
    /// <summary>
    /// Represents an SMTP server dispatcher.
    /// </summary>
    public class SmtpServerDispatcher : Dispatcher
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public SmtpServerDispatcher() { }

        /// <summary>
        /// Dispatches the given messages.
        /// </summary>
        /// <param name="messages">Messages to dispatch.</param>
        public override void Dispatch(IEnumerable<System.Net.Mail.MailMessage> messages)
        {
            SmtpClient client = null;
            int port = 0, timeout = 0;
            System.Exception error = null;
            string errorMessage = string.Empty;
            SmtpDeliveryMethod deliveryMethod = SmtpDeliveryMethod.Network;
            
            if (messages != null && messages.Any(m => m != null))
            {
                client = new SmtpClient();

                if(Settings != null)
                    Enum.TryParse<SmtpDeliveryMethod>(Settings[Settings.DeliveryMethod] ?? string.Empty, true, out deliveryMethod);

                client.DeliveryMethod = deliveryMethod;
                
                if (Settings != null)
                {
                    if (!string.IsNullOrEmpty(Settings[Settings.Host]))
                        client.Host = Settings[Settings.Host];

                    if (!string.IsNullOrEmpty(Settings[Settings.PickupDirectory]))
                        client.PickupDirectoryLocation = Settings[Settings.PickupDirectory];

                    if (int.TryParse(Settings[Settings.Port] ?? string.Empty, out port))
                        client.Port = port;

                    if (int.TryParse(Settings[Settings.Timeout] ?? string.Empty, out timeout))
                        client.Timeout = timeout;
                }

                if (client.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory &&
                    (string.IsNullOrWhiteSpace(client.PickupDirectoryLocation) || !System.IO.Directory.Exists(client.PickupDirectoryLocation)))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                }

                foreach (MailMessage message in messages.Where(m => m != null))
                {
                    error = null;

                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception e) { error = e; }

                    if (error != null)
                    {
                        errorMessage = error.Message;

                        if (error.InnerException != null)
                            errorMessage += string.Format(" ({0})", error.InnerException.Message);

                        errorMessage = string.Format("Unable to send message: {0}.", errorMessage);
                        errorMessage = errorMessage.Trim().Trim('.') + ".";
                    }

                    OnDispatching(new DispatchEventArgs(message, error != null ? new LogEntry(errorMessage, LogEntryLevel.Error) : null));
                }
            }
        }
    }
}
