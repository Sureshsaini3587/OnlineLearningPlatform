using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using System.Data;
using static Dapper.SqlMapper;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class PQJQuestionRepository : IPQJQuestionRepository
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        public PQJQuestionRepository(IConfiguration config,IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
        public async Task<int> StartTrial(int courseId)
        {
            var guestToken = Guid.NewGuid().ToString(); 
            _httpContextAccessor.HttpContext.Response.Cookies.Append("GuestToken", guestToken); 
            using var db = Connection;

            var attemptId = await db.ExecuteScalarAsync<int>(
                @"INSERT INTO PQJ_Attempts (CourseId, GuestToken, IsGuest, AttemptDate, IsActive)
                   VALUES (@CourseId, @GuestToken, 1, GETDATE(), 1);
                   SELECT SCOPE_IDENTITY();",
                new { CourseId = courseId, GuestToken = guestToken });

            return attemptId;
        }
        public async Task SaveAnswer(SaveAnswerDTO dto)
        {
            using var db = Connection;

            await db.ExecuteAsync(
                @"IF EXISTS (SELECT 1 FROM PQJ_AttemptAnswers 
                     WHERE AttemptId=@AttemptId AND QuestionId=@QuestionId)
                  UPDATE PQJ_AttemptAnswers 
                  SET SelectedOptionId=@SelectedOptionId
                  WHERE AttemptId=@AttemptId AND QuestionId=@QuestionId
                  ELSE
                  INSERT INTO PQJ_AttemptAnswers 
                  (AttemptId, QuestionId, SelectedOptionId)
                  VALUES (@AttemptId, @QuestionId, @SelectedOptionId)",
                dto);
        }

        public async Task<List<QuestionVM>> GetFilteredQuestions(int? courseId, int? topicId, int difficulty)
        {
            using var db = Connection;

            var dict = new Dictionary<int, QuestionVM>();

            var result = await db.QueryAsync<PQJQuestion, PQJOption, PQJQuestion>(
                "sp_PQJQuestion",
                (q, opt) =>
                {
                    if (!dict.TryGetValue(q.QuestionId ?? 0, out var existing))
                    {
                        existing = new QuestionVM
                        {
                            Question = q
                        };

                        existing.Question.Options = new List<PQJOption>();

                        dict.Add(q.QuestionId ?? 0, existing);
                    }

                    if (opt != null)
                    {
                        existing.Question.Options.Add(opt);
                    }

                    return existing.Question;
                },
                new
                {
                    Action = "GET_FILTERED",
                    CourseId = courseId,
                    CategoryId = topicId,
                    DifficultyLevel = difficulty
                },
                splitOn: "OptionId"
            );

            return dict.Values.ToList();
        }
        public async Task<IEnumerable<CommanDTO>> GetCourse()
        {
            using var db = Connection;
            var data = await db.QueryAsync<CommanDTO>(
                  "sp_PQJQuestion",
                  new { Action = "GET_Course" },
                  commandType: CommandType.StoredProcedure
              );
            return data.ToList();
        }
        public async Task<IEnumerable<CommanDTO>> GetCategory()
        {
            using var db = Connection;
            var data = await db.QueryAsync<CommanDTO>(
                  "sp_PQJQuestion",
                  new { Action = "GET_Category" },
                  commandType: CommandType.StoredProcedure
              );
            return data.ToList();
        }
        public async Task<int> CreateAsync(PQJQuestion q, List<PQJOption> options, int correctOption)
        {
            using var db = Connection;
             
            var questionId = await db.ExecuteScalarAsync<int>(
                "sp_PQJQuestion",
                new
                {
                    Action = "CREATE",
                    q.QuestionText,
                    q.CategoryId,
                    q.CourseId,
                    QuestionType = (int)q.QuestionType,
                    q.DifficultyLevel,
                    q.Explanation,
                    q.Marks, 
                    q.CreatedBy
                },
                commandType: CommandType.StoredProcedure
            );
             
            for (int i = 0; i < options.Count; i++)
            {
                await db.ExecuteAsync(
                    "sp_PQJOption",
                    new
                    {
                        Action = "INSERT",
                        QuestionId = questionId,
                        OptionText = options[i].OptionText,
                        IsCorrect = (i == correctOption)
                    },
                    commandType: CommandType.StoredProcedure
                );
            }

            return questionId;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_PQJQuestion",
                new
                {
                    Action = "DELETE",
                    QuestionId = id,
                    UpdatedBy = userId
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
        public async Task<IEnumerable<PQJQuestion>> GetAllAsync()
        {
            using var db = Connection;

            return await db.QueryAsync<PQJQuestion>(
                "sp_PQJQuestion",
                new { Action = "GETALL" },
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<PQJQuestion> GetByIdAsync(int id)
        {
            using var db = Connection;

            var question = await db.QueryFirstOrDefaultAsync<PQJQuestion>(
                "sp_PQJQuestion",
                new { Action = "GETBYID", QuestionId = id },
                commandType: CommandType.StoredProcedure
            );

            if (question != null)
            {
                var options = await db.QueryAsync<PQJOption>(
                    "sp_PQJOption",
                    new { Action = "GETBYQUESTION", QuestionId = id },
                    commandType: CommandType.StoredProcedure
                );

                question.Options = options.ToList();
            }

            return question;
        }

        public async Task<bool> UpdateAsync(PQJQuestion q, List<PQJOption> options, int correctOption)
        {
            using var db = Connection;
            try
            { 
                var result = await db.ExecuteAsync(
                    "sp_PQJQuestion",
                    new
                    {
                        Action = "UPDATE",
                        q.QuestionId,
                        q.QuestionText,
                        q.CategoryId,
                        q.CourseId,
                        QuestionType = (int)q.QuestionType,
                        q.DifficultyLevel,
                        q.Explanation,
                        q.Marks, 
                        q.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );
                 
                await db.ExecuteAsync(
                    "sp_PQJOption",
                    new
                    {
                        Action = "DELETEBYQUESTION",
                        QuestionId = q.QuestionId
                    },
                    commandType: CommandType.StoredProcedure
                );
                 
                for (int i = 0; i < options.Count; i++)
                {
                    await db.ExecuteAsync(
                        "sp_PQJOption",
                        new
                        {
                            Action = "INSERT",
                            QuestionId = q.QuestionId,
                            OptionText = options[i].OptionText,
                            IsCorrect = (i == correctOption)
                        },
                        commandType: CommandType.StoredProcedure
                    );
                } 

                return result > 0;
            }
            catch (Exception)
            { 
                throw;
            }
        }
    }
}
