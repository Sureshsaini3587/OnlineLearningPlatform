using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly ISubscriberRepository _subscriberRepo;
        private readonly IEmailSender _emailSender;
        public NewsletterController(ISubscriberRepository subscriberRepo, IEmailSender emailSender)
        {
            _subscriberRepo = subscriberRepo;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe([FromForm] SubscribeRequest model)
        { 
            string returnUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "WebHome")!;
            }

            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return Redirect(returnUrl);
            }

            string cleanEmail = model.Email.Trim().ToLower();

            if (await _subscriberRepo.ExistsAsync(cleanEmail))
            {
                TempData["InfoMessage"] = "This email is already subscribed!";
                return Redirect(returnUrl);
            }

            var subscriber = new SubscriberDTO
            {
                Email = cleanEmail,
                SubscribedAt = DateTime.UtcNow,
                IsActive = true
            };

            bool isSaved = await _subscriberRepo.AddAsync(subscriber);

            if (!isSaved)
            {
                TempData["ErrorMessage"] = "Failed to save subscription. Try again.";
                return Redirect(returnUrl);
            }
             
            try
            {
                string emailBody = GetWelcomeEmailHtml(cleanEmail);
                await _emailSender.SendEmailAsync(cleanEmail, "Welcome to Manika Learning Academy!", emailBody);
            }
            catch
            { 
            }

            TempData["SuccessMessage"] = "Thank you for subscribing!";
            return Redirect(returnUrl);
        }
        private string GetWelcomeEmailHtml(string recipientEmail)
        {
            return $@"
             <!DOCTYPE html>
             <html lang=""en"">
             <head>
                 <meta charset=""UTF-8"">
                 <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                 <title>Welcome to Manika Learning Academy</title>
                 <!--[if mso]>
                 <style type=""text/css"">
                     body, table, td {{ font-family: Arial, Helvetica, sans-serif !important; }}
                 </style>
                 <![endif]-->
             </head>
             <body style=""margin: 0; padding: 0; background-color: #06090e; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; -webkit-font-smoothing: antialiased;"">
             
                 <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #06090e; padding: 40px 10px;"">
                     <tr>
                         <td align=""center"">
                             
                             <!-- Main Email Card -->
                             <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""max-width: 600px; background-color: #0c1319; border-radius: 20px; overflow: hidden; border: 1px solid #1e2d3b; box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.7);"">
                                 
                                 <!-- BRAND HEADER WITH CINEMATIC GRADIENT -->
                                 <tr>
                                     <td style=""background: linear-gradient(135deg, #0c1c24 0%, #11262d 100%); padding: 40px 30px; text-align: center; border-bottom: 1px solid #1e2d3b;"">
                                         <div style=""display: inline-block; background: rgba(45, 212, 191, 0.1); padding: 8px 18px; border-radius: 50px; border: 1px solid rgba(45, 212, 191, 0.3); margin-bottom: 16px;"">
                                             <span style=""color: #2dd4bf; font-weight: 700; font-size: 13px; letter-spacing: 1px;"">🟢 NEW ERA OF DIGITAL LEARNING</span>
                                         </div>
                                         <h1 style=""color: #ffffff; font-size: 28px; font-weight: 800; margin: 10px 0 0 0; line-height: 1.25; letter-spacing: -0.5px;"">
                                             Build skills through <span style=""color: #2dd4bf;"">cinematic</span> <span style=""color: #f3a17c;"">learning</span> &amp; <span style=""color: #c084fc;"">smart practice</span>.
                                         </h1>
                                     </td>
                                 </tr>
             
                                 <!-- HERO CONTENT -->
                                 <tr>
                                     <td style=""padding: 35px 30px 20px 30px; color: #f8fafc;"">
                                         <p style=""font-size: 16px; line-height: 1.6; color: #94a3b8; margin-top: 0;"">
                                             Welcome aboard 👋,
                                         </p>
                                         <p style=""font-size: 15px; line-height: 1.6; color: #cbd5e1;"">
                                             Thank you for subscribing to <strong style=""color: #2dd4bf;"">Manika Learning Academy</strong>! Watch demo classes for free, unlock complete premium courses, and boost your confidence with interactive PQJ exercises.
                                         </p>
             
                                         <!-- FEATURE CARDS (UI MATCH) -->
                                         <div style=""background-color: #121c24; border-radius: 14px; padding: 22px; margin: 28px 0; border: 1px solid #1e2d3b;"">
                                             <h3 style=""color: #2dd4bf; font-size: 14px; margin: 0 0 16px 0; letter-spacing: 1px; font-weight: 700; text-transform: uppercase;"">
                                                 ✦ Your Learning Journey Starts Here:
                                             </h3>
                                             
                                             <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
                                                 <tr>
                                                     <td width=""32"" vertical-align=""top"" style=""padding-bottom: 12px;"">
                                                         <span style=""color: #2dd4bf; font-size: 18px;"">●</span>
                                                     </td>
                                                     <td style=""padding-bottom: 12px; color: #94a3b8; font-size: 14px; line-height: 1.5;"">
                                                         <strong style=""color: #f1f5f9;"">Free Demo Access:</strong> Experience lessons before committing.
                                                     </td>
                                                 </tr>
                                                 <tr>
                                                     <td width=""32"" vertical-align=""top"" style=""padding-bottom: 12px;"">
                                                         <span style=""color: #f3a17c; font-size: 18px;"">●</span>
                                                     </td>
                                                     <td style=""padding-bottom: 12px; color: #94a3b8; font-size: 14px; line-height: 1.5;"">
                                                         <strong style=""color: #f1f5f9;"">UI Design &amp; Web Development:</strong> Structured, premium course paths.
                                                     </td>
                                                 </tr>
                                                 <tr>
                                                     <td width=""32"" vertical-align=""top"">
                                                         <span style=""color: #c084fc; font-size: 18px;"">●</span>
                                                     </td>
                                                     <td style=""color: #94a3b8; font-size: 14px; line-height: 1.5;"">
                                                         <strong style=""color: #f1f5f9;"">PQJ Performance Practice:</strong> Interactive exercises with instant results.
                                                     </td>
                                                 </tr>
                                             </table>
                                         </div>
             
                                         <!-- CTA BUTTON (GET STARTED STYLE) -->
                                         <table role=""presentation"" width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""margin: 30px 0 15px 0;"">
                                             <tr>
                                                 <td align=""center"">
                                                     <a href=""https://manikalearning.com/WebHome/Courses"" target=""_blank"" style=""background: linear-gradient(135deg, #2dd4bf 0%, #06b6d4 100%); color: #06090e; text-decoration: none; font-size: 15px; font-weight: 800; padding: 15px 36px; border-radius: 50px; display: inline-block; box-shadow: 0 0 20px rgba(45, 212, 191, 0.4); letter-spacing: 0.3px;"">
                                                         Get Started Free &rarr;
                                                     </a>
                                                 </td>
                                             </tr>
                                         </table>
             
                                     </td>
                                 </tr>
             
                                 <!-- FOOTER SECTION -->
                                 <tr>
                                     <td style=""background-color: #080d12; padding: 30px; text-align: center; border-top: 1px solid #1e2d3b;"">
                                         
                                         <!-- Social Links -->
                                         <div style=""margin-bottom: 20px;"">
                                             <a href=""https://www.youtube.com/@manikalearningacademy"" target=""_blank"" style=""text-decoration: none; display: inline-block; margin: 0 6px;"">
                                                 <span style=""background-color: #121c24; color: #ef4444; padding: 8px 16px; border-radius: 20px; font-size: 12px; border: 1px solid #1e2d3b; font-weight: 600;"">▶ YouTube</span>
                                             </a>
                                             <a href=""https://www.whatsapp.com/channel/0029Vb7XLuMLNSa6IBMp4N0G"" target=""_blank"" style=""text-decoration: none; display: inline-block; margin: 0 6px;"">
                                                 <span style=""background-color: #121c24; color: #22c55e; padding: 8px 16px; border-radius: 20px; font-size: 12px; border: 1px solid #1e2d3b; font-weight: 600;"">💬 WhatsApp</span>
                                             </a>
                                         </div>
             
                                         <p style=""color: #64748b; font-size: 12px; margin: 0 0 8px 0;"">
                                             Jaipur, Rajasthan, India
                                         </p>
                                         
                                         <p style=""color: #475569; font-size: 11px; line-height: 1.5; margin: 0;"">
                                             You received this email because you subscribed at <a href=""https://manikalearning.com"" style=""color: #2dd4bf; text-decoration: none;"">manikalearning.com</a> ({recipientEmail}).<br/>
                                             © 2026 Manika Learning Academy. All rights reserved.
                                         </p>
                                     </td>
                                 </tr>
             
                             </table>
                             
                         </td>
                     </tr>
                 </table>
             
             </body>
             </html>";
        }
    }
   
}
