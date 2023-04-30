using System.Security.Claims;

namespace FinancioBackend.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetId()
    {
        var result = 0;

        if (_httpContextAccessor != null)
        {
            var userClaim = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            result = int.TryParse(userClaim, out var id) ? id : 0;
        }

        return result;
    }

    public string GetEmail()
    {
        var result = "";

        if (_httpContextAccessor != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        }

        return result;
    }
}