using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
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

        public async Task<bool> AddAsync(SubscriptionPlanDTO entity)
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
            return result > 0; 
        }
        public async Task<bool> UpdateAsync(SubscriptionPlanDTO entity)
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

            return result > 0;
        }
        public async Task<bool> DeleteAsync(SubscriptionPlanDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_SubscriptionPlan",
                new { Action = "DELETE", entity.PlanId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        } 
    }
}