using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using Razorpay.Api;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public PaymentRepository(IConfiguration config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
        }
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DbConnection"));
            }
        }
        public async Task<bool> HasActiveSubscription(int studentId,int courseId)
        {
            using var db = Connection;
            return await db.ExecuteScalarAsync<bool>(
                @"SELECT COUNT(1)
                    FROM StudentSubscriptions
                    WHERE StudentId=@StudentId 
                    AND IsActive=1
                    AND EndDate > GETDATE()",
                new
                {
                    StudentId = studentId,
                    CourseId = courseId
                });
        }

        public async Task<object> CreateOrder(int userId,CoursePaymentDTO dto)
        {
            try
            {
                using var db = Connection;
                  
                var plan =
                    await db.QueryFirstOrDefaultAsync<PlanCourseListDto>(
                        @"SELECT 
                        SP.Price AS Price
                             FROM SubscriptionPlans SP
                             INNER JOIN PlanCourses PC
                                   ON SP.PlanId = PC.PlanId
                             WHERE PC.PlanCourseId = @PlanId",
                        new
                        {
                            PlanId = dto.PlanId
                        });

                if (plan == null)
                {
                    throw new Exception(
                        "Plan not found");
                } 

                if (plan.Price <= 0)
                {
                    throw new Exception(
                        "Invalid plan price");
                } 
                var client =
                    new RazorpayClient(
                        _config["Razorpay:Key"],
                        _config["Razorpay:Secret"]);
                 

                Dictionary<string, object> options =
                    new();

                options.Add(
                    "amount",
                    Convert.ToInt32(plan.Price * 100));

                options.Add(
                    "currency",
                    "INR");

                options.Add(
                    "receipt",
                    Guid.NewGuid().ToString());

                options.Add(
                    "payment_capture",
                    1);
                 

                Order order =
                    client.Order.Create(options);

                if (order == null)
                {
                    throw new Exception(
                        "Unable to create payment order");
                }
                 

                await db.ExecuteAsync(
                    "sp_Payment",
                    new
                    {
                        Action = "CREATE",
                        StudentId = userId,
                        CourseId = dto.CourseId,
                        Amount = plan.Price,
                        PaymentGateway = "Razorpay",
                        TransactionId = order["id"].ToString()
                    },
                    commandType: CommandType.StoredProcedure);
                  
                return new
                {
                    success = true,

                    key = _config["Razorpay:Key"],

                    amount = order["amount"],

                    orderId = order["id"].ToString(),

                    courseId = dto.CourseId,

                    planId = dto.PlanId
                };
            }
            catch (Razorpay.Api.Errors.BadRequestError ex)
            {
                throw new Exception(
                    $"Razorpay bad request: {ex.Message}");
            }
            catch (Razorpay.Api.Errors.ServerError ex)
            {
                throw new Exception(
                    $"Razorpay server error: {ex.Message}");
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    ex.Message);
            }
        }
        public async Task<bool> VerifyPayment(string orderId,string paymentId,string signature)
        {
            try
            {
                string secret =  _config["Razorpay:Secret"];

                string payload =  $"{orderId}|{paymentId}";

                using var hmac =  new HMACSHA256(  Encoding.UTF8.GetBytes(secret));

                var hash =   hmac.ComputeHash( Encoding.UTF8.GetBytes(payload));

                string generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return generatedSignature == signature;
            }
            catch
            {
                return false;
            }
        }
        public async Task<int> CompletePayment( int userId, CoursePaymentDTO dto)
        {
            using var db = Connection;

            db.Open();
            using var tran =  db.BeginTransaction();

            try
            {
                var plan = await db.QueryFirstAsync<dynamic>(
                   @"SELECT 
                        SP.DurationDays AS DurationDays
                             FROM SubscriptionPlans SP
                             INNER JOIN PlanCourses PC
                                   ON SP.PlanId = PC.PlanId
                             WHERE PC.PlanCourseId =@PlanId",
                   new { PlanId = dto.PlanId }, tran); 

                if (plan == null)
                    throw new Exception("Plan not found");
                 

                await db.ExecuteAsync(
                    @"  UPDATE Payments  SET  Status = 'Success', TransactionId = @PaymentId,  UpdatedOn = GETDATE()   WHERE TransactionId = @OrderId  ",
                    new
                    {
                        PaymentId =
                            dto.RazorpayPaymentId,

                        OrderId =
                            dto.RazorpayOrderId
                    },  tran);
                 
                int subscriptionId =
                    await db.ExecuteScalarAsync<int>(
                    @"
                         INSERT INTO StudentSubscriptions
                         (
                             StudentId,
                             PlanId,
                             StartDate,
                             EndDate,
                             Status,
                             IsActive,
                             CreatedOn
                         )
                         VALUES
                         (
                             @StudentId,
                             @PlanId,
                             GETDATE(),
                             DATEADD(
                                 DAY,
                                 @Days,
                                 GETDATE()
                             ),
                             'Active',
                             1,
                             GETDATE()
                         )
                       
                         SELECT CAST(SCOPE_IDENTITY() AS INT)
                         ",
                    new
                    {
                        StudentId = userId,
                        PlanId = dto.PlanId,
                        Days = plan.DurationDays
                    },
                    tran);

                tran.Commit();

                return subscriptionId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

          
        public async Task<bool> HasCourseAccess(int userId, int courseId)
        {
            using var _db = Connection;
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
        public async Task<Result> AddAsync(PaymentDto entity)
        {
            try
            {
                using var _db = Connection; 
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

                return result > 0
                    ? new Result { Success = true, Message = "Payment recorded successfully." }
                    : new Result { Success = false, Message = "Failed to record payment." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(PaymentDto entity)
        {
            try
            {
                using var _db = Connection;
                var result = await _db.ExecuteAsync(
                    "sp_Payment",
                    new
                    {
                        Action = "UPDATE_STATUS",
                        entity.PaymentId,
                        entity.PaymentStatus,
                        entity.TransactionId
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Payment status updated successfully." }
                    : new Result { Success = false, Message = "Payment update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(PaymentDto entity)
        {
            try
            {
                using var _db = Connection;
                var result = await _db.ExecuteAsync(
                    "sp_Payment",
                    new
                    {
                        Action = "DELETE",
                        entity.PaymentId
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Payment record deleted successfully." }
                    : new Result { Success = false, Message = "Payment record could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<int> UpdateStatusAsync(int paymentId, string status, string transactionId)
        {
            using var _db = Connection;
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
