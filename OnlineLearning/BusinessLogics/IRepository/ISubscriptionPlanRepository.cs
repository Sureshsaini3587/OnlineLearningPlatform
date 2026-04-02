using OnlineLearning.DTO;
using OnlineLearning.Models;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ISubscriptionPlanRepository : ICommonRepository<SubscriptionPlanDTO>
    {
       Task<List<SubscriptionPlanDTO>> GetAllWithDetails();
    }
}
