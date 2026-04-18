using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models; 

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class StudentController : BaseController
    {
       
        private readonly IStudentRepository _student;
        public StudentController(INotificationService notify,IStudentRepository student) :
            base(notify)
        {
            _student = student;
        }

        [Authorize(Roles = "Student")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult Watch()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult MyCourses()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult Subscription()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult Profile()
        {
            return View();
        } 
        #region Student

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var students = await _student.GetAllWithDetails();
            return View(students);
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Student model)
        {
            if (!ModelState.IsValid)
            {
                await LoadGender();
                return View(model);
            }
            int userId = UserHelper.GetUserId(User);
            if(model.ImageFile != null)
            {
                using (var ms = new MemoryStream())
                {
                    await model.ImageFile.CopyToAsync(ms);
                    model.ProfileImage=ms.ToArray();
                }
            }
            var studentDTO = new StudentDTO
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
            var result = await _student.AddAsync(studentDTO);

            if (result)
            {
                _notify.Success("Student saved successfully ! ");
                return RedirectToAction("Index");
            }

            await LoadGender();
            _notify.Error("Some Error Occured while saving details!");
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _student.GetByIdAsync(id);
            if (student == null) return NotFound();
             
            var studentDTO = new Student
            {
                StudentId = student.StudentId,
                UserID = student.UserID,
                FullName = student.FullName,
                DOB = student.DOB,
                Email = student.Email,
                Gender = student.Gender,
                Address = student.Address,
                Mobile = student.Mobile,
                ProfileImage = student.ProfileImage,
                IsActive = student.IsActive,
            };
            await LoadGender();
            return View(studentDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Student model)
        {
            if (ModelState.IsValid)
            {
                int userId = UserHelper.GetUserId(User);
                if (model.ImageFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.ImageFile.CopyToAsync(ms);
                        model.ProfileImage = ms.ToArray();
                    }
                }
                else
                {
                    var existingData = await _student.GetByIdAsync((int)model.UserID); 
                    model.ProfileImage = existingData?.ProfileImage;
                }

                var studentDTO = new StudentDTO
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
                    UpdatedBy = userId
                };
                var result = await _student.UpdateAsync(studentDTO);
                if (!result)
                {
                    await LoadGender();
                    _notify.Error("An Error Occures While Updating Section Details !"); 
                    return View(model);
                }

                if (result)
                {
                    _notify.Success("Student Profile updated successfully!"); 
                    return RedirectToAction("Index");
                } 
            }
            await LoadGender();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _student.GetByIdAsync(id);
            var studentDTO = new Student
            {
                FullName = student.FullName,
                UserID = student.UserID
            };
            return View(studentDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int UserID)
        {
            int userId = UserHelper.GetUserId(User);
            var student = await _student.GetByIdAsync(UserID);
            student.IsDeleted = true;
            student.UpdatedBy = userId;
            var result = await _student.DeleteAsync(student);
            _notify.Success("Student Profile Delete successfully!");
            return RedirectToAction("Index");
        } 

        #endregion
    }
}
