using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseSectionRepository : ICourseSectionRepository
    {

        private readonly IConfiguration _config;
         
        public CourseSectionRepository(IConfiguration config)
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
         
        public async Task<List<CourseSectionDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<CourseSectionDTO>(
                  "sp_CourseSection",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }  
         
        public async Task<List<CourseSectionDTO>> GetByCourseId(int courseId)
        {
            string cacheKey = $"COURSE_SECTIONS_{courseId}"; 
           
            using var db = Connection; 
            var data = await db.QueryAsync<CourseSectionDTO>(
                  "sp_CourseSection",
                  new { Action = "GET_ALL_BYCourse", CourseId = courseId },
                  commandType: CommandType.StoredProcedure
              );
            return data.ToList();    
             
        }  
        public async Task<List<CourseSectionDTO>> GetAllWithDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<CourseSectionDTO>(
                "sp_CourseSection",               
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure
            );

            return data.ToList();
        }

        public async Task<CourseSectionDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<CourseSectionDTO>(
                "sp_CourseSection",
                new { Action = "GET_BY_ID", SectionId = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<Result> AddAsync(CourseSectionDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                     "sp_CourseSection",
                     new
                     {
                         Action = "INSERT",
                         entity.CourseId,
                         entity.SectionTitle,
                         entity.SortOrder,
                         entity.IsActive,
                         entity.CreatedBy
                     },
                     commandType: CommandType.StoredProcedure
                 );

                if (result > 0)
                { 
                    string cacheKey = $"COURSE_SECTIONS_{entity.CourseId}"; 
                    return new Result { Success = true, Message = "Section added successfully." };
                }

                return new Result { Success = false, Message = "Failed to add section." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(CourseSectionDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_CourseSection",
                    new
                    {
                        Action = "UPDATE",
                        entity.CourseId,
                        entity.SectionTitle,
                        entity.SectionId,
                        entity.SortOrder,
                        entity.IsActive,
                        entity.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (result > 0)
                { 
                    string cacheKey = $"COURSE_SECTIONS_{entity.CourseId}"; 
                    return new Result { Success = true, Message = "Section updated successfully." };
                }

                return new Result { Success = false, Message = "Section update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(CourseSectionDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_CourseSection",
                    new { Action = "DELETE", entity.SectionId },
                    commandType: CommandType.StoredProcedure
                );

                if (result > 0)
                {  
                    string cacheKey = $"COURSE_SECTIONS_{entity.CourseId}"; 
                    return new Result { Success = true, Message = "Section deleted successfully." };
                }

                return new Result { Success = false, Message = "Section could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
    }
}