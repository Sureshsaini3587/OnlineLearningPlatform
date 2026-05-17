using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    [Authorize(Roles = "Student")]
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
            int userId = UserHelper.GetUserId(User);  

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
            var result = await _paymentService.CreateOrder(userId,dto);

            return Json(result);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Success([FromBody] CoursePaymentDTO dto)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please login"
                    });
                } 
                int userId = UserHelper.GetUserId(User); 

                bool alreadySubscribed = await _paymentService.HasActiveSubscription(  userId,  dto.CourseId);

                if (alreadySubscribed)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Course already unlocked"
                    });
                }
                  
                bool verified =
                    await _paymentService.VerifyPayment(
                        dto.RazorpayOrderId,
                        dto.RazorpayPaymentId,
                        dto.RazorpaySignature);

                if (!verified)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Payment verification failed"
                    });
                }
                 
                int subscriptionId = await _paymentService.CompletePayment( userId, dto);

                return Json(new
                {
                    success = true,
                    subscriptionId = subscriptionId
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        } 
    }
}
