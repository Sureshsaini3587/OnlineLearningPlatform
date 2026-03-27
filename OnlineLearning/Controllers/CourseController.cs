using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;

namespace OnlineLearning.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseRepository _course;

        public CourseController(ICourseRepository cousre)
        {
            _course = cousre;
        }
         
        public async Task<IActionResult> Index()
        {
            var courses = await _course.GetAllWithDetails(); 
            return View(courses);
        }
         
        public IActionResult Create()
        {
            //LoadDropdowns();
            return View();
        }
         
        [HttpPost]
        public async Task<IActionResult> Create(Course model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedOn = DateTime.Now;
                model.IsActive = true;
                model.IsDeleted = false;

              var result = await _course.AddAsync(model);
                if (!result)
                    ViewBag.Message = "An Error Occured  While Saving !";

                return RedirectToAction("Index");
            }
            //LoadDropdowns();
            return View(model);
        }
         
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _course.GetByIdAsync(id);
            if (course == null) return NotFound();

            //LoadDropdowns();
            return View(course);
        }
         
        [HttpPost]
        public async Task<IActionResult> Edit(Course model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatedOn = DateTime.Now;

               var result= await _course.UpdateAsync(model);
                if (!result)
                    ViewBag.Message = "An Error Occures While Updating Course Details !";
                return RedirectToAction("Index");
            }

            //LoadDropdowns();
            return View(model);
        }
         
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _course.GetByIdAsync(id);
            return View(course);
        }
         
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int courseId)
        {
            var course = await _course.GetByIdAsync(courseId);
            course.IsDeleted = true;
            var result = await _course.DeleteAsync(course); 

            return RedirectToAction("Index");
        } 
        //private void LoadDropdowns()
        //{
        //    ViewBag.Categories = _context.CourseCategories
        //        .Where(x => x.IsActive && !x.IsDeleted)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.CategoryId.ToString(),
        //            Text = x.CategoryName
        //        }).ToList();

        //    ViewBag.Levels = _context.CourseLevels
        //        .Where(x => x.IsActive && !x.IsDeleted)
        //        .Select(x => new SelectListItem
        //        {
        //            Value = x.LevelId.ToString(),
        //            Text = x.LevelName
        //        }).ToList();
        //}
    }
}