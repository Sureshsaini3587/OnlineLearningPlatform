using OnlineLearning.Models;
using OnlineLearning.Models.ResponseModel;

namespace OnlineLearning.BusinessLogics.Services
{
    public interface IAuthRepository
    {
        Task<User> LoginUserAsync(LoginViewModel loginViewModel);
        Task<User> RegisterUser(RegisterViewModel registerViewModel);
    }
}
