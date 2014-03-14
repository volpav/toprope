using System;

namespace Toprope.Aggregator
{
    /// <summary>
    /// Represents an error log. This class cannot be inherited.
    /// </summary>
    public sealed class ErrorLog
    {
        /// <summary>
        /// Gets the error log location.
        /// </summary>
        private const string Location = @"C:\Toprope files\errorlog.txt";

        /// <summary>
        /// Writes a message to the log.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public static void Write(string message)
        {
            byte[] buffer = null;

            if (!string.IsNullOrEmpty(message))
            {
                using (System.IO.FileStream stream = new System.IO.FileStream(Location, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                {
                    buffer = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}: {1}\n", DateTime.Now.ToLongDateString(), message));
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// Writes an exception to the log.
        /// </summary>
        /// <param name="exception">Exception.</param>
        public static void Write(Exception exception)
        {
            string message = string.Empty;

            if (exception != null)
            {
                message = exception.Message;
                if (exception.InnerException != null && !string.IsNullOrEmpty(exception.InnerException.Message))
                    message = string.Concat(message, string.Format(" ({0})", exception.InnerException.Message));

                message = string.Concat(message, " ", exception.StackTrace);

                Write(message);
            }
        }
    }
}
