using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using OnlineLearning.BusinessLogics.IRepository;
using OnlineLearning.Models;
using System.Data;

namespace OnlineLearning.BusinessLogics.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public DashboardRepository(IConfiguration config, IMemoryCache cache)
        {
            _config = config;
            _cache = cache;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DbConnection"));
            }
        }
        public async Task<int> GetTotalStudentsAsync()
        { 
            return await _cache.GetOrCreateAsync("TotalStudents", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                using (var db = Connection)  
                { 
                    return await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users WHERE RoleId=2 AND IsActive=1");
                }
            });
        }

        public async Task<int> GetActiveCoursesAsync()
        {
            using var db = Connection; 
            db.Open();
            return await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Courses WHERE IsActive = 1");
        }

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            using var db = Connection;
            db.Open();
            var sql = @"SELECT ISNULL(SUM(Amount), 0) 
                        FROM Payments 
                        WHERE MONTH(PaymentDate) = MONTH(GETDATE()) 
                        AND YEAR(PaymentDate) = YEAR(GETDATE())";
            return await db.QueryFirstOrDefaultAsync<decimal>(sql);
        }

        public async Task<int> GetPendingQueriesAsync()
        {
            using var db = Connection;
            db.Open();
            // return await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Queries WHERE IsResolved = 0");
            return 0;
        }

        public async Task<List<EnrollmentDto>> GetRecentEnrollmentsAsync(int count)
        {
            using var db = Connection;
            db.Open();
            var sql = $@"SELECT TOP {count}
                            U.FullName as StudentName, 
                            P.PlanName as PlanName, 
                            SS.CreatedOn as EnrollmentDate
                         FROM  Users U  
                         Join StudentSubscriptions SS ON SS.StudentId=U.UserId
                         JOIN SubscriptionPlans P ON P.PlanId = SS.PlanId
                         ORDER BY SS.CreatedOn DESC";

            var result = await db.QueryAsync<EnrollmentDto>(sql);
            return result.ToList();
        }

        public async Task<List<decimal>> GetRevenueStatsAsync()
        { 
            using var db = Connection;
            db.Open();
            var sql = @"SELECT SUM(Amount) as MonthlyTotal
                        FROM Payments
                        WHERE PaymentDate >= DATEADD(month, -6, GETDATE())
                        GROUP BY YEAR(PaymentDate), MONTH(PaymentDate)
                        ORDER BY YEAR(PaymentDate) DESC, MONTH(PaymentDate) DESC";

            var result = await db.QueryAsync<decimal>(sql);
            return result.ToList();
        }
    }
}
