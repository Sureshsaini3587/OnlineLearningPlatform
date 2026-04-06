using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
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

        public async Task<bool> AddAsync(InstructorDTO entity)
        {
            var Pswd = PasswordHelper.HashPassword("Ins@123");
            using var db = Connection;
            var result = await db.ExecuteAsync(
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
                     Pswd 
                 },
                 commandType: CommandType.StoredProcedure
             ); 
            return result > 0; 
        }
        public async Task<bool> UpdateAsync(InstructorDTO entity)
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

            return result > 0;
        }
        public async Task<bool> DeleteAsync(InstructorDTO entity)
        {
            using var db = Connection;

            var result = await db.ExecuteAsync(
                "sp_Instructor",
                new { Action = "DELETE", entity.InstructorId },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        } 
    }
}