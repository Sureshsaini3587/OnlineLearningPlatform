using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using System.Reflection;

namespace OnlineLearning.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _student; 
        public StudentController(IStudentRepository student)
        {
            _student = student; 
        }

        #region Student
        public async Task<IActionResult> Index()
        {
            var courses = await _student.GetAllWithDetails();
            return View(courses);
        }

        public async Task<IActionResult> Create()
        {
            await LoadGender();
            return View();
        }

        public async Task<IActionResult> LoadGender()
        {
            var list =await _student.GetGender();
            ViewBag.Gender = list.Select(x => new SelectListItem
            {
                Value = x.GenderName ,
                Text = x.GenderName
            }).ToList(); 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student model,IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                await LoadGender();
                return View(model);
            }
            int userId = UserHelper.GetUserId(User);
            if(ImageFile != null)
            {
                using (var ms = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(ms);
                    model.ProfileImage=ms.ToArray();
                }
            }
            var CourseDTO = new StudentDTO
            {
                FullName = model.FullName,
                DOB = model.DOB,
                Email = model.Email,
                Gender = model.Gender,
                Address = model.Address,
                Mobile = model.Mobile,
                ProfileImage=model.ProfileImage,
                IsActive = model.IsActive,
                CreatedBy = userId
            };
            var result = await _student.AddAsync(CourseDTO);

            if (result)
            {
                ViewBag.Success = "Plan saved successfully!";
                return RedirectToAction("Index");
            }

            await LoadGender();
            ViewBag.Error = "Something went wrong!";   
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _student.GetByIdAsync(id);
            if (course == null) return NotFound();
             
            var CourseDTO = new Student
            {
                StudentId = course.StudentId,
                UserID = course.UserID,
                FullName = course.FullName,
                DOB = course.DOB,
                Email = course.Email,
                Gender = course.Gender,
                Address = course.Address,
                Mobile = course.Mobile,
                ProfileImage = course.ProfileImage,
                IsActive = course.IsActive,
            };
            await LoadGender();
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student model,IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                int userId = UserHelper.GetUserId(User);
                if (ImageFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(ms);
                        model.ProfileImage = ms.ToArray();
                    }
                }
                var CourseDTO = new StudentDTO
                {
                    StudentId = model.StudentId,
                    UserID = model.UserID,
                    FullName = model.FullName,
                    DOB = model.DOB,
                    Email = model.Email,
                    Gender = model.Gender,
                    Address = model.Address,
                    Mobile = model.Mobile,
                    ProfileImage = model.ProfileImage,
                    IsActive = model.IsActive,
                    CreatedBy = userId
                };
                var result = await _student.UpdateAsync(CourseDTO);
                if (!result)
                {
                    await LoadGender();
                    ViewBag.Error = "An Error Occures While Updating Section Details !"; 
                    return View(model);
                }


                return RedirectToAction("Index");
            }
            await LoadGender();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _student.GetByIdAsync(id);
            var CourseDTO = new Student
            {
                FullName = course.FullName,
                UserID = course.UserID
            };
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int UserID)
        {
            var course = await _student.GetByIdAsync(UserID);
            course.IsDeleted = true;
            var result = await _student.DeleteAsync(course);

            return RedirectToAction("Index");
        } 

        #endregion
    }
}
