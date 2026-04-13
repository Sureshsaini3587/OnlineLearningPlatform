using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Controllers
{
    public class WebHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult ContactUS()
        {
            return View();
        }
        public IActionResult Courses()
        {
            return View();
        }
        public IActionResult CourseDetails()
        {
            return View();
        }
        public IActionResult Demos()
        {
            return View();
        }
        public IActionResult Faq()
        {
            return View();
        }
    }
}
