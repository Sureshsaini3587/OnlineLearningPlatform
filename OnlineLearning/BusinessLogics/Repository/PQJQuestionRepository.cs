using Azure;
using Azure.Core;
using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;
using System.Security.Claims;
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
        private string GetGuestToken()
        {
            string token =
               _httpContextAccessor.HttpContext.Request.Cookies["PQJ_GUEST"];

            if (string.IsNullOrEmpty(token))
            {
                token = Guid.NewGuid().ToString();

                _httpContextAccessor.HttpContext.Response.Cookies.Append(
                    "PQJ_GUEST",
                    token,
                    new CookieOptions
                    {
                        Expires =
                            DateTime.Now.AddDays(7),

                        HttpOnly = true,

                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    });
            }

            return token;
        }
        public async Task<int> GetOrCreateAttempt( int courseId)
        {
            bool isLoggedIn =
                _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

            int? studentId = null;

            string guestToken = null;

            if (isLoggedIn)
            {
                studentId =
                    Convert.ToInt32(
                        _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            else
            {
                guestToken = GetGuestToken();
            }
             
            int attemptId =
                await GetExistingAttempt(  courseId, studentId, guestToken);
             
            if (attemptId == 0)
            {
                attemptId =
                    await CreateAttempt( courseId, studentId, guestToken);
            }

            return attemptId;
        }
        public async Task<int> GetExistingAttempt(  int courseId, int? studentId, string guestToken)
        {
            using var db = Connection;

            if (studentId.HasValue)
            {
                return await db.ExecuteScalarAsync<int>(
                    @"
                     SELECT TOP 1 AttemptId
                     FROM PQJ_Attempts
                     WHERE StudentId=@StudentId
                     AND CourseId=@CourseId
                     AND IsCompleted=0
                     ",
                    new
                    {
                        StudentId = studentId,
                        CourseId = courseId
                    });
            }

            return await db.ExecuteScalarAsync<int>(
                @"
                   SELECT TOP 1 AttemptId
                   FROM PQJ_Attempts
                   WHERE GuestToken=@GuestToken
                   AND CourseId=@CourseId
                   AND IsCompleted=0
                ",
                new
                {
                    GuestToken = guestToken,
                    CourseId = courseId
                });
        }
        public async Task<int> CreateAttempt( int courseId,   int? studentId,  string guestToken)
        {
            using var db = Connection;

            return await db.ExecuteScalarAsync<int>(
                @"
                  INSERT INTO PQJ_Attempts
                  (
                      StudentId,
                      CourseId,
                      GuestToken,
                      AttemptDate,
                      IsCompleted,
                      IsActive,
                      IsDeleted
                  )
                  VALUES
                  (
                      @StudentId,
                      @CourseId,
                      @GuestToken,
                      GETDATE(),
                      0,
                      1,
                      0
                  )
                 
                  SELECT CAST(SCOPE_IDENTITY() AS INT)
                  ",
                new
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    GuestToken = guestToken
                });
        }
        public async Task<SaveAnswerResultDTO> SaveAnswer( SaveAnswerDTO dto)
        {
            using var db = Connection;
             
            int correctOptionId =
                await db.ExecuteScalarAsync<int>(
                    @"  SELECT TOP 1 OptionId   FROM PQJ_Options   WHERE QuestionId=@QuestionId   AND IsCorrect=1  ",
                    new
                    {
                        dto.QuestionId
                    });

            bool isCorrect =  correctOptionId ==  dto.SelectedOptionId;
             
            await db.ExecuteAsync(
                @"
                  IF EXISTS  (  SELECT 1  FROM PQJ_AttemptAnswers  WHERE AttemptId=@AttemptId  AND QuestionId=@QuestionId ) 
                  UPDATE PQJ_AttemptAnswers
                     SET SelectedOptionId=@SelectedOptionId,   IsCorrect=@IsCorrect,   UpdatedOn=GETDATE()
                        WHERE AttemptId=@AttemptId  AND QuestionId=@QuestionId 
                  ELSE 
                     INSERT INTO PQJ_AttemptAnswers
                     (
                         AttemptId,
                         QuestionId,
                         SelectedOptionId,
                         IsCorrect,
                         CreatedOn
                     )
                     VALUES
                     (
                         @AttemptId,
                         @QuestionId,
                         @SelectedOptionId,
                         @IsCorrect,
                         GETDATE()
                     )
                ",
                new
                {
                    dto.AttemptId,
                    dto.QuestionId,
                    dto.SelectedOptionId,
                    IsCorrect = isCorrect
                });

            return new SaveAnswerResultDTO
            {
                IsCorrect = isCorrect,

                CorrectOptionId = correctOptionId
            };
        }
        
        public async Task<List<QuestionVM>> GetFilteredQuestions(int? courseId, int? topicId, int difficulty,string Mode)
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
                    DifficultyLevel = difficulty,
                    Mode = Mode
                },
                splitOn: "OptionId"
            );

            return dict.Values.ToList();
        }
        public async Task CompleteAttempt(int attemptId)
        {
            using var db = Connection;
             
            var result = await db.QueryFirstOrDefaultAsync<dynamic>(
                @"
                   SELECT
                       COUNT(*) AS TotalQuestions, 
                       SUM(
                           CASE
                               WHEN o.IsCorrect = 1 THEN 1
                               ELSE 0
                           END
                       ) AS CorrectAnswers 
                   FROM PQJ_AttemptAnswers a 
                   INNER JOIN PQJ_Options o
                       ON a.SelectedOptionId = o.OptionId 
                   WHERE a.AttemptId = @AttemptId
                   ",
                new { AttemptId = attemptId });

            int totalQuestions =
                result?.TotalQuestions ?? 0;

            int correctAnswers =
                result?.CorrectAnswers ?? 0;

            int wrongAnswers =  totalQuestions - correctAnswers;

            decimal score = 0;

            if (totalQuestions > 0)
            {
                score =  ((decimal)correctAnswers  / totalQuestions) * 100;
            }
             
            await db.ExecuteAsync(
                @"
                      UPDATE PQJ_Attempts
                      SET
                          IsCompleted = 1,
                          UpdatedOn = GETDATE(),
                          TotalQuestions = @TotalQuestions,
                          CorrectAnswers = @CorrectAnswers,
                          WrongAnswers = @WrongAnswers,
                          Score = @Score
                   
                      WHERE AttemptId = @AttemptId
                    ",
                new
                {
                    AttemptId = attemptId,
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    WrongAnswers = wrongAnswers,
                    Score = score
                });
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
        public async Task<Result> CreateAsync(PQJQuestion q, List<PQJOption> options, int correctOption)
        {
            using var db = Connection;
            try
            {
                var questionId = await db.ExecuteScalarAsync<int>(
                "sp_PQJQuestion",
                new
                {
                         Action = "CREATE",
                         q.QuestionText,
                         q.CategoryId,
                         q.CourseId,
                         QuestionType = (int)q.QuestionType,
                         q.IsTrial,
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
                if (questionId > 0)
                { 
                    return new Result { Success = true, Message = "Question added successfully." };
                }
                return new Result { Success = false, Message = "Failed to add question." };
             }
            catch (Exception ex)
            {
                         return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            } 
        }

        public async Task<Result> DeleteAsync(int id, int userId)
        {
            using var db = Connection;
            try
            {
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

                if (result > 0)
                { 
                    return new Result { Success = true, Message = "Question deleted successfully." };
                }

                return new Result { Success = false, Message = "Question could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
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

        public async Task<Result> UpdateAsync(PQJQuestion q, List<PQJOption> options, int correctOption)
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
                        q.IsTrial,
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
                if (result > 0)
                {
                    return new Result { Success = true, Message = "Question update successfully." };
                }
                return new Result { Success = false, Message = "Failed to update question." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
         
        }
    }
}
