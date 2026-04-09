using Dapper;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnection _db;

        public PaymentRepository(IDbConnection db)
        {
            _db = db;
        }
        public async Task<bool> AddAsync(PaymentDto entity)
        {
            var result = await _db.ExecuteScalarAsync<int>(
                "sp_Payment",
                new
                {
                    Action = "CREATE",
                    entity.StudentId,
                    entity.PlanName,
                    entity.SubscriptionId,
                    entity.Amount,
                    entity.PaymentMethod,
                    TransactionId = Guid.NewGuid().ToString()
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> UpdateAsync(PaymentDto entity)
        {
            var result = await _db.ExecuteAsync(
                "sp_Payment",
                new
                {
                    Action = "UPDATE_STATUS", // mostly status hi update hota hai
                    PaymentId = entity.PaymentId,
                    PaymentStatus = entity.PaymentStatus,
                    TransactionId = entity.TransactionId
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> DeleteAsync(PaymentDto entity)
        {
            // Agar delete nahi hai SP me, to ignore ya soft delete implement karo
            var result = await _db.ExecuteAsync(
                "sp_Payment",
                new
                {
                    Action = "DELETE",
                    PaymentId = entity.PaymentId
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _db.QueryAsync<Payment>(
                "sp_Payment",
                new { Action = "GET_ALL" },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Payment> GetByIdAsync(int paymentId)
        {
            return await _db.QueryFirstOrDefaultAsync<Payment>(
                "sp_Payment",
                new
                {
                    Action = "GET_BY_ID",
                    PaymentId = paymentId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> UpdateStatusAsync(int paymentId, string status, string transactionId)
        {
            return await _db.ExecuteAsync(
                "sp_Payment",
                new
                {
                    Action = "UPDATE_STATUS",
                    PaymentId = paymentId,
                    PaymentStatus = status,
                    TransactionId = transactionId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        Task<List<PaymentDto>> ICommonRepository<PaymentDto>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<PaymentDto?> ICommonRepository<PaymentDto>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
