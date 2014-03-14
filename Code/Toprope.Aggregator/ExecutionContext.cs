namespace Toprope.Aggregator
{
    /// <summary>
    /// Represents an execution context.
    /// </summary>
    public class ExecutionContext
    {
        #region Properties

        /// <summary>
        /// Gets or sets the parser.
        /// </summary>
        public Parsing.Parser Parser { get; set; }

        /// <summary>
        /// Gets or sets the writer.
        /// </summary>
        public Storage.Writer Writer { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ExecutionContext() { }
    }
}
