using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class StudyMaterialController : Controller
    {
        private readonly ICourseRepository _course;
        private readonly IReadingQuestionRepository _questionRepo; 
        private readonly ICourseSectionRepository _section;
        private readonly ICourseSubSectionRepository _subSectionRepo;
        public StudyMaterialController(ICourseRepository cousre,IReadingQuestionRepository questionRepo, ICourseSectionRepository section, ICourseSubSectionRepository subSectionRepo)
        {
            _course = cousre;
            _questionRepo = questionRepo;
            _section = section;
            _subSectionRepo = subSectionRepo;
        }
         
        public async Task<IActionResult> Index()
        {
            var data = await _questionRepo.GetAllAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ReadingQuestionVM();
            var courses = await _course.GetAllAsync();

            ViewBag.Courses = new SelectList(
                courses.Select(x => new SelectListItem
                {
                    Value = x.CourseId.ToString(),
                    Text = x.CourseTitle
                }),
                "Value",  "Text" );
            ViewBag.Sections = new SelectList(new List<SelectListItem>(), "Value", "Text");
            return PartialView("_ReadingQuestionForm", vm);
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReadingQuestionVM model, IFormFile? DiagramFile, int CorrectOptionIndex)
        {
            try
            { 
                if (model.Options != null && model.Options.Count > CorrectOptionIndex)
                {
                    for (int i = 0; i < model.Options.Count; i++)
                    {
                        model.Options[i].IsCorrect = (i == CorrectOptionIndex);
                    }
                }
                 
                if (DiagramFile != null && DiagramFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/diagrams");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + DiagramFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await DiagramFile.CopyToAsync(fileStream);
                    }
                    model.DiagramImageUrl = "/uploads/diagrams/" + uniqueFileName;
                }
                 
                var result = await _questionRepo.AddAsync(model);

                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSectionsByCourseId(int courseId)
        {
            var sections = await _section.GetByCourseId(courseId);
            var sectionList = sections.Select(x => new {
                id = x.SectionId,
                title = x.SectionTitle
            });
            return Json(sectionList);
        }
        [HttpGet]
        public async Task<IActionResult> GetSubSectionsBysectionId(int sectionId)
        {
            var subSections = await _subSectionRepo.GetAllAsync(sectionId);

            var sectionList = subSections.Select(x => new {
                id = x.SubSectionId,
                title = x.SubSectionTitle
            });
            return Json(sectionList);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Invalid Question ID!" });

            var model = await _questionRepo.GetByIdAsync(id);
            if (model == null) return Json(new { success = false, message = "Question not found!" });
             
            var courses = await _course.GetAllAsync();
            ViewBag.Courses = new SelectList(
                courses.Select(x => new SelectListItem
                {
                    Value = x.CourseId.ToString(),
                    Text = x.CourseTitle
                }),
                "Value", "Text", model.CourseId);
             
            var section = await _section.GetByCourseId(model.CourseId);
            ViewBag.Sections = new SelectList(
              section.Select(x => new SelectListItem
              {
                  Value = x.SectionId.ToString(),
                  Text = x.SectionTitle
              }),
              "Value", "Text", model.SectionId);  

            return PartialView("_ReadingQuestionForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReadingQuestionVM model, IFormFile? DiagramFile, int CorrectOptionIndex)
        {
            try
            {
                if (model.Options != null && model.Options.Count > CorrectOptionIndex)
                {
                    for (int i = 0; i < model.Options.Count; i++)
                    {
                        model.Options[i].IsCorrect = (i == CorrectOptionIndex);
                    }
                }

                if (DiagramFile != null && DiagramFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/diagrams");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + DiagramFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await DiagramFile.CopyToAsync(fileStream);
                    }
                    model.DiagramImageUrl = "/uploads/diagrams/" + uniqueFileName;
                }
                 
                var result = await _questionRepo.UpdateAsync(model);

                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var model = new ReadingQuestionVM { QuestionId = id };
                var result = await _questionRepo.DeleteAsync(model);

                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}