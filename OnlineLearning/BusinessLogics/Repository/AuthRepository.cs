using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.Services;
using OnlineLearning.Helpers;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
using Org.BouncyCastle.Crypto.Generators;
using System.Data;

public class AuthRepository : IAuthRepository
{

    private readonly IConfiguration _config;

    public AuthRepository(IConfiguration config)
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
    public async Task<User> LoginUserAsync(LoginViewModel loginViewModel)
    {

        using var db = Connection;

        var parameters = new DynamicParameters();
        parameters.Add("@Email", loginViewModel.Email);

        var user = await db.QueryFirstOrDefaultAsync<User>(
            "sp_UserLogin",
            parameters,
            commandType: CommandType.StoredProcedure);

        return user;
    }
    public async Task<User> RegisterUser(RegisterViewModel registerViewModel)
    {
        using var db = Connection;
        string checkQuery = "SELECT COUNT(1) FROM Users WHERE LOWER(Email) = LOWER(@Email)";
         
        string insertQuery = @"
            INSERT INTO Users
             (FullName, Email, Mobile,PasswordHash ,RoleId, IsActive, IsDeleted,  CreatedOn)
             VALUES
             (@FullName, @Email, @Mobile, @Pswd,2, 1, 0, GETDATE())

            DECLARE @NewUserId INT = SCOPE_IDENTITY();
 
           INSERT INTO StudentProfile
           (StudentId, IsActive, IsDeleted, CreatedOn)
           VALUES
           (@NewUserId,  1, 0, GETDATE());
 
          SELECT @NewUserId;";

        string passwordHash = PasswordHelper.HashPassword(registerViewModel.Password);
         DateTime createdAt = DateTime.UtcNow; 
         int exists = await db.ExecuteScalarAsync<int>(checkQuery, new { Email = registerViewModel.Email });
         if (exists > 0)
         {
             return null;  
         }

        int newId = await db.ExecuteScalarAsync<int>(insertQuery, new
        {
            FullName = registerViewModel.FullName,
            Mobile = registerViewModel.MobileNumber,
            Email = registerViewModel.Email,
            Pswd = passwordHash
        });

        if (newId > 0)
         {
            var model = new LoginViewModel
            {
                Email = registerViewModel.Email
            };
           var User= await LoginUserAsync(model);
            return User;
         }
       
        return null;
    }

    public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
    {
        using var db = Connection;
         
        string updateQuery = @"
        UPDATE Users 
        SET PasswordHash = @PasswordHash 
        WHERE UserId = @UserId  ";
        string passwordHash = PasswordHelper.HashPassword(newPasswordHash);
        int rowsAffected = await db.ExecuteAsync(updateQuery, new
        {
            UserId = userId,
            PasswordHash = passwordHash
        });

        return rowsAffected > 0;
    }
}