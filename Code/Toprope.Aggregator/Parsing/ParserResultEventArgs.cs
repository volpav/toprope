using System;
using System.Collections.Generic;

namespace Toprope.Aggregator.Parsing
{
    /// <summary>
    /// Provides information about parser result.
    /// </summary>
    public class ParserResultEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the parsed areas.
        /// </summary>
        public IEnumerable<Parsing.ParsedArea> Areas { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        /// <param name="areas">Parsed areas.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="areas">areas</paramref> is null.</exception>
        public ParserResultEventArgs(IEnumerable<Parsing.ParsedArea> areas)
        {
            if (areas == null)
                throw new ArgumentNullException("areas");
            else
                this.Areas = areas;
        }
    }
}
