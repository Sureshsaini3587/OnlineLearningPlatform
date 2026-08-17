using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {

        private readonly IConfiguration _config;

        public SubscriptionPlanRepository(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(
                    _config.GetConnectionString("DbConnection"));
            }
        }
         
        public async Task<List<SubscriptionPlanDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<SubscriptionPlanDTO>(
                  "sp_SubscriptionPlan",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }
        public async Task<List<SubscriptionPlanDTO>> GetAllWithDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<SubscriptionPlanDTO>(
                "sp_SubscriptionPlan",                
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure                
            );

            return data.ToList();
        }

        public async Task<SubscriptionPlanDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<SubscriptionPlanDTO>(
                "sp_SubscriptionPlan",
                new { Action = "GET_BY_ID", PlanId = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<Result> AddAsync(SubscriptionPlanDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                     "sp_SubscriptionPlan",
                     new
                     {
                         Action = "INSERT",
                         entity.PlanName,
                         entity.Price,
                         entity.DurationInDays,
                         entity.Description,
                         entity.IsActive,
                         entity.CreatedBy
                     },
                     commandType: CommandType.StoredProcedure
                 );

                return result > 0
                    ? new Result { Success = true, Message = "Subscription plan added successfully." }
                    : new Result { Success = false, Message = "Failed to add subscription plan." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(SubscriptionPlanDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_SubscriptionPlan",
                    new
                    {
                        Action = "UPDATE",
                        entity.PlanId,
                        entity.PlanName,
                        entity.Price,
                        entity.DurationInDays,
                        entity.Description,
                        entity.IsActive,
                        entity.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Subscription plan updated successfully." }
                    : new Result { Success = false, Message = "Plan update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(SubscriptionPlanDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_SubscriptionPlan",
                    new { Action = "DELETE", entity.PlanId },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Subscription plan deleted successfully." }
                    : new Result { Success = false, Message = "Subscription plan could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
    }
}