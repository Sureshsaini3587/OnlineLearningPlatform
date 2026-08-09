using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace OnlineLearning.Repositories
{
    public class ReadingQuestionRepository : IReadingQuestionRepository
    { 
        private readonly IConfiguration _config; 

        public ReadingQuestionRepository(IConfiguration config )
        {
            _config = config; 
        }
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DbConnection"));
            }
        }
        public async Task<List<ReadingQuestionVM>> GetAllAsync()
        {
            using var _db = Connection;
            var query = "sp_ReadingQuestions";
            var parameters = new { Action = "GET_ALL" };
            var result = await _db.QueryAsync<ReadingQuestionVM>(query, parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
         
        public async Task<ReadingQuestionVM?> GetByIdAsync(int id)
        {
            using var _db = Connection;
            var query = "sp_ReadingQuestions";
            var parameters = new { Action = "GET_BY_ID", QuestionId = id };

            using var multi = await _db.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure);

            var question = await multi.ReadFirstOrDefaultAsync<ReadingQuestionVM>();
            if (question != null)
            {
                var options = await multi.ReadAsync<ReadingOptionVM>();
                question.Options = options.ToList();
            }

            return question;
        }
         
        public async Task<Result> AddAsync(ReadingQuestionVM model)
        {
            try
            {
                var query = "sp_ReadingQuestions";
                var parameters = new
                {
                    Action = "INSERT",
                    model.QuestionNumber,
                    model.QuestionTextEn,
                    model.QuestionTextHi,
                    model.CourseId,
                    model.CategoryId,
                    model.SubCategoryId,
                    model.ExamId,
                    model.Year,
                    model.Shift,
                    model.DifficultyLevel,
                    model.DiagramImageUrl,
                    model.ExplanationText
                };

                using var _db = Connection;
                var newQuestionId = await _db.ExecuteScalarAsync<int>(query, parameters, commandType: CommandType.StoredProcedure);

                if (newQuestionId > 0)
                { 
                    if (model.Options != null && model.Options.Any())
                    {
                        await SaveOptionsAsync(newQuestionId, model.Options);
                    }

                    return new Result { Success = true, Message = "Question added successfully." };
                }

                return new Result { Success = false, Message = "Failed to add question." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
         
        public async Task<Result> UpdateAsync(ReadingQuestionVM model)
        {
            try
            {
                var query = "sp_ReadingQuestions";
                var parameters = new
                {
                    Action = "UPDATE",
                    model.QuestionId,
                    model.QuestionNumber,
                    model.QuestionTextEn,
                    model.QuestionTextHi,
                    model.CourseId,
                    model.CategoryId,
                    model.SubCategoryId,
                    model.ExamId,
                    model.Year,
                    model.Shift,
                    model.DifficultyLevel,
                    model.DiagramImageUrl,
                    model.ExplanationText,
                    model.IsActive
                };
                using var _db = Connection;

                var affectedRows = await _db.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);

                if (affectedRows > 0)
                { 
                    if (model.Options != null && model.Options.Any())
                    {
                        await SaveOptionsAsync(model.QuestionId, model.Options);
                    }

                    return new Result { Success = true, Message = "Question updated successfully." };
                }

                return new Result { Success = false, Message = "Question not found or update failed." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
         
        public async Task<Result> DeleteAsync(ReadingQuestionVM model)
        {
            try
            {
                using var _db = Connection;
                var query = "sp_ReadingQuestions";
                var parameters = new { Action = "DELETE", QuestionId = model.QuestionId };
                var affectedRows = await _db.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);

                return affectedRows > 0
                    ? new Result { Success = true, Message = "Question deleted successfully." }
                    : new Result { Success = false, Message = "Question not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
         
        public async Task SaveOptionsAsync(int questionId, List<ReadingOptionVM> options)
        {
            using var _db = Connection;
            await _db.ExecuteAsync("DELETE FROM ReadingOptions WHERE QuestionId = @QuestionId", new { QuestionId = questionId });
             
            foreach (var opt in options)
            {
                var insertQuery = @"INSERT INTO ReadingOptions (QuestionId, OptionTextEn, OptionTextHi, IsCorrect) 
                                    VALUES (@QuestionId, @OptionTextEn, @OptionTextHi, @IsCorrect)";

                await _db.ExecuteAsync(insertQuery, new
                {
                    QuestionId = questionId,
                    opt.OptionTextEn,
                    opt.OptionTextHi,
                    opt.IsCorrect
                });
            }
        }
    }
}