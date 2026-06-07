using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using OnlineLearning.Views.Services;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class StudentController : BaseController
    {
        private readonly ICourseRepository _course;
        private readonly IPQJQuestionRepository _pqj;
        private readonly IMemoryCache _cache; 
        private readonly IStudentRepository _student;
        private readonly ProtectorService _protect;
        public StudentController(ICourseRepository course, IMemoryCache cache, ProtectorService protect, INotificationService notify,IStudentRepository student, IPQJQuestionRepository pqj) :
            base(notify)
        {
            _course = course;
            _protect = protect;
            _cache = cache;
            _student = student;
            _pqj = pqj;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Dashboard()
        {
            int userId = UserHelper.GetUserId(User); 
            var data = await _student.GetDashboard(userId); 
            return View(data); 
        }
   
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyCourses()
        {
            int studentId = UserHelper.GetUserId(User); 
            var courses = await _student.GetStudentCourses(studentId); 
            return View(courses);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Subscription()
        {
            return View();
        }

        [Authorize(Roles = "Student")] 
        [HttpGet]
        public async Task<IActionResult> Profile()
        { 
            int studentId = UserHelper.GetUserId(User); 
            var profile = await _student.GetStudentProfileAsync(studentId);
            if (profile == null) return NotFound();

            return View(profile);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CourseDetails(string id)
        {
            try
            {
                int did = _protect.Decrypt(id);
                var course = await _course.GetCourseDetailsById(did);

                if (course == null)
                    return NotFound();

                return View(course);
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }


        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Watch(string I, int? contentId)
        {
            int studentId = UserHelper.GetUserId(User);

            int courseId = _protect.Decrypt(I);
            bool hasAccess = await _student.CheckCourseAccessAsync(studentId, courseId);
            if (!hasAccess)
            {
                _notify.Error("Access Denied. This course is not included in your active subscription plan.");
                return RedirectToAction("MyCourses");
            }

            CourseDetailsVM courseDetails = await _student.GetCourseDetailsById(courseId);
            
            if (courseDetails == null || courseDetails.Sections == null || !courseDetails.Sections.Any())
            {
                _notify.Error("This course doesn't have any content yet.");
                return RedirectToAction("MyCourses");
            }
            ViewBag.CourseDetails = courseDetails;

            VideoVM activeVideo = null;

            if (contentId.HasValue)
            {
                activeVideo = courseDetails.Sections.SelectMany(s => s.Videos).FirstOrDefault(v => v.VideoId == contentId.Value);
            }
            else
            {
                activeVideo = courseDetails.Sections.OrderBy(s => s.SectionId).FirstOrDefault()?.Videos.OrderBy(v => v.VideoId).FirstOrDefault();
            }

            return View(activeVideo);
        }
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(StudentProfileViewModel model)
        {
            int studentId = UserHelper.GetUserId(User);
            model.StudentId = studentId;   
            if (!ModelState.IsValid)
            {
                return View(model);
            }
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
                var existingData = await _student.GetByIdAsync((int)studentId);
                model.ProfileImage = existingData?.ProfileImage;
            }
            bool isUpdated = await _student.UpdateStudentProfileAsync(model);
            if (isUpdated)
            {
                _notify.Success("Profile updated successfully!");
                return RedirectToAction("Profile"); 
            } 
            ModelState.AddModelError("", "Something went wrong while updating profile.");
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> PQJ()
        {
            var courses = await _student.GetCourse();
            var model = new PQJPlayerVM
            {
                Courses = courses
            };
            return View(model); 
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> LoadQuestion(int index, int courseId, int topicId, int difficulty)
        {
            string cacheKey = $"PQJ_{courseId}_{topicId}_{difficulty}";
            if (!_cache.TryGetValue(cacheKey, out List<QuestionVM> questions))
            {
                questions =
                    await _pqj.GetFilteredQuestions(courseId, topicId, difficulty, "Student");

                if (!questions.Any())
                {
                    return Content(
                        "<div class='alert alert-warning'>No questions found</div>");
                }

                _cache.Set(cacheKey, questions, TimeSpan.FromMinutes(30));
            }
            int attemptId = await _pqj.GetOrCreateAttempt(courseId);
            
            if (index <= 0 ||
                index > questions.Count)
            {
                await _pqj.CompleteAttempt(attemptId);
                return Content(@"
                         <div class='text-center p-5'>
                             <h3>No More Questions</h3>
                             <p>You have reached the end of this PQJ set.</p>
                       
                             <button class='btn btn-plan mt-3'
                                     onclick='location.reload()'>
                                 Restart
                             </button>
                         </div>");
            }

            var question =
                questions[index - 1]; 

            var model = new PQJPlayerVM
            {
                AttemptId = attemptId,
                Question = question,
                CurrentIndex = index,
                TotalQuestions = questions.Count
            };

            return PartialView(
                "_PQJPlayer",
                model);
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
