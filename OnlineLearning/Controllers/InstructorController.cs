using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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

        [HttpGet]
        public IActionResult Create() => PartialView("_InstructorForm", new Instructor());
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Instructor model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
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
                return Json(new { success = true, message = "Instructor saved successfully !" }); 
            }
            return Json(new { success = false, message = "Some Error Occured while saving details!" }); 
        }

        [HttpGet]
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
            return PartialView("_InstructorForm", InstructorDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Instructor model)
        { 
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill the data correctly!" });
            }

            try
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
                    return Json(new { success = false, message = "An Error Occured While Updating Instructor Details!" });
                }

                return Json(new { success = true, message = "Instructor Profile updated successfully!" });
            }
            catch (Exception ex)
            { 
                // _logger.LogError(ex, "Error updating instructor ID {Id}", model.InstructorId); 
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                 
                var instructor = await _instructor.GetByIdAsync(id);
                 
                if (instructor == null)
                {
                    return Json(new { success = false, message = "Instructor not found!" });
                }
                 
                instructor.IsDeleted = true;
                instructor.UpdatedBy = userId;
                 
                var result = await _instructor.DeleteAsync(instructor);

                if (result)
                {
                    return Json(new { success = true, message = "Instructor deleted successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Unable to delete instructor. Please try again." });
                }
            }
            catch (Exception ex)
            { 
                // _logger.LogError(ex, "Error deleting instructor ID {Id}", id); 
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        #endregion
    }
}
