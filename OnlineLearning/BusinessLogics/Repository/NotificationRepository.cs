using NToastNotify;
using OnlineLearning.BusinessLogics.IRepository;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class NotificationService : INotificationService
    {
        private readonly IToastNotification _toast;

        public NotificationService(IToastNotification toast)
        {
            _toast = toast;
        }

        public void Success(string message)
        {
            _toast.AddSuccessToastMessage(message);
        }

        public void Error(string message)
        {
            _toast.AddErrorToastMessage(message);
        }

        public void Info(string message)
        {
            _toast.AddInfoToastMessage(message);
        }

        public void Warning(string message)
        {
            _toast.AddWarningToastMessage(message);
        }
    }
}
