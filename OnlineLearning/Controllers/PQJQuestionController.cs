using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Helpers;
using OnlineLearning.Helpers.enums;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PQJQuestionController : BaseController
    {
        private readonly IPQJQuestionRepository _service;  

        public PQJQuestionController(INotificationService notify,IPQJQuestionRepository service):
            base(notify)
        {
            _service = service; 
        }
         
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        } 
        public async Task<IActionResult> Details(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return View(data);
        } 
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            var vm = new QuestionVM
            {
                Question = new PQJQuestion()  
            };
            return PartialView("_QuestionForm", vm);
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
            }
            try
            {  
                if (vm.Question.QuestionType == QuestionType.MCQ || vm.Question.QuestionType == QuestionType.TrueFalse)
                {
                    if (vm.Options == null || vm.Options.Count < 2)
                    {
                        return Json(new { success = false, message = "Minimum 2 options required !" }); 
                    }
                }
                int userId = UserHelper.GetUserId(User);
                vm.Question.CreatedBy = userId;   
               var result = await _service.CreateAsync(vm.Question, vm.Options, vm.CorrectOption);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }
         
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null) return NotFound();
            await LoadDropdowns();
            var vm = new QuestionVM
            {
                Question = data,
                Options = data.Options ?? new List<PQJOption>(),
                CorrectOption = data.Options?.FindIndex(x => x.IsCorrect) ?? -1
            };
            return PartialView("_QuestionForm", vm); 
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Fill the data correctly !" });
            }
            try
            {
                int userId = UserHelper.GetUserId(User);
                vm.Question.UpdatedBy = userId;

               var result =  await _service.UpdateAsync(vm.Question, vm.Options, vm.CorrectOption);

                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int QuestionId)
        {
            try
            {
                int userId = UserHelper.GetUserId(User);
             var result = await _service.DeleteAsync(QuestionId, userId);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
        private async Task LoadDropdowns()
        {
            var Course = await _service.GetCourse();
            ViewBag.Course = Course
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();  
        }
    }
}
