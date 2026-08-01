using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Views.Shared.Components.Model
{ 
    public class ModelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
