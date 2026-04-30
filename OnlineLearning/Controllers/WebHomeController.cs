using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.BusinessLogics.Repository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Views.Services;

namespace OnlineLearning.Controllers
{
    public class WebHomeController : Controller
    {
        private readonly ICourseRepository _course;
        private readonly ICourseCategoryRepository _courseCategory;
        private readonly ISubscriptionPlanRepository _plan;
        private readonly ICourseSectionRepository _section;
        private readonly IPQJQuestionRepository  _pqj;
        private readonly ProtectorService _protect;
        private readonly IMemoryCache _cache;
        public WebHomeController(IMemoryCache cache,ProtectorService protect, IPQJQuestionRepository pqj,ICourseSectionRepository section, ICourseRepository course, ICourseCategoryRepository courseCategory, ISubscriptionPlanRepository plan)
        {
            _cache = cache;
            _pqj = pqj;
            _section = section;
            _protect = protect;
            _course = course;
            _courseCategory = courseCategory;
            _plan = plan;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _courseCategory.GetAllWithDetails();
            var courses = await _course.GetAllWithDetails();
            var Plan = await _plan.GetAllWithDetails();
            var model = new HomeVM
            {
                Categories = categories.Select(c => new CourseCategoriesDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CourseCount = courses.Count(x => x.CategoryId == c.CategoryId)
                }).Take(4).ToList(),
                Courses = courses.Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.CourseTitle,
                    Thumbnail = c.Thumbnail,
                    Price = c.Price,
                    TotalLectures = c.TotalLectures,
                    LevelName = c.LevelName,
                    CategoryId = c.CategoryId
                }).Take(6).ToList(),
                Plans = Plan.Select(c => new SubscriptionPlanDTO
                {
                    PlanId = c.PlanId,
                    PlanName = c.PlanName,
                    Description = c.Description,
                    Price = c.Price,
                    DurationInDays = c.DurationInDays
                }).Take(3).ToList()
            };

            return View(model);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult ContactUS()
        {
            return View();
        }
        
        public async Task<IActionResult> Courses(string search, int? categoryId)
        {
            var courses = await _course.GetAllWithDetails();
            var categories = await _courseCategory.GetAllWithDetails();
            if (!string.IsNullOrWhiteSpace(search))
            {
                courses = courses.Where(c =>
                    c.CourseTitle.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                courses = courses.Where(c => c.CategoryId == categoryId).ToList();
            }
             
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CourseList", courses);
            }

            var model = new HomeVM
            {
                Courses = courses,
                Categories = categories.Select(c => new CourseCategoriesDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                }).ToList()
            };

            return View(model);
        } 
         
        public async Task<IActionResult> CourseDetails(string id)
        {
            try
            {
                int did = _protect.Decrypt(id);
                var course = await _course.GetCourseDetailsById(did);

                if (course == null)
                    return NotFound();

                return View(course);
            }
            catch(Exception ex)
            {
                return NotFound();
            }
            
        }
        public IActionResult Demos()
        {
            return View();
        }
        public IActionResult Faq()
        {
            return View();
        }
        public async Task<IActionResult> PQJ()
        {
            var courses = await _course.GetAllWithDetails();
            var model = new PQJPlayerVM
            {
                Courses = courses 
            }; 
            return View(model);
        }
        public async Task<IActionResult> GetTopicsByCourse(int courseId)
        {
            var topics = await _section.GetByCourseId(courseId); 
            return Json(topics.Select(t => new {
                t.SectionId,
                t.SectionTitle
            }));
        }
       
        public async Task<IActionResult> LoadQuestion(int index, int courseId, int topicId, int difficulty)
        {
            string cacheKey = $"PQJ_{courseId}_{topicId}_{difficulty}";
             
            if (!_cache.TryGetValue(cacheKey, out List<QuestionVM> questions))
            {
                questions = await _pqj.GetFilteredQuestions(courseId, topicId, difficulty);

                if (!questions.Any())
                    return Content("<p>No questions found</p>");
                 
                _cache.Set(cacheKey, questions, TimeSpan.FromMinutes(10));
                 
                var attemptId = await _pqj.StartTrial(Convert.ToInt32(courseId));
                HttpContext.Items["AttemptId"] = attemptId;  
            }
             
            if (index <= 0 || index > questions.Count)
                return Content("<p>Invalid index</p>");

            var question = questions[index - 1];

            var model = new PQJPlayerVM
            {
                Question = question,
                CurrentIndex = index,
                TotalQuestions = questions.Count,
                TimeLeft = "09:24"
            };

            return PartialView("_PQJPlayer", model);
        } 

        [HttpPost]
        public async Task<IActionResult> SaveAnswer([FromBody] SaveAnswerDTO dto)
        {
            var attemptId = HttpContext.Session.GetInt32("ATTEMPT");

            if (attemptId == null)
                return BadRequest();

            dto.AttemptId = attemptId.Value; 
            await _pqj.SaveAnswer(dto);

            return Ok();
        }
        public IActionResult Plans()
        {
            return View();
        }
    }
}
