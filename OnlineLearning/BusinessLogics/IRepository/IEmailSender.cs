namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendContactEmailAsync(string userEmail, string userName, string subject, string message);
    }
}
