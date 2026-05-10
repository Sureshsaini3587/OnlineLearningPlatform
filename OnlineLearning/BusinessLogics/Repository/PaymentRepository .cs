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
        public async Task<bool> HasActiveSubscription(int studentId,int courseId)
        { 

            return await _db.ExecuteScalarAsync<bool>(
                @"SELECT COUNT(1)
                    FROM StudentSubscriptions
                    WHERE StudentId=@StudentId
                    AND CourseId=@CourseId
                    AND IsActive=1
                    AND ExpiryDate > GETDATE()",
                new
                {
                    StudentId = studentId,
                    CourseId = courseId
                });
        }
        public async Task<dynamic> CreateQRPayment(int userId, CoursePaymentDTO dto)
        { 

            var plan = await _db.QueryFirstAsync<PlanCourseListDto>(
                @"SELECT * FROM SubscriptionPlans  WHERE PlanId=@PlanId",
                new { dto.PlanId });

            string transactionId = $"TXN{Guid.NewGuid():N}";
             
            await _db.ExecuteAsync(
                @"INSERT INTO Payments
                   (
                       StudentId, SubscriptionId,  Amount,  PaymentGateway, TransactionId,  Status,  PaymentDate, IsActive, IsDeleted, CreatedBy, CreatedOn
                   )
                   VALUES
                   (
                       @StudentId,  @SubscriptionId, @Amount, @PaymentGateway, @TransactionId,'PENDING', GETDATE(), 1, 0, @CreatedBy,GETDATE()                      
                   )",
                new
                {
                    StudentId = userId,  SubscriptionId = dto.PlanId, Amount = plan.Price, PaymentGateway = "UPI",  TransactionId = transactionId,
                    Status = "PENDING",   CreatedBy = userId
                });

            string upiLink =
                $"upi://pay?pa=test@upi" +
                $"&pn=OnlineLearning" +
                $"&am={plan.Price}" +
                $"&cu=INR" +
                $"&tn={transactionId}";

            string qrCode =
                $"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={Uri.EscapeDataString(upiLink)}";

            return new
            {
                amount = plan.Price,
                qrCode,
                transactionId
            };
        }
        public async Task<bool> VerifyPayment(   string transactionId)
        { 
            var payment =
                await _db.QueryFirstOrDefaultAsync(
                @"SELECT *    FROM Payments  WHERE TransactionId=@TransactionId",
                new
                {
                    TransactionId = transactionId
                });

            if (payment == null)
                return false; 

            return true;
        }


        public async Task<int> CompletePayment(int userId, int planId)
        {
            using var tran = _db.BeginTransaction();

            try
            { 
                var plan = await _db.QueryFirstAsync<dynamic>(
                    @"SELECT DurationDays FROM Plans WHERE PlanId=@PlanId",
                    new { PlanId = planId }, tran);

                // Insert subscription
                var subId = await _db.ExecuteScalarAsync<int>(
                    @"INSERT INTO StudentSubscriptions
                  (StudentId, PlanId, StartDate, ExpiryDate, IsActive)
                  VALUES (@UserId, @PlanId, GETDATE(),
                          DATEADD(DAY, @Days, GETDATE()), 1);
                  SELECT SCOPE_IDENTITY();",
                    new
                    {
                        UserId = userId,
                        PlanId = planId,
                        Days = plan.DurationDays
                    }, tran);

                tran.Commit();

                return subId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // STEP 3: Course Access Check
        public async Task<bool> HasCourseAccess(int userId, int courseId)
        {
            var count = await _db.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
              FROM StudentSubscriptions ss
              JOIN PlanCourses pc ON ss.PlanId = pc.PlanId
              WHERE ss.StudentId=@UserId
              AND pc.CourseId=@CourseId
              AND ss.IsActive=1
              AND ss.ExpiryDate > GETDATE()",
                new { UserId = userId, CourseId = courseId });

            return count > 0;
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

        public Task<PaymentResponseDTO> StartPayment(int userId, int planId, int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
