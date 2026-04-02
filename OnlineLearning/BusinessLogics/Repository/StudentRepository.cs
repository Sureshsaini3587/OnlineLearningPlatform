using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using OnlineLearning.Models;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class StudentRepository : IStudentRepository 
    {

        private readonly IConfiguration _config;

        public StudentRepository(IConfiguration config)
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
                     entity.CreatedBy
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