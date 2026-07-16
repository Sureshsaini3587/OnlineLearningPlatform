namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
