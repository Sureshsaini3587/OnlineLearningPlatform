using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IPaymentRepository:ICommonRepository<PaymentDto>
    {
        Task<bool> HasActiveSubscription(int studentId, int courseId);
        Task<int> UpdateStatusAsync(int paymentId, string status, string transactionId);
        Task<object> CreateOrder(int userId, CoursePaymentDTO dto);
        Task<PaymentResponseDTO> StartPayment(int userId, int planId, int courseId);
        Task<int> CompletePayment(int userId, CoursePaymentDTO dto);
        Task<bool> HasCourseAccess(int userId, int courseId);

        Task<bool> VerifyPayment(string orderId,string paymentId,string signature);
          
    }
}
