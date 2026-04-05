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

        public CourseController(INotificationService notify, ICourseRepository cousre, ICourseCategoryRepository coursecategory, ICourseLevelRepository courseLevel)
         :
            base(notify)
        {
            _course = cousre;
            _coursecategory = coursecategory;
            _courseLevel = courseLevel;
        }

        #region Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _course.GetAllWithDetails(); 
            return View(courses);
        }
         
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Course model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }
            int userId = UserHelper.GetUserId(User);
            var CourseDTO = new CourseDTO
            {
                CourseTitle=model.CourseTitle,
                Price=model.Price,
                CategoryId=model.CategoryId,
                Level=model.Level,
                Language=model.Language,
                InstructorId=model.InstructorId,
                Thumbnail=model.Thumbnail,
                Description=model.Description,
                IsPublished=model.IsPublished,
                IsActive=model.IsActive,
                CreatedBy=userId
            };
            var result = await _course.AddAsync(CourseDTO);

            if (result)
            {
                _notify.Success("Course saved successfully!");
                return RedirectToAction("Index");
            }

            _notify.Error("Something went wrong!");
           await LoadDropdowns();

            return View(model);
        }

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
            return View(CourseDTO);
        }
         
        [HttpPost]
        public async Task<IActionResult> Edit(Course model)
        {
            if (ModelState.IsValid)
            { 
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
                if (!result)
                {
                    _notify.Error("An Error Occures While Updating Course Details !");
                    await LoadDropdowns();
                    return View(model);
                }

                _notify.Success("Course Update successfully!");
                return RedirectToAction("Index");
            } 
            await LoadDropdowns();
            return View(model);
        }
         
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _course.GetByIdAsync(id);
            var CourseDTO = new Course
            {
                CourseId = course.CourseId ,
                CourseTitle=course.CourseTitle 
            };
            return View(CourseDTO);
        }
         
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int courseId)
        {
            int userId = UserHelper.GetUserId(User);
            var course = await _course.GetByIdAsync(courseId);
            course.IsDeleted = true;
            course.UpdatedBy = userId;
            var result = await _course.DeleteAsync(course);
            _notify.Success("Course delete successfully!");
            return RedirectToAction("Index");
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

            if (result)
            {
                _notify.Success("Categories saved successfully!");
                return RedirectToAction("Categories");
            }

            _notify.Error("Something went wrong!");

            await LoadParentCategory();
            return View(model);
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
                if (!result)
                {
                    _notify.Error("An Error Occures While Updating Categories Details !"); 
                    return View(model);
                }
                _notify.Success("Categories Upadate Successfully ! ");
                return RedirectToAction("Categories");
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