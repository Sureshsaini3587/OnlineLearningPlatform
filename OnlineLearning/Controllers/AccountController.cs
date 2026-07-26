using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.BusinessLogics.Services;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    public class AccountController : Controller 
    {
        private readonly IDataProtector _protector;
        private readonly IAuthRepository _authRepository; 
        private readonly IEmailSender _emailSender; 
        public AccountController(IAuthRepository authRepository, IDataProtectionProvider provider, IEmailSender emailSender) 
        {
            _authRepository = authRepository; 
            _protector = provider.CreateProtector("AccountController");
            _emailSender = emailSender;
        }
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
             
            var user = await _authRepository.RegisterUser(model);

            if (user != null)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role,user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Home");
                else if (user.Role == "Student")
                    return RedirectToAction("Dashboard", "Student");
                else if (user.Role == "Teacher")
                    return RedirectToAction("Dashboard", "Instructor");
            }
             
            ModelState.AddModelError(string.Empty, "Registration failed. Email or Mobile number might already be in use.");
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _authRepository.LoginUserAsync(model);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid Email or Username.";
                ModelState.AddModelError("Email", "Invalid Email or Username.");
                return View(model);
            }
            bool valid = PasswordHelper.VerifyPassword(model.Password,user.PasswordHash);
            if (!valid)
            {
                ModelState.AddModelError("Password", "Invalid Password.");
                TempData["ErrorMessage"] = "Invalid Password.";
                return View(model);
            }

            if (user != null && valid)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role,user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
                var principal = new ClaimsPrincipal(identity); 
                await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme,  principal);
                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Home"); 
                else if (user.Role == "Student")
                    return RedirectToAction("Dashboard", "Student"); 
                else if (user.Role == "Teacher")
                    return RedirectToAction("Dashboard", "Instructor"); 
            }

            TempData["ErrorMessage"]  = "Invalid Login";
            return View();

        }

        public async Task<IActionResult> Logout()
        { 
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        } 

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var login = new LoginViewModel();
            login.Email= model.Email;
            var user = await _authRepository.LoginUserAsync(login);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid Email.");
                TempData["ErrorMessage"] = "User Not Found"; 
                return View(model);
            }
            string plainTokenData = $"{user.UserId}|{DateTime.UtcNow.Ticks}";
             
            string secureToken = _protector.Protect(plainTokenData);
            string id = _protector.Protect(user.UserId.ToString());
             
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { user = id, key = secureToken }, protocol: Request.Scheme);
            string emailBody = $@"
            <div style='background-color: #f8fafc; padding: 40px 10px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; min-height: 100%;'>
                <div style='max-width: 560px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.06);'>
                    
                    <!-- Premium Accent Top Border -->
                    <div style='height: 6px; background: linear-gradient(90deg, #4f46e5 0%, #06b6d4 100%);'></div>
                    
                    <!-- Main Content Area -->
                    <div style='padding: 40px; text-align: left;'>
                         
                        <div style='margin-bottom: 32px;'>
                            <a href='https://manikalearning.com/' style='text-decoration: none; display: inline-block;'>
                                <span style='font-size: 1.1rem; font-weight: 700; color: #1e293b; letter-spacing: -0.025em;'>
                                    🎓 Manika Learning Academy
                                </span>
                            </a>
                        </div>
                        
                        <h2 style='color: #0f172a; font-size: 1.5rem; font-weight: 700; margin: 0 0 16px 0; letter-spacing: -0.025em;'>
                            Reset your password
                        </h2>
                        
                        <p style='color: #475569; font-size: 0.95rem; line-height: 1.6; margin: 0 0 24px 0;'>
                            Hello,
                        </p>
                        
                        <p style='color: #475569; font-size: 0.95rem; line-height: 1.6; margin: 0 0 32px 0;'>
                            We received a request to reset the password associated with your learning portal account. Click the secure button below to choose a new, strong password:
                        </p>
                         
                        <div style='margin-bottom: 32px; text-align: center;'>
                            <a href='{callbackUrl}' style='display: inline-block; background-color: #4f46e5; color: #ffffff !important; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: 600; font-size: 0.95rem; box-shadow: 0 4px 10px rgba(79, 70, 229, 0.2);'>
                                Reset My Password
                            </a>
                        </div>
                        
                        <!-- Technical Expiry Notice -->
                        <div style='background-color: #f1f5f9; border-left: 4px solid #cbd5e1; padding: 16px; border-radius: 0 8px 8px 0; margin-bottom: 32px;'>
                            <p style='color: #64748b; font-size: 0.85rem; line-height: 1.5; margin: 0;'>
                                <strong>Security Notice:</strong> This single-use recovery link is highly secure and will expire automatically in <strong>2 hours</strong>. If you didn't initiate this request, your account is perfectly safe and you can safely disregard this email.
                            </p>
                        </div>
                         
                        <hr style='border: 0; border-top: 1px solid #e2e8f0; margin: 32px 0;'> 
                    </div>
                     
                    <div style='background-color: #fafafa; border-top: 1px solid #f1f5f9; padding: 24px 40px; text-align: center;'>
                        <p style='color: #94a3b8; font-size: 0.75rem; margin: 0; line-height: 1.5;'>
                            © {DateTime.UtcNow.Year} <a href='https://manikalearning.com/' style='color: #94a3b8; text-decoration: underline;'>Manika Learning Academy</a>. All rights reserved.<br>
                            This is an automated operational security transmission. Please do not reply directly to this mailbox.
                        </p>
                    </div>
                    
                </div>
            </div>";

            await _emailSender.SendEmailAsync(model.Email, "Reset Your Learning Portal Password", emailBody);
            TempData["SuccessMessage"] = "If your account exists, a secure password reset link has been sent to your email.";
            return View();
        }
         
        [HttpGet]
        public IActionResult ResetPassword(string user, string key)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(key))
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { UserId = user, Token = key };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            { 
               
                model.UserId = _protector.Unprotect(model.UserId);
                if (!int.TryParse(model.UserId, out int cleanUserId))
                {
                    TempData["ErrorMessage"]="Invalid User Identification Identity Matrix.";
                    return RedirectToAction("Login");
                }
                 
                bool isUpdated = await _authRepository.UpdatePasswordAsync(cleanUserId, model.ConfirmPassword);

                if (isUpdated)
                {
                    TempData["SuccessMessage"] = "Your password has been reset successfully. Please login with your new credentials.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to update password. Account might be inactive.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while resetting the password.";
                return View(model);
            }
        }
    }
}
