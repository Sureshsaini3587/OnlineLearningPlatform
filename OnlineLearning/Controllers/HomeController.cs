using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using System.Diagnostics;

namespace OnlineLearning.Controllers
{
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IDashboardRepository dashboardRepo)
        {
            _logger = logger;
            _dashboardRepo = dashboardRepo;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalStudents = await _dashboardRepo.GetTotalStudentsAsync(),
                ActiveCourses = await _dashboardRepo.GetActiveCoursesAsync(),
                MonthlyRevenue = await _dashboardRepo.GetMonthlyRevenueAsync(),
                PendingQueries = await _dashboardRepo.GetPendingQueriesAsync(),
                RecentEnrollments = await _dashboardRepo.GetRecentEnrollmentsAsync(5),
                RevenueData = await _dashboardRepo.GetRevenueStatsAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
