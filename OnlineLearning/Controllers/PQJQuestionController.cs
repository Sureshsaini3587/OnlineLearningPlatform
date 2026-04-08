using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Helpers;
using OnlineLearning.Helpers.enums;
using OnlineLearning.Models;

namespace OnlineLearning.Controllers
{
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
            return View(new QuestionVM());
        }
         
        [HttpPost]
        public async Task<IActionResult> Create(QuestionVM vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdowns(); 
                    return View(vm);
                }
                 
                if (vm.Question.QuestionType == QuestionType.MCQ)
                {
                    if (vm.Options == null || vm.Options.Count < 2)
                    {
                        ModelState.AddModelError("", "Minimum 2 options required");
                        return View(vm);
                    }
                }
                int userId = UserHelper.GetUserId(User);
                vm.Question.CreatedBy = userId;   
                await _service.CreateAsync(vm.Question, vm.Options, vm.CorrectOption); 
               _notify.Success("Question created successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadDropdowns();
                return View(vm);
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

            return View(vm);
        }
         
        [HttpPost]
        public async Task<IActionResult> Edit(QuestionVM vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdowns();
                    return View(vm);
                }
                int userId = UserHelper.GetUserId(User);
                vm.Question.UpdatedBy = userId;

                await _service.UpdateAsync(vm.Question, vm.Options, vm.CorrectOption);

                _notify.Success("Question updated successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await LoadDropdowns();
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _service.GetByIdAsync(id); 
            var CourseDTO = new QuestionVM
            {
                Question = new PQJQuestion
                {
                    QuestionId = data.QuestionId,
                    QuestionText = data.QuestionText
                }
            };
            return View(CourseDTO); 
        }
          
        public async Task<IActionResult> DeleteConfirmed(int QuestionId)
        {
            int userId = UserHelper.GetUserId(User);
            await _service.DeleteAsync(QuestionId, userId);
            _notify.Success("Deleted successfully");
            return RedirectToAction("Index");
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
            var Category = await _service.GetCategory();
            ViewBag.Category = Category
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
             
        }
    }
}
