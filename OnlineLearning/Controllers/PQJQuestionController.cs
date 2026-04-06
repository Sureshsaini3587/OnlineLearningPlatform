using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.IRepository;
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
        public IActionResult Create()
        {
            return View(new QuestionVM());
        }
         
        [HttpPost]
        public async Task<IActionResult> Create(QuestionVM vm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(vm);
                 
                if (vm.Question.QuestionType == QuestionType.MCQ)
                {
                    if (vm.Options == null || vm.Options.Count < 2)
                    {
                        ModelState.AddModelError("", "Minimum 2 options required");
                        return View(vm);
                    }
                }

                vm.Question.CreatedBy = 1;  

                await _service.CreateAsync(vm.Question, vm.Options, vm.CorrectOption);

               _notify.Success("Question created successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
         
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null) return NotFound();

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
                    return View(vm);

                vm.Question.UpdatedBy = 1;

                await _service.UpdateAsync(vm.Question, vm.Options, vm.CorrectOption);

                _notify.Success("Question updated successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
          
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id, 1);
            _notify.Success("Deleted successfully");
            return RedirectToAction("Index");
        }
    }
}
