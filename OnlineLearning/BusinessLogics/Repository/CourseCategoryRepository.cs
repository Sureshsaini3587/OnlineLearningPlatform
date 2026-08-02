using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.ClientModel.Primitives;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseCategoryRepository : ICourseCategoryRepository
    {

        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public CourseCategoryRepository(IConfiguration config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
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
            string cacheKey = "CategoryWithDetails";
            if (_cache.TryGetValue(cacheKey, out List<CourseCategoriesDTO> courses))
            {
                return courses;
            }
            using var db = Connection; 
            var data = await db.QueryAsync<CourseCategoriesDTO>(
                "sp_CourseCategory",                
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure                
            );
            courses = data.ToList();
            _cache.Set(cacheKey, courses,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(30),
                    SlidingExpiration =
                        TimeSpan.FromMinutes(10),
                    Priority =
                        CacheItemPriority.High
                });
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
        public async Task<Result> AddAsync(CourseCategoriesDTO entity)
        {
            try
            {
                using var db = Connection; 
                var result = await db.QueryFirstOrDefaultAsync<int>(
                     "sp_CourseCategory",
                     new
                     {
                         Action = "INSERT",
                         entity.CategoryName,
                         entity.ParentCategoryId,
                         entity.IsActive,
                         entity.IsParent,
                         entity.CreatedBy
                     },
                     commandType: CommandType.StoredProcedure
                 );

                if (result == -1) return new Result { Success = false, Message = "Category name already exists." };
                if (result > 0) return new Result { Success = true, Message = "Category added successfully." };

                return new Result { Success = false, Message = "Failed to add category." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
        public async Task<Result> UpdateAsync(CourseCategoriesDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.QueryFirstOrDefaultAsync<int>(
                    "sp_CourseCategory",
                    new
                    {
                        Action = "UPDATE",
                        entity.CategoryId,
                        entity.CategoryName,
                        entity.ParentCategoryId,
                        entity.IsParent,
                        entity.IsActive,
                        entity.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Category updated successfully." }
                    : new Result { Success = false, Message = "Category update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(CourseCategoriesDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_CourseCategory",
                    new { Action = "DELETE", entity.CategoryId },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Category deleted successfully." }
                    : new Result { Success = false, Message = "Category could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
    }
}