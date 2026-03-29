using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseCategoryRepository : ICourseCategoryRepository
    {

        private readonly IConfiguration _config;

        public CourseCategoryRepository(IConfiguration config)
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
         
        public async Task<List<CourseCategoriesDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<CourseCategoriesDTO>(
                  "sp_CourseCategory",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }
        public async Task<List<CourseCategoriesDTO>> GetAllWithDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<CourseCategoriesDTO>(
                "sp_CourseCategory",                
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure                
            );

            return data.ToList();
        }

        public async Task<CourseCategoriesDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<CourseCategoriesDTO>(
                "sp_CourseCategory",
                new { Action = "GET_BY_ID", CategoryId = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<bool> AddAsync(CourseCategoriesDTO entity)
        {
            using var db = Connection;
            var result = await db.ExecuteAsync(
                 "sp_CourseCategory",
                 new
                 {
                     Action = "INSERT",
                     entity.CategoryName,
                     entity.ParentCategoryId, 
                     entity.IsActive,
                     entity.CreatedBy
                 },
                 commandType: CommandType.StoredProcedure
             ); 
            return result > 0; 
        }
        public async Task<bool> UpdateAsync(CourseCategoriesDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_CourseCategory",
                new
                {
                    Action = "UPDATE",
                    entity.CategoryId,
                    entity.CategoryName,
                    entity.ParentCategoryId,  
                    entity.IsActive,
                    entity.UpdatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
        public async Task<bool> DeleteAsync(CourseCategoriesDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_CourseCategory",
                new { Action = "DELETE", entity.CategoryId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        } 
    }
}