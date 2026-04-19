using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.Services;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepository;

        public AccountController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _authRepository.LoginUserAsync(model);
            if (user == null)
            {
                ViewBag.Message = "Invalid User";
                return View();
            }
            bool valid = PasswordHelper.VerifyPassword(
                       model.Password,
                       user.PasswordHash);
            if (!valid)
            {
                ViewBag.Message = "Invalid Password";
                return View();
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

            ViewBag.Message = "Invalid Login";
            return View();

        }

        public async Task<IActionResult> Logout()
        { 
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
