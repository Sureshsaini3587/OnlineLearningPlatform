using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;
using System.Security.Claims;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class StudentRepository : IStudentRepository 
    {
        private readonly ILogger<StudentRepository> _logger; 
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentRepository(IConfiguration config, IMemoryCache cache, IHttpContextAccessor httpContextAccessor, ILogger<StudentRepository> logger)
        {
            _config = config;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(
                    _config.GetConnectionString("DbConnection"));
            }
        }
        public async Task<CourseDetailsVM> GetCourseDetailsById(int id)
        {
            try
            {
                using var db = Connection;
                var parameters = new { Action = "GET_Course_DETAILS_BY_ID", CourseId = id };
                 
                var rawResult = await db.QueryAsync<dynamic>("sp_Student", parameters, commandType: CommandType.StoredProcedure);
                var rawRows = rawResult.ToList();

                if (!rawRows.Any()) return null;

                var courseDict = new Dictionary<int, CourseDetailsVM>();

                foreach (var row in rawRows)
                {
                    int courseId = (int)row.CourseId;
                     
                    if (!courseDict.TryGetValue(courseId, out var courseEntry))
                    {
                        courseEntry = new CourseDetailsVM
                        {
                            CourseId = courseId,
                            CourseTitle = row.CourseTitle,
                            Description = row.Description,
                            Price = row.Price != null ? (decimal)row.Price : 0,
                            Thumbnail = row.Thumbnail,
                            LevelName = row.LevelName,
                            CategoryName = row.CategoryName,
                            Sections = new List<SectionVM>()
                        };
                        courseDict.Add(courseId, courseEntry);
                    }
                     
                    if (row.SectionId != null && (int)row.SectionId > 0)
                    {
                        int sectionId = (int)row.SectionId;
                         
                        var existingSection = courseEntry.Sections.FirstOrDefault(s => s.SectionId == sectionId);
                         
                        if (existingSection == null)
                        {
                            existingSection = new SectionVM
                            {
                                SectionId = sectionId,
                                SectionTitle = row.SectionTitle,
                                SectionOrder = row.SectionOrder,  
                                Videos = new List<VideoVM>()
                            };
                            courseEntry.Sections.Add(existingSection);
                        }
                         
                        if (row.VideoId != null && (int)row.VideoId > 0)
                        {
                            int videoId = (int)row.VideoId;
                             
                            if (!existingSection.Videos.Any(v => v.VideoId == videoId))
                            {
                                existingSection.Videos.Add(new VideoVM
                                {
                                    VideoId = videoId,
                                    Title = row.Title,
                                    IsDemo = row.IsDemo != null ? Convert.ToBoolean(row.IsDemo) : false,
                                    VideoUrl = row.VideoUrl,
                                    VideoOrder = row.VideoOrder,  
                                    Duration = row.Duration != null ? (int)row.Duration : 0
                                }
                                );
                            }
                        }
                    }
                }
                return courseDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Runtime tracking error inside data matrix transformation for Course: {Id}", id);
                throw;
            }
        }
        public async Task<bool> CheckCourseAccessAsync(int userId, int courseId)
        {
            using var db = Connection;
            var hasAccess = await db.ExecuteScalarAsync<int>(
                "sp_Student",
                new { Action = "CHECK_COURSE_ACCESS", UserId = userId, CourseId = courseId },
                commandType: CommandType.StoredProcedure
            );

            return hasAccess == 1;
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

        public async Task<Result> AddAsync(StudentDTO entity)
        {
            try
            {
                var pswd = PasswordHelper.HashPassword("Std@123");
                using var db = Connection;
                 
                var result = await db.QueryFirstOrDefaultAsync<int>(
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
                         Pswd = pswd
                     },
                     commandType: CommandType.StoredProcedure
                 );

                if (result == -1) return new Result { Success = false, Message = "Student email already exists." };
                return result > 0
                    ? new Result { Success = true, Message = "Student added successfully." }
                    : new Result { Success = false, Message = "Failed to add student." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(StudentDTO entity)
        {
            try
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

                return result > 0
                    ? new Result { Success = true, Message = "Student updated successfully." }
                    : new Result { Success = false, Message = "Student update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(StudentDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_Student",
                    new { Action = "DELETE", entity.UserID },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Student deleted successfully." }
                    : new Result { Success = false, Message = "Student could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
    }
}