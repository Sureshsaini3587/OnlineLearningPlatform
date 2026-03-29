using System.Security.Claims;

namespace OnlineLearning.Helpers
{
    public static class UserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Convert.ToInt32(userId) : 0;
        }
    }
}
