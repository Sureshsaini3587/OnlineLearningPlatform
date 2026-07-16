using OnlineLearning.BusinessLogics.IRepository;
using System.Net;
using System.Net.Mail;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
         
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string senderMail = _configuration["Google:mail"];
            string appPassword = _configuration["Google:pswd"];
            if (!string.IsNullOrEmpty(appPassword))
            {
                appPassword = appPassword.Replace(" ", "");
            }

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderMail, appPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderMail, "Manika Learning Academy"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
