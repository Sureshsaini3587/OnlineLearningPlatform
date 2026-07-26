using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Views.Services;

namespace OnlineLearning.Controllers
{
    public class WebHomeController : Controller
    {
        private readonly ICourseRepository _course;
        private readonly ICourseVideoRepository _courseVideo;
        private readonly ICourseCategoryRepository _courseCategory;
        private readonly ISubscriptionPlanRepository _plan;
        private readonly ICourseSectionRepository _section;
        private readonly IPQJQuestionRepository  _pqj;
        private readonly ProtectorService _protect;
        private readonly IMemoryCache _cache;
        private readonly IEmailSender _mail;
        public WebHomeController(IMemoryCache cache, IEmailSender mail, ICourseVideoRepository courseVideo,ProtectorService protect, IPQJQuestionRepository pqj,ICourseSectionRepository section, ICourseRepository course, ICourseCategoryRepository courseCategory, ISubscriptionPlanRepository plan)
        {
            _cache = cache;
            _mail = mail;
            _pqj = pqj;
            _section = section;
            _protect = protect;
            _course = course;
            _courseVideo = courseVideo;
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

        [HttpGet]
        public IActionResult ContactUS()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> ContactUS(ContactViewModel model)
        { 
            if (!ModelState.IsValid)
            {
                return View(model);
            } 
            try
            { 
                await _mail.SendContactEmailAsync(
                    model.Email,
                    model.Name,
                    model.Subject,
                    model.Message
                );
                
                
                TempData["SuccessMessage"] = "Thank you! Your message has been sent successfully. Our team will get back to you soon.";
                 
                return RedirectToAction(nameof(ContactUS));
            }
            catch (System.Exception ex)
            { 
                ModelState.AddModelError(string.Empty, "Sorry, there was a problem sending your message. Please try again later.");
                return View(model);
            }
        }
        public async Task<IActionResult> Courses(string search, int? categoryId, bool isAjax = false)
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
            if (isAjax)
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
        public async Task<IActionResult> Demos()
        {
            var videoDTOs = await _courseVideo.GetAllDemosDetails();

            return View(videoDTOs);
        }
        public IActionResult Faq()
        {
            return View();
        }

        
        public IActionResult Privacy()
        {
            return View();
        }
         
        public IActionResult Terms()
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


        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> CoursePlans(int  courseId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var plans = await _course.GetPlansByCourse(courseId);

            return View(plans);
        }

        
        [HttpGet]
        public async Task<IActionResult> LoadQuestion(int index,  int courseId,  int topicId, int difficulty)
        {
            string cacheKey = $"PQJ_{courseId}_{topicId}_{difficulty}"; 
            if (!_cache.TryGetValue(  cacheKey, out List<QuestionVM> questions))
            {
                questions =
                    await _pqj.GetFilteredQuestions(  courseId, topicId, difficulty,"Trial");

                if (!questions.Any())
                {
                    return Content(
                        "<div class='alert alert-warning'>No questions found</div>");
                }

                _cache.Set(  cacheKey,   questions,  TimeSpan.FromMinutes(30));
            }
             
            if (index <= 0 ||
                index > questions.Count)
            {
                return Content(@"
                         <div class='text-center p-5'>
                             <h3>No More Questions</h3>
                             <p>You have reached the end of this PQJ set.</p>
                       
                             <button class='btn btn-plan mt-3'
                                     onclick='location.reload()'>
                                 Restart
                             </button>
                         </div>");
            }

            var question =
                questions[index - 1]; 

            int attemptId = await _pqj.GetOrCreateAttempt(courseId);

            var model = new PQJPlayerVM
            {
                AttemptId = attemptId,
                Question = question,
                CurrentIndex = index,
                TotalQuestions = questions.Count
            };

            return PartialView(
                "_PQJPlayer",
                model);
        }


        [HttpPost]
        [Route("PQJ/SaveAnswer")]
        public async Task<IActionResult> SaveAnswer( [FromBody] SaveAnswerDTO dto)
        {
            if (dto.AttemptId == null)
                return BadRequest();

            if (dto.QuestionId == null)
                return BadRequest();

            if (dto.SelectedOptionId == null)
                return BadRequest();

            var result = await _pqj.SaveAnswer(dto);

            return Json(new
            {
                success = true,
                isCorrect = result.IsCorrect, 
                correctOptionId = result.CorrectOptionId, 
                selectedOptionId =  dto.SelectedOptionId
            });
        }
        public IActionResult Plans()
        {
            return View();
        }


    }
}
