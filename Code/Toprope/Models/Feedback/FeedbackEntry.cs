namespace Toprope.Models.Feedback
{
    /// <summary>
    /// Represents a feedback entry.
    /// </summary>
    public class FeedbackEntry
    {
        #region Properties

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public FeedbackEntry() { }

        #region Properties

        /// <summary>
        /// Creates new feedback entry from the details submitted via current HTTP request. If the call to this method was already previously made, no entry is returned.
        /// </summary>
        /// <returns>Feedback entry.</returns>
        public static FeedbackEntry CreateOnceFromRequest()
        {
            FeedbackEntry ret = null;

            if (System.Web.HttpContext.Current != null)
            {
                if (!Toprope.Infrastructure.Utilities.Input.GetBool(System.Web.HttpContext.Current.Items["FeedbackEntry.Retrieved"]))
                {
                    ret = CreateFromRequest();
                    System.Web.HttpContext.Current.Items["FeedbackEntry.Retrieved"] = true.ToString();
                }
            }

            return ret;
        }

        /// <summary>
        /// Creates new feedback entry from the details submitted via current HTTP request.
        /// </summary>
        /// <returns>Feedback entry.</returns>
        public static FeedbackEntry CreateFromRequest()
        {
            FeedbackEntry ret = null;

            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
            {
                if (string.Compare(System.Web.HttpContext.Current.Request.HttpMethod, "post", System.StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    ret = new FeedbackEntry();

                    ret.Subject = System.Web.HttpContext.Current.Request.Form["feedback-subject"] ?? string.Empty;
                    ret.Comment = System.Web.HttpContext.Current.Request.Form["feedback-comment"] ?? string.Empty;
                }
            }

            return ret;
        }

        /// <summary>
        /// Creates a new feedback entry for a given area.
        /// </summary>
        /// <param name="area">Area.</param>
        /// <returns>Feedback entry.</returns>
        public static FeedbackEntry Create(Models.Area area)
        {
            return new FeedbackEntry() { Subject = area != null ? string.Format("[Area][{0},{1}]", area.Name, area.Id) : string.Empty };
        }

        /// <summary>
        /// Creates a new feedback entry for a given sector.
        /// </summary>
        /// <param name="sector">Sector.</param>
        /// <returns>Feedback entry.</returns>
        public static FeedbackEntry Create(Models.Sector sector)
        {
            return new FeedbackEntry() { Subject = sector != null ? string.Format("[Sector][{0},{1}]", sector.Name, sector.Id) : string.Empty };
        }

        #endregion
    }
}