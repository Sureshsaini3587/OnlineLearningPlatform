 
using Microsoft.AspNetCore.Mvc; 
using OnlineLearning.BusinessLogics.IRepository;

namespace OnlineLearning.Controllers
{
    public class BaseController : Controller
    { 
        protected readonly INotificationService _notify;

        public BaseController(INotificationService notify)
        { 
            _notify = notify;
        }
    }
}
