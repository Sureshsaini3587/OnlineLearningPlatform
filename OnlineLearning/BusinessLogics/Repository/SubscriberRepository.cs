using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.DTO;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public SubscriberRepository(IConfiguration config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(
                    _config.GetConnectionString("DbConnection"));
            }
        }

        public async Task<bool> ExistsAsync(string email)
        {
            const string sql = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 
                    FROM Subscribers 
                    WHERE LOWER(Email) = LOWER(@Email)
                ) THEN 1 ELSE 0 END;";

            using var connection = Connection;
             
            int exists = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
            return exists == 1;
        }

        public async Task<bool> AddAsync(SubscriberDTO subscriber)
        {
            const string sql = @"
                INSERT INTO Subscribers (Email, SubscribedAt, IsActive)
                VALUES (@Email, @SubscribedAt, @IsActive);";

            using var connection = Connection;
             
            int rowsAffected = await connection.ExecuteAsync(sql, subscriber);
            return rowsAffected > 0;
        }
    }
}
