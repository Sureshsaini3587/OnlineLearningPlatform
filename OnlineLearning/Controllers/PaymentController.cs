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
    public class PaymentController : BaseController
    {
        private readonly IPaymentRepository _paymentService;
        public PaymentController(INotificationService notify, IPaymentRepository paymentService) :
            base(notify)
        {
            _paymentService = paymentService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CoursePaymentDTO dto)
        {
            int userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

            bool alreadySubscribed =
                await _paymentService.HasActiveSubscription(
                    userId,
                    dto.CourseId);

            if (alreadySubscribed)
            {
                _notify.Warning("Course already unlocked");
                return BadRequest(new
                {
                    message = "Course already unlocked"
                });
            }

            //var result = await _paymentService.CreateQRPayment(
            //    userId,
            //    dto);

            return Json("");
        }
        [HttpPost]
        public async Task<IActionResult> Success([FromBody] CoursePaymentDTO dto)
        {
            int userId = UserHelper.GetUserId(User);

            var subId = await _paymentService.CompletePayment(userId, dto.SubscriptionId);

            return Json(new { success = true, subId });
        } 
    }
}
