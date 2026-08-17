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
    public class SubscriptionController : BaseController
    {
        private readonly ISubscriptionPlanRepository _plan; 
        public SubscriptionController(INotificationService notify,ISubscriptionPlanRepository plan):
            base(notify)
        {
            _plan = plan; 
        }

        #region Subscription Plan
        public async Task<IActionResult> Index()
        {
            var courses = await _plan.GetAllWithDetails();
            return View(courses);
        }
         
        [HttpGet]
        public IActionResult Create() => PartialView("_SubscriptionForm", new SubscriptionPlan());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriptionPlan model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
            }
            try
            {

                int userId = UserHelper.GetUserId(User);
                var CourseDTO = new SubscriptionPlanDTO
                {
                    PlanName = model.PlanName,
                    DurationInDays = model.DurationInDays,
                    Price = model.Price,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CreatedBy = userId
                };
                var result = await _plan.AddAsync(CourseDTO);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error updating SubscriptionPlan ID {Id}", model.PlanName); 
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _plan.GetByIdAsync(id);
            if (course == null) return NotFound();
             
            var SubscriptionPlan = new SubscriptionPlan
            {
                PlanId = course.PlanId,
                PlanName = course.PlanName,
                Price = course.Price,
                DurationInDays = course.DurationInDays,
                Description = course.Description,
                IsActive = course.IsActive
            };
            return PartialView("_SubscriptionForm", SubscriptionPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriptionPlan model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill the data correctly!" });
            }
            try
            {
                int userId = UserHelper.GetUserId(User);
                var CourseDTO = new SubscriptionPlanDTO
                {
                    PlanId = model.PlanId,
                    PlanName = model.PlanName,
                    DurationInDays = model.DurationInDays,
                    Price = model.Price,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    UpdatedBy = userId
                };
                var result = await _plan.UpdateAsync(CourseDTO);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            { 
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }

        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PlanId)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
                var course = await _plan.GetByIdAsync(PlanId);
                course.IsDelete = true;
                course.UpdatedBy = userId;
                var result = await _plan.DeleteAsync(course);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            { 
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        } 

        #endregion
    }
}
