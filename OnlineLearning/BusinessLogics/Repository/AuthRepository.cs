using Dapper;
using Microsoft.Data.SqlClient;
using OnlineLearning.BusinessLogics.Services;
using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;
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

   
}