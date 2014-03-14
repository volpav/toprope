namespace Toprope.Models
{
    /// <summary>
    /// Represents an error view model.
    /// </summary>
    public class ErrorViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public System.Net.HttpStatusCode Code { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public ErrorViewModel() { }
    }
}