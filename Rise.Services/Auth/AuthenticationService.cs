using System;
using Rise.Persistence;

namespace Rise.Services.Auth;

public abstract class AuthenticatedService
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly IAuthContextProvider _authContextProvider;
    protected AuthenticatedService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
    {
        if (authContextProvider.User is null)
            throw new ArgumentNullException(
                nameof(authContextProvider),
                $"{GetType().Name} requires a {nameof(authContextProvider)}"
            );

            



        _dbContext = dbContext;
        _authContextProvider = authContextProvider;
    }

}
