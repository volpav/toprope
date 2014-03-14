using System;

namespace Toprope.Aggregator.Parsing
{
    /// <summary>
    /// Provides trace information from the parser.
    /// </summary>
    public class ParserTraceEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="message">Message text.</param>
        public ParserTraceEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
