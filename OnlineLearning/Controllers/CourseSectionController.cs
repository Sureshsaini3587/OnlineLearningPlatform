using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;

namespace OnlineLearning.Controllers
{
    public class CourseSectionController : BaseController
    {
        private readonly ICourseRepository _course;
        private readonly ICourseSectionRepository _section;
        public CourseSectionController(INotificationService notify, ICourseRepository cousre, ICourseSectionRepository section) :
            base(notify)
        {
            _course = cousre;
            _section = section; 
        } 

        #region Courses Section
        public async Task<IActionResult> Index()
        {
            var courses = await _section.GetAllWithDetails();
            return View(courses);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseSection model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }
            int userId = UserHelper.GetUserId(User);
            var CourseDTO = new CourseSectionDTO
            {
                SectionTitle = model.SectionTitle,
                CourseId = model.CourseId, 
                SortOrder = model.SortOrder, 
                IsActive = model.IsActive,
                CreatedBy = userId
            };
            var result = await _section.AddAsync(CourseDTO);

            if (result)
            {
                _notify.Success("Section saved successfully!");
                return RedirectToAction("Index");
            }

            _notify.Error("Something went wrong!");
            await LoadDropdowns();

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _section.GetByIdAsync(id);
            if (course == null) return NotFound();

            await LoadDropdowns();
            var CourseDTO = new CourseSection
            {
                SectionId = course.SectionId,
                CourseId = course.CourseId,
                SectionTitle = course.SectionTitle,
                SortOrder = course.SortOrder, 
                IsActive = course.IsActive
            };
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CourseSection model)
        {
            if (ModelState.IsValid)
            {
                int userId = UserHelper.GetUserId(User);
                var CourseDTO = new CourseSectionDTO
                {
                    CourseId = model.CourseId, 
                    SectionId = model.SectionId, 
                    SectionTitle = model.SectionTitle,
                    SortOrder = model.SortOrder,
                    IsActive = model.IsActive,
                    UpdatedBy = userId
                };
                var result = await _section.UpdateAsync(CourseDTO);
                if (!result)
                {
                    _notify.Error("An Error Occures While Updating Section Details !");
                    await LoadDropdowns();
                    return View(model);
                }

                _notify.Success("Section Details Update Successfully !");
                return RedirectToAction("Index");
            }
            await LoadDropdowns();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _section.GetByIdAsync(id);
            var CourseDTO = new CourseSection
            {
                SectionTitle = course.SectionTitle,
                SectionId = course.SectionId
            };
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int SectionId)
        {
            int userId = UserHelper.GetUserId(User);
            var course = await _section.GetByIdAsync(SectionId);
            course.IsDeleted = true;
            course.UpdatedBy = userId;
            var result = await _section.DeleteAsync(course);
            _notify.Success("Section Delete Successfully !");
            return RedirectToAction("Index");
        }
        private async Task LoadDropdowns()
        {
            var Categorys = _course.GetAllAsync();
            ViewBag.Course = Categorys.Result
                .Select(x => new SelectListItem
                {
                    Value = x.CourseId.ToString(),
                    Text = x.CourseTitle
                }).ToList(); 
        }

        #endregion
          
    }
}
