using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PlanCourseController : BaseController
    {
        private readonly ICourseRepository _course;
        private readonly ISubscriptionPlanRepository _Plan;
        public PlanCourseController(ICourseRepository course, ISubscriptionPlanRepository Plan, INotificationService notify) :  base(notify){
          this._course = course;
          this._Plan = Plan;
        }
        public async Task<IActionResult> Index()
        {
            var data = await _course.GetAllCoursePlan();

            var grouped = data
                .GroupBy(x => new { x.PlanId, x.PlanName })
                .Select(g => new PlanCourseVM
                {
                    PlanId = g.Key.PlanId,
                    Plans = new List<SelectListItem>
                    {
                         new SelectListItem { Value = g.Key.PlanId.ToString(), Text = g.Key.PlanName }
                    },
                    SelectedCourseIds = g.Select(x => x.CourseId).ToList(),
                    Courses = g.Select(x => new SelectListItem
                    {
                        Value = x.CourseId.ToString(),
                        Text = x.CourseTitle
                    }).ToList()
                }).ToList();

            return View(grouped); 
        }
        public async Task<IActionResult> Create()
        {
            var Courses = await _course.GetAllAsync();
            var Plans = await _Plan.GetAllAsync();
            var vm = new PlanCourseVM
            {
                Courses = Courses.Select(x => new SelectListItem { Value = x.CourseId.ToString(), Text = x.CourseTitle }).ToList(),

                Plans =  Plans.Select(x=>new SelectListItem { Value=x.PlanId.ToString(),Text=x.PlanName}).ToList()
            };
            return PartialView("_MappingForm", vm); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanCourseVM vm)
        {
            if (vm.PlanId == 0 || vm.SelectedCourseIds == null || !vm.SelectedCourseIds.Any())
            { 
                return Json(new { success = false, message = "Please select plan and at least one course!" }); 
            }

            foreach (var courseId in vm.SelectedCourseIds)
            {
                await _course.AddCourseToPlanAsync(new PlanCourseDto
                {
                    PlanId = vm.PlanId,
                    CourseId = courseId
                });
            }
            return Json(new { success = true, message = "Courses assigned successfully" }); 
        }
        public async Task<IActionResult> Edit(int planId)
        {
            if (planId == 0)
            {
                return Json(new { success = false, message = "Invalid details !" });
            }
            var assignedCourses =  await _course.GetCoursesByPlanAsync(planId);
            var Courses = await _course.GetAllAsync();
            var Plans = await _Plan.GetAllAsync();
            var vm = new PlanCourseVM
            {
                PlanId = planId,
                SelectedCourseIds = assignedCourses.Select(x => x.CourseId).ToList(), 
                Courses = Courses.Select(x => new SelectListItem { Value = x.CourseId.ToString(), Text = x.CourseTitle }).ToList(),

                Plans = Plans.Select(x => new SelectListItem { Value = x.PlanId.ToString(), Text = x.PlanName }).ToList()
            };
            return PartialView("_MappingForm", vm); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PlanCourseVM vm)
        {
            if (vm.PlanId == 0 || vm.SelectedCourseIds == null || !vm.SelectedCourseIds.Any())
            {
                return Json(new { success = false, message = "Please select plan and at least one course!" });
            }  
            await _course.DeleteByPlanIdAsync(vm.PlanId);
             
            foreach (var courseId in vm.SelectedCourseIds)
            {
                await _course.AddCourseToPlanAsync(new PlanCourseDto
                {
                    PlanId = vm.PlanId,
                    CourseId = courseId
                });
            }
            return Json(new { success = true, message = "Update Successfully" }); 
        }
    }
}
