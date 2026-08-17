using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseVideoRepository : ICourseVideoRepository
    {

        private readonly IConfiguration _config;

        public CourseVideoRepository(IConfiguration config)
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
         
        public async Task<List<CourseVideoDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<CourseVideoDTO>(
                  "sp_CourseVideo",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }  
        public async Task<List<CourseVideoDTO>> GetAllDemosDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<CourseVideoDTO>(
                "sp_CourseVideo",               
                new { Action = "GET_ALL_Demo" },
                commandType: CommandType.StoredProcedure
            );

            return data.ToList();
        }
        public async Task<List<CourseVideoDTO>> GetAllWithDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<CourseVideoDTO>(
                "sp_CourseVideo",               
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure
            );

            return data.ToList();
        }

        public async Task<CourseVideoDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<CourseVideoDTO>(
                "sp_CourseVideo",
                new { Action = "GET_BY_ID", VideoId = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<Result> AddAsync(CourseVideoDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                     "sp_CourseVideo",
                     new
                     {
                         Action = "INSERT",
                         entity.Title,
                         entity.SectionId,
                         entity.SortOrder,
                         entity.Duration,
                         entity.IsDemo,
                         entity.VideoUrl,
                         entity.ThumbnailUrl,
                         entity.IsActive,
                         entity.CreatedBy
                     },
                     commandType: CommandType.StoredProcedure
                 );

                return result > 0
                    ? new Result { Success = true, Message = "Video added successfully." }
                    : new Result { Success = false, Message = "Failed to add video." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(CourseVideoDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_CourseVideo",
                    new
                    {
                        Action = "UPDATE",
                        entity.VideoId,
                        entity.Title,
                        entity.SectionId,
                        entity.SortOrder,
                        entity.Duration,
                        entity.IsDemo,
                        entity.VideoUrl,
                        entity.ThumbnailUrl,
                        entity.IsActive,
                        entity.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Video updated successfully." }
                    : new Result { Success = false, Message = "Video update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(CourseVideoDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_CourseVideo",
                    new { Action = "DELETE", entity.VideoId },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Video deleted successfully." }
                    : new Result { Success = false, Message = "Video could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
    }
}