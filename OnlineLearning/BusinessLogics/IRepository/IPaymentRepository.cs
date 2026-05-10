using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IPaymentRepository:ICommonRepository<PaymentDto>
    {
        Task<bool> HasActiveSubscription(int studentId, int courseId);
        Task<int> UpdateStatusAsync(int paymentId, string status, string transactionId); 
        Task<Payment> GetByIdAsync(int paymentId); 
        Task<IEnumerable<Payment>> GetAllAsync(); 
        Task<PaymentResponseDTO> StartPayment(int userId, int planId, int courseId);
        Task<int> CompletePayment(int userId, int planId);
        Task<bool> HasCourseAccess(int userId, int courseId);
    }
}
