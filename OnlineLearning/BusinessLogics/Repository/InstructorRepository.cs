using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class InstructorRepository : IInstructorRepository 
    {

        private readonly IConfiguration _config;

        public InstructorRepository(IConfiguration config)
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
         
        public async Task<List<InstructorDTO>> GetAllAsync()
        {  
            using var db = Connection; 
            var data = await db.QueryAsync<InstructorDTO>(
                  "sp_Instructor",
                  new { Action = "GET_ALL" },
                  commandType: CommandType.StoredProcedure
              ); 
            return data.ToList();
        } 
        public async Task<List<InstructorDTO>> GetAllWithDetails()
        {
            using var db = Connection;

            var data = await db.QueryAsync<InstructorDTO>(
                "sp_Instructor",                
                new { Action = "GET_ALL_DETAILS" },
                commandType: CommandType.StoredProcedure                
            );

            return data.ToList();
        }

        public async Task<InstructorDTO?> GetByIdAsync(int id)
        {  
            using var db = Connection;
            return await db.QueryFirstOrDefaultAsync<InstructorDTO>(
                "sp_Instructor",
                new { Action = "GET_BY_ID", InstructorId = id },
                commandType: CommandType.StoredProcedure
            );  
        }

        public async Task<Result> AddAsync(InstructorDTO entity)
        {
            try
            {
                var pswd = PasswordHelper.HashPassword("Ins@123");
                using var db = Connection;
                 
                var result = await db.QueryFirstOrDefaultAsync<int>(
                     "sp_Instructor",
                     new
                     {
                         Action = "INSERT",
                         entity.FullName,
                         entity.Email,
                         entity.Mobile,
                         entity.Role,
                         entity.Bio,
                         entity.ExperienceYears,
                         entity.ProfileImage,
                         entity.IsActive,
                         entity.CreatedBy,
                         Pswd = pswd
                     },
                     commandType: CommandType.StoredProcedure
                 );

                if (result == -1) return new Result { Success = false, Message = "Email already exists." };
                return result > 0
                    ? new Result { Success = true, Message = "Instructor added successfully." }
                    : new Result { Success = false, Message = "Failed to add instructor." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> UpdateAsync(InstructorDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_Instructor",
                    new
                    {
                        Action = "UPDATE",
                        entity.InstructorId,
                        entity.FullName,
                        entity.Email,
                        entity.Mobile,
                        entity.Role,
                        entity.Bio,
                        entity.ExperienceYears,
                        entity.ProfileImage,
                        entity.IsActive,
                        entity.UpdatedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Instructor updated successfully." }
                    : new Result { Success = false, Message = "Instructor update failed or not found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }

        public async Task<Result> DeleteAsync(InstructorDTO entity)
        {
            try
            {
                using var db = Connection;
                var result = await db.ExecuteAsync(
                    "sp_Instructor",
                    new { Action = "DELETE", entity.InstructorId },
                    commandType: CommandType.StoredProcedure
                );

                return result > 0
                    ? new Result { Success = true, Message = "Instructor deleted successfully." }
                    : new Result { Success = false, Message = "Instructor could not be found." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = $"Database error: {ex.Message}" };
            }
        }
    }
}