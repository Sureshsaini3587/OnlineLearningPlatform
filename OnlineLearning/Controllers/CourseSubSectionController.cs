using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class CourseSubSectionController : BaseController
    {
        private readonly ICourseRepository _course;
        private readonly ICourseSectionRepository _section;
        private readonly ICourseSubSectionRepository _subSectionRepo;
        public CourseSubSectionController(INotificationService notify, ICourseRepository cousre, ICourseSectionRepository section, ICourseSubSectionRepository subSectionRepo) :
            base(notify)
        {
            _course = cousre;
            _section = section;
            _subSectionRepo = subSectionRepo;
        }

        public async Task<IActionResult> Index(int? sectionId)
        {
            var subSections = await _subSectionRepo.GetAllAsync(sectionId);

            if (sectionId.HasValue)
            {
                ViewBag.SectionId = sectionId.Value; 
                var Categorys = _section.GetAllAsync();
                ViewBag.Section = Categorys.Result
                    .Select(x => new SelectListItem
                    {
                        Value = x.SectionId.ToString(),
                        Text = x.SectionTitle
                    }).ToList();
            }

            return View(subSections);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id, int? sectionId)
        {
            SubSection model = new SubSection();

            if (id.HasValue && id > 0)
            {
                model = await _subSectionRepo.GetByIdAsync(id.Value);
                if (model == null) return NotFound();
            }
            else if (sectionId.HasValue)
            {
                model.SectionId = sectionId.Value;
            }
            var Categorys = _section.GetAllAsync();
            ViewBag.Sections = Categorys.Result
                .Select(x => new SelectListItem
                {
                    Value = x.SectionId.ToString(),
                    Text = x.SectionTitle
                }).ToList();
            return PartialView("_SubSectionForm",model);
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(SubSection model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
            }
           
                await _subSectionRepo.UpsertAsync(model);
                return Json(new { success = true, message = "Save Successfully" }); 
            
        }
         
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var subSection = await _subSectionRepo.GetByIdAsync(id);
                if (subSection != null)
                {
                    int sectionId = subSection.SectionId;
                    await _subSectionRepo.DeleteAsync(id);
                    return Json(new { success = true, message = "Delete Successfully" }); 
                }
                return Json(new { success = false, message = "An unexpected error to getting data "   });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
             
        }

    }
}
