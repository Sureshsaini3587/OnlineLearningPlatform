using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using static System.Collections.Specialized.BitVector32;

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

        public async Task<IActionResult> Create()
        { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubscriptionPlan model)
        {
            if (!ModelState.IsValid)
            { 
                return View(model);
            }
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

            if (result)
            {
                _notify.Success("Plan saved successfully!");
                return RedirectToAction("Index");
            }

            _notify.Error("Something went wrong!");   
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _plan.GetByIdAsync(id);
            if (course == null) return NotFound();
             
            var CourseDTO = new SubscriptionPlan
            {
                PlanId = course.PlanId,
                PlanName = course.PlanName,
                Price = course.Price,
                DurationInDays = course.DurationInDays,
                Description = course.Description,
                IsActive = course.IsActive
            };
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SubscriptionPlan model)
        {
            if (ModelState.IsValid)
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
                if (!result)
                {
                    _notify.Error("An Error Occures While Updating Plan Details !"); 
                    return View(model);
                }
                _notify.Success("Plan Details Update Successfully !");

                return RedirectToAction("Index");
            } 
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _plan.GetByIdAsync(id);
            var CourseDTO = new SubscriptionPlan
            {
                PlanName = course.PlanName,
                PlanId = course.PlanId
            };
            return View(CourseDTO);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int PlanId)
        {
            int userId = UserHelper.GetUserId(User);
            var course = await _plan.GetByIdAsync(PlanId);
            course.IsDelete = true;
            course.UpdatedBy = userId;
            var result = await _plan.DeleteAsync(course);
            _notify.Success("Plan Delete Successfully !");
            return RedirectToAction("Index");
        } 

        #endregion
    }
}
