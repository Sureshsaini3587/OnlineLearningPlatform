using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;

namespace OnlineLearning.Controllers
{
    public class InstructorController : BaseController
    {
       
        private readonly IInstructorRepository _instructor;
        public InstructorController(INotificationService notify, IInstructorRepository instructor) :
            base(notify)
        {
            _instructor = instructor;
        }

        #region Instructor
        public async Task<IActionResult> Index()
        {
            var Instructors = await _instructor.GetAllWithDetails();
            return View(Instructors);
        }

        public async Task<IActionResult> Create()
        { 
            return View();
        }
         

        [HttpPost]
        public async Task<IActionResult> Create(Instructor model)
        {
            if (!ModelState.IsValid)
            { 
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
            var InstructorDTO = new InstructorDTO
            {
                FullName = model.FullName, 
                Email = model.Email,
                Bio = model.Bio,
                ExperienceYears = model.ExperienceYears,
                Mobile = model.Mobile,
                ProfileImage=model.ProfileImage,
                IsActive = model.IsActive,
                CreatedBy = userId
            };
            var result = await _instructor.AddAsync(InstructorDTO);

            if (result)
            {
                _notify.Success("Instructor saved successfully ! ");
                return RedirectToAction("Index");
            }
             
            _notify.Error("Some Error Occured while saving details!");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var Instructor = await _instructor.GetByIdAsync(id);
            if (Instructor == null) return NotFound();
             
            var InstructorDTO = new Instructor
            {
                InstructorId = Instructor.InstructorId, 
                FullName = Instructor.FullName, 
                Email = Instructor.Email,
                Bio = Instructor.Bio,
                ExperienceYears = Instructor.ExperienceYears,
                Mobile = Instructor.Mobile,
                ProfileImage = Instructor.ProfileImage,
                IsActive = Instructor.IsActive,
            };  
            return View(InstructorDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Instructor model)
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
                    var existingData = await _instructor.GetByIdAsync((int)model.InstructorId); 
                    model.ProfileImage = existingData?.ProfileImage;
                }

                var InstructorDTO = new InstructorDTO
                {
                    InstructorId = model.InstructorId, 
                    FullName = model.FullName,
                    Bio = model.Bio,
                    Email = model.Email,
                    ExperienceYears = model.ExperienceYears, 
                    Mobile = model.Mobile,
                    ProfileImage = model.ProfileImage,
                    IsActive = model.IsActive,
                    UpdatedBy = userId
                };
                var result = await _instructor.UpdateAsync(InstructorDTO);
                if (!result)
                { 
                    _notify.Error("An Error Occures While Updating Section Details !"); 
                    return View(model);
                }

                if (result)
                {
                    _notify.Success("Instructor Profile updated successfully!"); 
                    return RedirectToAction("Index");
                } 
            } 
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Instructor = await _instructor.GetByIdAsync(id);
            var InstructorDTO = new Instructor
            {
                FullName = Instructor.FullName,
                InstructorId = Instructor.InstructorId
            };
            return View(InstructorDTO);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int UserID)
        {
            int userId = UserHelper.GetUserId(User);
            var Instructor = await _instructor.GetByIdAsync(UserID);
            Instructor.IsDeleted = true;
            Instructor.UpdatedBy = userId;
            var result = await _instructor.DeleteAsync(Instructor);
            _notify.Success("Instructor Profile Delete successfully!");
            return RedirectToAction("Index");
        } 

        #endregion
    }
}
