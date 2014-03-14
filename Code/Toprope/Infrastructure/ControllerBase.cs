using System.Web.Mvc;

namespace Toprope.Infrastructure
{
    /// <summary>
    /// Represents a base controller.
    /// </summary>
    public abstract class ControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        protected ControllerBase() { }

        /// <summary>
        /// Invokes the action in the current controller context.
        /// </summary>
        protected override void ExecuteCore()
        {
            Models.Feedback.FeedbackEntry entry = Models.Feedback.FeedbackEntry.CreateOnceFromRequest();

            if (entry != null && !string.IsNullOrWhiteSpace(entry.Subject) && !string.IsNullOrWhiteSpace(entry.Comment))
            {
                using (Infrastructure.Mail.MailService service = Infrastructure.Mail.MailService.GetDefaultMailService())
                    service.Send("volpav+toprope@gmail.com", entry.Subject, entry.Comment);
            }

            base.ExecuteCore();
        }
    }
}