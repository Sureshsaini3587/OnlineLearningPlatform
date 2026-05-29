using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using System.Data;
using System.Security.Claims;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class StudentRepository : IStudentRepository 
    { 
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentRepository(IConfiguration config, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(
                    _config.GetConnectionString("DbConnection"));
            }
        }
        public async Task<StudentProfileViewModel> GetStudentProfileAsync(int studentId)
        {
            string query = @"
            SELECT 
                u.UserId AS StudentId, u.FullName, u.Email, u.Mobile,
                p.DOB, p.Gender, p.Address, p.ProfileImage
            FROM Users u
            LEFT JOIN StudentProfile p ON u.UserId = p.StudentId
            WHERE u.UserId = @StudentId AND u.IsDeleted = 0"; 
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<StudentProfileViewModel>(query, new { StudentId = studentId });
        }
         
        public async Task<bool> UpdateStudentProfileAsync(StudentProfileViewModel model)
        {
            string updateQuery = @" 
            UPDATE Users 
            SET FullName = @FullName, Mobile = @Mobile 
            WHERE UserId = @StudentId;
 
            UPDATE StudentProfile
            SET DOB = @DOB, Gender = @Gender, Address = @Address, ProfileImage= @ProfileImage ,
                UpdatedBy = @StudentId, UpdatedOn = GETDATE()
            WHERE StudentId = @StudentId;";
            using var db = Connection;
            int rowsAffected = await db.ExecuteAsync(updateQuery, model);
            return rowsAffected > 0;
        }
        public async Task<List<CourseDTO>> GetCourse()
        {
            
            int studentId = Convert.ToInt32( _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            string cacheKey =  $"Courses_{studentId}"; 
            if (_cache.TryGetValue(cacheKey, out List<CourseDTO> courses))
            {
                return courses;
            }
            using var db = Connection;

            var data = await db.QueryAsync<CourseDTO>(
                "sp_Student",
                new { Action = "GET_Course", StudentId = studentId },
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

            return courses;
        }
        public async Task<StudentDashboardVM> GetDashboard(int userId)
        {
            using var db = Connection;

            var multi = await db.QueryMultipleAsync("sp_Student",
                new { Action = "GET_Dashboard" , UserId = userId },
                commandType: CommandType.StoredProcedure);

            var vm = new StudentDashboardVM();

            var summary = await multi.ReadFirstAsync();

            vm.UserName = summary.UserName;
            vm.PlanName = summary.PlanName;
            vm.DaysLeft = summary.DaysLeft;
            vm.TotalCourses = summary.TotalCourses;
            vm.InProgressCourses = summary.InProgressCourses;
            vm.HoursWatched = summary.HoursWatched;
            vm.PQJScore = summary.PQJScore; 
            vm.ContinueCourse = await multi.ReadFirstOrDefaultAsync<ContinueCourseVM>();
            vm.RecentCourses = (await multi.ReadAsync<CourseDTO>()).ToList();

            return vm;
        }
        public async Task<List<CourseDTO>> GetStudentCourses(int studentId)
        {
            using var db = Connection;

            var data =
                await db.QueryAsync<CourseDTO>(
                @"
                     SELECT DISTINCT  c.CourseId,  c.CourseTitle,    c.Description,   c.Thumbnail,c.IsActive,   L.LevelName,  CT.CategoryName,   ss.EndDate
                      FROM StudentSubscriptions ss 
                     INNER JOIN PlanCourses pc  ON ss.PlanId = pc.PlanCourseId 
                     INNER JOIN Courses c   ON pc.CourseId = c.CourseId 
                     INNER JOIN CourseCategories CT ON CT.CategoryId=C.CategoryId
                     INNER JOIN CourseLevels L ON L.LevelId=c.Level
                     WHERE ss.StudentId = @StudentId
                     AND ss.IsActive = 1
                     AND ss.EndDate >= GETDATE()
                ",
                new
                {
                    StudentId = studentId
                });

            return data.ToList();
        }
        public async Task<List<StudentDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<StudentDTO>(
                  "sp_Student",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }
         
        public async Task<List<GenderDTO>> GetGender()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<GenderDTO>(
                  "sp_Student",
                  new { Action = "GET_Gender" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        }
        public async Task<List<StudentDTO>> GetAllWithDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<StudentDTO>(
                "sp_Student",                
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure                
            );

            return data.ToList();
        }

        public async Task<StudentDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<StudentDTO>(
                "sp_Student",
                new { Action = "GET_BY_ID", UserID = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<bool> AddAsync(StudentDTO entity)
        {
            var Pswd = PasswordHelper.HashPassword("Std@123");
            using var db = Connection;
            var result = await db.ExecuteAsync(
                 "sp_Student",
                 new
                 {
                     Action = "INSERT",
                     entity.FullName, 
                     entity.Email, 
                     entity.DOB, 
                     entity.Address, 
                     entity.Mobile, 
                     entity.Role, 
                     entity.Gender, 
                     entity.ProfileImage, 
                     entity.IsActive,
                     entity.CreatedBy,
                     Pswd
                 },
                 commandType: CommandType.StoredProcedure
             ); 
            return result > 0; 
        }
        public async Task<bool> UpdateAsync(StudentDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_Student",
                new
                {
                    Action = "UPDATE",
                    entity.StudentId,
                    entity.UserID,
                    entity.FullName,
                    entity.Email,
                    entity.DOB,
                    entity.Address,
                    entity.Mobile,
                    entity.Role,
                    entity.Gender,
                    entity.ProfileImage,
                    entity.IsActive,
                    entity.UpdatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
        public async Task<bool> DeleteAsync(StudentDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_Student",
                new { Action = "DELETE", entity.UserID },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        } 
    }
}