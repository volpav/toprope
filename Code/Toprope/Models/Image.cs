namespace Toprope.Models
{
    /// <summary>
    /// Represents an image.
    /// </summary>
    public class Image
    {
        #region Properties

        /// <summary>
        /// Gets or sets the URL of the image.
        /// </summary>
        public string Url { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Image() { }

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <returns>A string representation of the given object.</returns>
        public override string ToString()
        {
            return Url;
        }
    }
}