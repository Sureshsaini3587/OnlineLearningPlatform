using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalStudentsAsync();
        Task<int> GetActiveCoursesAsync();
        Task<decimal> GetMonthlyRevenueAsync();
        Task<int> GetPendingQueriesAsync();
        Task<List<EnrollmentDto>> GetRecentEnrollmentsAsync(int count);
        Task<List<decimal>> GetRevenueStatsAsync();
    }
}
