using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using System.Reflection;

namespace OnlineLearning.Controllers
{
    public class CourseVideoController : Controller
    {
        private readonly ICourseVideoRepository _video;
        private readonly ICourseSectionRepository _section;
        public CourseVideoController(ICourseVideoRepository video, ICourseSectionRepository section)
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseVideo model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
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

            if (result)
            {
                ViewBag.Success = "Section saved successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Something went wrong!";
            await LoadDropdowns();

            return View(model);
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
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CourseVideo model)
        {
            if (ModelState.IsValid)
            {
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
                if (!result)
                {
                    ViewBag.Error = "An Error Occures While Updating Section Details !";
                    await LoadDropdowns();
                    return View(model);
                }


                return RedirectToAction("Index");
            }
            await LoadDropdowns();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _video.GetByIdAsync(id);
            var CourseDTO = new CourseVideo
            {
                Title = course.Title,
                VideoId = course.VideoId
            };
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int VideoId)
        {
            var course = await _video.GetByIdAsync(VideoId);
            course.IsDeleted = true;
            var result = await _video.DeleteAsync(course);

            return RedirectToAction("Index");
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
