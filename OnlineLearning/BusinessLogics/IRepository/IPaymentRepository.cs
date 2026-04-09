using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IPaymentRepository:ICommonRepository<PaymentDto>
    {
        Task<int> UpdateStatusAsync(int paymentId, string status, string transactionId); 
        Task<Payment> GetByIdAsync(int paymentId); 
        Task<IEnumerable<Payment>> GetAllAsync();
    }
}
