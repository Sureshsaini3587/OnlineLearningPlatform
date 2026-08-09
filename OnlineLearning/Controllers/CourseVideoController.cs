using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using System.Reflection;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class CourseVideoController : BaseController
    {
        private readonly ICourseVideoRepository _video;
        private readonly ICourseSectionRepository _section;
        public CourseVideoController(INotificationService notify,ICourseVideoRepository video, ICourseSectionRepository section) :
            base(notify)
        {
            _video = video;
            _section = section; 
        } 
          
        #region Courses Video
        public async Task<IActionResult> Index()
        {
            var courses = await _video.GetAllWithDetails();
            return View(courses);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return PartialView("_VideoForm", new CourseVideo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseVideo model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
            }
            int userId = UserHelper.GetUserId(User);
            var CourseDTO = new CourseVideoDTO
            {
                Title = model.Title,
                SectionId = model.SectionId, 
                Duration = model.Duration, 
                VideoUrl = model.VideoUrl, 
                IsDemo = model.IsDemo, 
                SortOrder = model.SortOrder, 
                IsActive = model.IsActive,
                CreatedBy = userId
            };
            var result = await _video.AddAsync(CourseDTO);
            return Json(new { success = result.Success, message = result.Message });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _video.GetByIdAsync(id);
            if (course == null) return NotFound();

            await LoadDropdowns();
            var CourseDTO = new CourseVideo
            {
                VideoId = course.VideoId,
                Title = course.Title,
                SectionId = course.SectionId,
                Duration = course.Duration,
                VideoUrl = course.VideoUrl,
                IsDemo = course.IsDemo,
                SortOrder = course.SortOrder,
                IsActive = course.IsActive,
            }; 
            return PartialView("_VideoForm", CourseDTO); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseVideo model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
            }

            int userId = UserHelper.GetUserId(User);
                var CourseDTO = new CourseVideoDTO
                {
                    VideoId = model.VideoId,
                    Title = model.Title,
                    SectionId = model.SectionId,
                    Duration = model.Duration,
                    VideoUrl = model.VideoUrl,
                    IsDemo = model.IsDemo,
                    SortOrder = model.SortOrder,
                    IsActive = model.IsActive,
                    UpdatedBy = userId
                };
                var result = await _video.UpdateAsync(CourseDTO);
                return Json(new { success = result.Success, message = result.Message });
            
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int VideoId)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                var course = await _video.GetByIdAsync(VideoId);
                course.IsDeleted = true;
                course.UpdatedBy = userId;
                var result = await _video.DeleteAsync(course);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
           
        }
        private async Task LoadDropdowns()
        {
            var Categorys = _section.GetAllAsync();
            ViewBag.Section = Categorys.Result
                .Select(x => new SelectListItem
                {
                    Value = x.SectionId.ToString(),
                    Text = x.SectionTitle
                }).ToList(); 
        }

        #endregion



    }
}
