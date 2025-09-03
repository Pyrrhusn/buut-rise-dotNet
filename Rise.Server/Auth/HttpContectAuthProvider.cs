using System.Security.Claims;
using Rise.Domain.Exceptions;
using Rise.Services.Auth;
using Serilog;
using Serilog.Core;

namespace Rise.Server.Auth;

public class HttpContextAuthProvider(IHttpContextAccessor httpContextAccessor) : IAuthContextProvider
{
    public ClaimsPrincipal? User => httpContextAccessor!.HttpContext?.User;

    public int? GetUserId()
    {
        if (User == null)
        {
            return null;
        }
        if (!int.TryParse(User!.Identity?.Name, out int userId))
        {
            throw new UserInvalidClaimStructureException();
        }
        return userId;
    }

    public bool IsAdmin()
    {
        if (User is null)
            return false;

        return User.IsInRole("Administrator");
    }

}
