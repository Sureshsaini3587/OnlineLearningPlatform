using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
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

        public async Task<Result> AddAsync(CourseLevelDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.QueryFirstOrDefaultAsync<int>(
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

                if (result == -1) return new Result { Success = false, Message = "Course level name already exists." };
                if (result > 0) return new Result { Success = true, Message = "Course level added successfully." };

                return new Result { Success = false, Message = "Failed to add course level." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(CourseLevelDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_CourseLevel",
                    new
                    {
                        Action = "UPDATE",
                        entity.LevelId,  
                        entity.LevelName,
                        entity.IsActive,
                        entity.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Course level updated successfully." }
                    : new Result { Success = false, Message = "Course level update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(CourseLevelDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_CourseLevel",
                    new { Action = "DELETE", entity.LevelId },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Course level deleted successfully." }
                    : new Result { Success = false, Message = "Course level could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
    }
}