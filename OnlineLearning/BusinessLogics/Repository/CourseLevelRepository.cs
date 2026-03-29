using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseLevelRepository : ICourseLevelRepository
    {

        private readonly IConfiguration _config;

        public CourseLevelRepository(IConfiguration config)
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
         
        public async Task<List<CourseLevelDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<CourseLevelDTO>(
                  "sp_CourseLevel",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }
        public async Task<List<CourseLevelDTO>> GetAllWithDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<CourseLevelDTO>(
                "sp_CourseLevel",                
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure                
            );

            return data.ToList();
        }

        public async Task<CourseLevelDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<CourseLevelDTO>(
                "sp_CourseLevel",
                new { Action = "GET_BY_ID", CategoryId = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<bool> AddAsync(CourseLevelDTO entity)
        {
            using var db = Connection;
            var result = await db.ExecuteAsync(
                 "sp_CourseLevel",
                 new
                 {
                     Action = "INSERT",
                     entity.LevelName, 
                     entity.IsActive,
                     entity.CreatedBy
                 },
                 commandType: CommandType.StoredProcedure
             ); 
            return result > 0; 
        }
        public async Task<bool> UpdateAsync(CourseLevelDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_CourseLevel",
                new
                {
                    Action = "UPDATE",
                    entity.LevelName, 
                    entity.IsActive,
                    entity.UpdatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
        public async Task<bool> DeleteAsync(CourseLevelDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_CourseLevel",
                new { Action = "DELETE", entity.LevelId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        } 
    }
}