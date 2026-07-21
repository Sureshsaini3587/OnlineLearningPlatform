using OnlineLearning.DTO;

namespace OnlineLearning.BusinessLogics.IRepository
{
    public interface ISubscriberRepository
    {
        Task<bool> ExistsAsync(string email);
        Task<bool> AddAsync(SubscriberDTO subscriber);
    }
}
