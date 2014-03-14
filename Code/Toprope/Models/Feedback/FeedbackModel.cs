namespace Toprope.Models.Feedback
{
    /// <summary>
    /// Represents a feedback model.
    /// </summary>
    public class FeedbackModel
    {
        #region Properties

        private FeedbackEntry _entry = null;

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        public FeedbackEntry Entry
        {
            get
            {
                if (_entry == null)
                    _entry = new FeedbackEntry();

                return _entry;
            }
            set { _entry = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public FeedbackModel() { }
    }
}