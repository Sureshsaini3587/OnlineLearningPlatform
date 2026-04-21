using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
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

        public async Task<bool> AddAsync(CourseSectionDTO entity)
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
            return result > 0; 
        }
        public async Task<bool> UpdateAsync(CourseSectionDTO entity)
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

                return result > 0;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeleteAsync(CourseSectionDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_CourseSection",
                new { Action = "DELETE", entity.SectionId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        } 
    }
}