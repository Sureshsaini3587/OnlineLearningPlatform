using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private const string CacheKey = "CourseWithDetails"; 
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public CourseRepository(IConfiguration config, IMemoryCache cache)
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
         
        public async Task<List<CourseDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<CourseDTO>(
                  "sp_Course",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }  
         
        public async Task<List<CommanDTO>> GetLanguage()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<CommanDTO>(
                  "sp_Course",
                  new { Action = "GET_Lang" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }  
         
        public async Task<List<CommanDTO>> GetInstructor()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<CommanDTO>(
                  "sp_Course",
                  new { Action = "GET_Inst" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }  
        public async Task<List<CourseDTO>> GetAllWithDetails()
        {
            string cacheKey = "CourseWithDetails";
            if (_cache.TryGetValue( cacheKey, out List<CourseDTO> courses))
            {
                return courses;
            }
            using var db = Connection;

            var data = await db.QueryAsync<CourseDTO>(
                "sp_Course",               
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure
            );

            courses = data.ToList();
            _cache.Set(cacheKey,courses,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(30), 
                    SlidingExpiration =
                        TimeSpan.FromMinutes(10), 
                    Priority =
                        CacheItemPriority.High
                });

            return courses;
        }

        public async Task<CourseDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<CourseDTO>(
                "sp_Course",
                new { Action = "GET_BY_ID", CourseId = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<CourseDetailsVM> GetCourseDetailsById(int id)
        {
            using var db = Connection;

            var courseDict = new Dictionary<int, CourseDetailsVM>();

            var result = await db.QueryAsync<CourseDetailsVM, SectionVM, VideoVM, CourseDetailsVM>(
                "sp_Course",
                (course, section, video) =>
                {
                    if (!courseDict.TryGetValue(course.CourseId, out var courseEntry))
                    {
                        courseEntry = course;
                        courseEntry.Sections = new List<SectionVM>();
                        courseDict.Add(courseEntry.CourseId, courseEntry);
                    }

                    if (section != null)
                    {
                        var existingSection = courseEntry.Sections
                            .FirstOrDefault(s => s.SectionTitle == section.SectionTitle);

                        if (existingSection == null)
                        {
                            existingSection = section;
                            existingSection.Videos = new List<VideoVM>();
                            courseEntry.Sections.Add(existingSection);
                        }

                        if (video != null)
                        {
                            existingSection.Videos.Add(video);
                        }
                    }

                    return courseEntry;
                },
                new { Action = "GET_DETAILS_BY_ID", CourseId = id },
                splitOn: "SectionId,VideoId",
                commandType: CommandType.StoredProcedure
            );

            return courseDict.Values.FirstOrDefault();
        }

        public async Task<Result> AddAsync(CourseDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                     "sp_Course",
                     new
                     {
                         Action = "INSERT",
                         entity.CourseTitle,
                         entity.Description,
                         entity.CategoryId,
                         entity.InstructorId,
                         entity.Price,
                         entity.Thumbnail,
                         entity.Language,
                         entity.Level,
                         entity.IsPublished,
                         entity.IsActive,
                         entity.CreatedBy
                     },
                     commandType: CommandType.StoredProcedure
                 );

                if (result > 0)
                {
                    _cache.Remove(CacheKey);
                    return new Result { Success = true, Message = "Course added successfully." };
                }

                return new Result { Success = false, Message = "Failed to add course." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(CourseDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_Course",
                    new
                    {
                        Action = "UPDATE",
                        entity.CourseId,
                        entity.CourseTitle,
                        entity.Description,
                        entity.CategoryId,
                        entity.InstructorId,
                        entity.Price,
                        entity.Thumbnail,
                        entity.Language,
                        entity.Level,
                        entity.IsPublished,
                        entity.IsActive,
                        entity.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (result > 0)
                {
                    _cache.Remove(CacheKey);
                    return new Result { Success = true, Message = "Course updated successfully." };
                }

                return new Result { Success = false, Message = "Course update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(CourseDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_Course",
                    new { Action = "DELETE", entity.CourseId },
                    commandType: CommandType.StoredProcedure
                );

                if (result > 0)
                {
                    _cache.Remove(CacheKey);
                    return new Result { Success = true, Message = "Course deleted successfully." };
                }

                return new Result { Success = false, Message = "Course could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<int> AddCourseToPlanAsync(PlanCourseDto dto)
        {
            using var db = Connection;
            var result = await db.QueryFirstAsync<int>(
                "sp_PlanCourses",
                new
                {
                    Action = "CREATE",
                    dto.PlanId,
                    dto.CourseId,
                    CreatedBy = 1
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<IEnumerable<CourseDTO>> GetCoursesByPlanAsync(int planId)
        {
            using var db = Connection;
            return await db.QueryAsync<CourseDTO>(
                "sp_PlanCourses",
                new
                {
                    Action = "GET_BY_PLAN",
                    PlanId = planId
                },
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<List<PlanCourseListDto>> GetAllCoursePlan()
        {
            using var db = Connection;

            var result = await db.QueryAsync<PlanCourseListDto>(
                "sp_PlanCourses",
                new { Action = "GET_ALL" },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task<List<PlanCourseListDto>> GetPlansByCourse(int Courseid)
        {
            using var db = Connection;

            var result = await db.QueryAsync<PlanCourseListDto>(
                "sp_PlanCourses",
                new { Action = "GET_BY_Course" , CourseId=Courseid },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task DeleteByPlanIdAsync(int planId)
        {
            using var db = Connection;

            await db.ExecuteAsync(
               "sp_PlanCourses",
                new { Action = "DELETE", PlanId = planId },
                commandType: CommandType.StoredProcedure 
            );
        }
    }
}