using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class CourseRepository : ICourseRepository
    {

        private readonly IConfiguration _config;

        public CourseRepository(IConfiguration config)
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

        // 📋 GET ALL
        public async Task<List<Course>> GetAllAsync()
        {
            var query = "SELECT * FROM Courses WHERE IsDeleted = 0";

            using var db = Connection; 
            var data = await db.QueryAsync<Course>(query);

            return data.ToList();
        }

        // 📋 GET ALL WITH DETAILS (JOIN)
        public async Task<List<Course>> GetAllWithDetails()
        {
            var query = @"
                    SELECT c.*, 
                           cat.CategoryId, cat.CategoryName,
                           lvl.LevelId, lvl.LevelName
                    FROM Courses c
                    LEFT JOIN CourseCategories cat ON c.CategoryId = cat.CategoryId
                    LEFT JOIN CourseLevels lvl ON c.Level = lvl.LevelId
                    WHERE c.IsDeleted = 0";


            using var db = Connection;

            var data = await db.QueryAsync<Course, Coursecategories, CourseLevel, Course>(
                query,
                (course, category, level) =>
                {
                    course.Category = category;
                    course.Level = level;
                    return course;
                },
                splitOn: "CategoryId,Level"
            );

            return data.ToList();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Courses WHERE CourseId = @Id AND IsDeleted = 0";

            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<Course>(query, new { Id = id });

        }
        public async Task<bool> AddAsync(Course entity)
        {
            var query = @"
                INSERT INTO Courses
                (CourseTitle, Description, CategoryId, InstructorId, Price, Thumbnail, Language, Level,
                 IsPublished, IsActive, IsDeleted, CreatedBy, CreatedOn)
                VALUES
                (@CourseTitle, @Description, @CategoryId, @InstructorId, @Price, @Thumbnail, @Language, @Level,
                 @IsPublished, @IsActive, 0, @CreatedBy, GETDATE())";

            using var db = Connection;
            var result = await db.ExecuteAsync(query, entity);

            return result > 0;
        }
         
        public async Task<bool> UpdateAsync(Course entity)
        {
            var query = @"
              UPDATE Courses SET
                  CourseTitle = @CourseTitle,
                  Description = @Description,
                  CategoryId = @CategoryId,
                  InstructorId = @InstructorId,
                  Price = @Price,
                  Thumbnail = @Thumbnail,
                  Language = @Language,
                  Level = @Level,
                  IsPublished = @IsPublished,
                  IsActive = @IsActive,
                  UpdatedBy = @UpdatedBy,
                  UpdatedOn = GETDATE()
              WHERE CourseId = @CourseId";

            using var db = Connection;
            var result = await db.ExecuteAsync(query, entity);

            return result > 0;
        }
         
        public async Task<bool> DeleteAsync(Course entity)
        {
            var query = @"
             UPDATE Courses 
             SET IsDeleted = 1, UpdatedOn = GETDATE()
             WHERE CourseId = @CourseId";

            using var db = Connection;
            var result = await db.ExecuteAsync(query, new { entity.CourseId });

            return result > 0;
        }
    }
}