using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Helpers;
using OnlineLearning.Models;

namespace OnlineLearning.Controllers
{  
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class CourseController : BaseController
    {
        private readonly ICourseLevelRepository _courseLevel;
        private readonly ICourseRepository _course;
        private readonly ICourseCategoryRepository _coursecategory;
        private readonly IWebHostEnvironment _env;

        public CourseController(INotificationService notify, ICourseRepository cousre, ICourseCategoryRepository coursecategory, ICourseLevelRepository courseLevel, IWebHostEnvironment env)
         :
            base(notify)
        {
            _course = cousre;
            _coursecategory = coursecategory;
            _courseLevel = courseLevel;
            _env = env;
        }

        #region Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _course.GetAllWithDetails(); 
            return View(courses);
        }

        [HttpGet] 
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return PartialView("_CourseForm", new Course());
        }
        [HttpGet]
        public IActionResult GetImage(string filename)
        {
            string folder = Path.Combine(_env.ContentRootPath, "Uploads/courses");
            string filePath = Path.Combine(folder, filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
             
            return PhysicalFile(filePath, "image/jpeg"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
            }
            try
            {
                if (model.ThumbnailFile != null)
                {
                    string folder = Path.Combine(_env.ContentRootPath, "Uploads/courses");
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string fileName = Guid.NewGuid() + Path.GetExtension(model.ThumbnailFile.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);

                    await model.ThumbnailFile.CopyToAsync(stream);

                    model.Thumbnail = fileName;
                }
                int userId = UserHelper.GetUserId(User);
                var CourseDTO = new CourseDTO
                {
                    CourseTitle = model.CourseTitle,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    Level = model.Level,
                    Language = model.Language,
                    InstructorId = model.InstructorId,
                    Thumbnail = model.Thumbnail,
                    Description = model.Description,
                    IsPublished = model.IsPublished,
                    IsActive = model.IsActive,
                    CreatedBy = userId
                };
                var result = await _course.AddAsync(CourseDTO);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error updating Course ID {Id}", model.CourseTitle); 
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _course.GetByIdAsync(id);
            if (course == null) return NotFound();

            await LoadDropdowns();
            var CourseDTO = new Course
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                Price = course.Price,
                CategoryId = course.CategoryId,
                Level = course.Level,
                Language = course.Language,
                InstructorId = course.InstructorId,
                Thumbnail = course.Thumbnail,
                Description = course.Description,
                IsPublished = course.IsPublished,
                IsActive = course.IsActive 
            }; 
            return PartialView("_CourseForm", CourseDTO); 
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill the data correctly!" });
            }
            try
            {
                var existingCourse = await _course.GetByIdAsync(model.CourseId);
                if (existingCourse == null)
                    return Json(new { success = false, message = "Course not found." });
                model.Thumbnail = existingCourse.Thumbnail;
                if (model.ThumbnailFile != null)
                {
                    string folder = Path.Combine(_env.ContentRootPath, "Uploads/courses");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                     
                    if (!string.IsNullOrEmpty(existingCourse.Thumbnail))
                    {
                        string oldPath = Path.Combine(folder, existingCourse.Thumbnail);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                     
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ThumbnailFile.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ThumbnailFile.CopyToAsync(stream);
                    }

                    model.Thumbnail = fileName; 
                }
                int userId = UserHelper.GetUserId(User); 
                var CourseDTO = new CourseDTO
                {
                    CourseId=model.CourseId,
                    CourseTitle = model.CourseTitle,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    Level = model.Level,
                    Language = model.Language,
                    InstructorId = model.InstructorId,
                    Thumbnail = model.Thumbnail,
                    Description = model.Description,
                    IsPublished = model.IsPublished,
                    IsActive = model.IsActive,
                    UpdatedBy = userId
                };
                var result= await _course.UpdateAsync(CourseDTO);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }
         
        
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int courseId)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
               var course = await _course.GetByIdAsync(courseId);
               course.IsDeleted = true;
               course.UpdatedBy = userId;
               var result = await _course.DeleteAsync(course);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
        private async Task LoadDropdowns()
        {
            var Categorys = await _coursecategory.GetAllAsync();
            ViewBag.Categories = Categorys
                .Select(x => new SelectListItem
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                }).ToList();
            var Level=await _courseLevel.GetAllAsync();
            ViewBag.Levels =Level 
                .Select(x => new SelectListItem
                {
                    Value = x.LevelId.ToString(),
                    Text = x.LevelName
                }).ToList();
            var Lang = await _course.GetLanguage();
            ViewBag.Language = Lang
                .Select(x => new SelectListItem
                {
                    Value = x.Name.ToString(),
                    Text = x.Name
                }).ToList();
            var Inst = await _course.GetInstructor();
            ViewBag.Instructor = Inst
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
        }

        #endregion

        #region Course Categories

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var courses = await _coursecategory.GetAllWithDetails();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> CategoriesCreate()
        {
           await LoadParentCategory();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CategoriesCreate(CourseCategories model)
        {
            if (!ModelState.IsValid)
            {
                await LoadParentCategory();
                return View(model);
            }
            int userId = UserHelper.GetUserId(User);
            var CourseDTO = new CourseCategoriesDTO
            { 
                CategoryName = model.CategoryName,
                ParentCategoryId = model.ParentCategoryId,
                IsParent = model.IsParent,
                IsActive = model.IsActive,
                CreatedBy=userId
            };
            var result = await _coursecategory.AddAsync(CourseDTO);
            return Json(new { success = result.Success, message = result.Message }); 
        }
        [HttpGet]
        public async Task<IActionResult> CategoryEdit(int id)
        {
            var course = await _coursecategory.GetByIdAsync(id);
            if (course == null) return NotFound();
             
            var CourseDTO = new CourseCategories
            {
                CategoryId = course.CategoryId,
                CategoryName = course.CategoryName,
                ParentCategoryId = course.ParentCategoryId,
                IsParent = course.IsParent,
                IsActive = course.IsActive  
            };

            await LoadParentCategory();
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryEdit(CourseCategories model)
        {
            if (ModelState.IsValid)
            {
                int userId = UserHelper.GetUserId(User);
                var CourseDTO = new CourseCategoriesDTO
                {
                    CategoryId = model.CategoryId,
                    CategoryName = model.CategoryName,
                    ParentCategoryId = model.ParentCategoryId,
                    IsParent = model.IsParent,
                    IsActive = model.IsActive,
                    UpdatedBy = userId
                };
                var result = await _coursecategory.UpdateAsync(CourseDTO);
                return Json(new { success = result.Success, message = result.Message }); 
            }

            await LoadParentCategory();
            return View(model);
        }
        public async Task<IActionResult> CategoryDelete(int id)
        {
            var course = await _coursecategory.GetByIdAsync(id);
            var CourseDTO = new CourseCategories
            {
                CategoryId = course.CategoryId,
                CategoryName= course.CategoryName,
            };
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryDeleteConfirmed(int CategoryId)
        {
            int userId = UserHelper.GetUserId(User);
            var course = await _coursecategory.GetByIdAsync(CategoryId);
            course.IsDeleted = true;
            course.UpdatedBy = userId;
            var result = await _coursecategory.DeleteAsync(course);
            _notify.Success("Course delete successfully!");
            return RedirectToAction("Index");
        }
        private async Task LoadParentCategory()
        {
            var Categorys = _coursecategory.GetAllAsync();
            ViewBag.Categories = Categorys.Result
                .Where(x=>x.IsParent)
                .Select(x => new SelectListItem
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                }).ToList(); 
        }
        #endregion
    }
}