using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ManageMint.Auth;

public class Auth0AuthnStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly Auth0Client auth0Client;

    public Auth0AuthnStateProvider(Auth0Client client)
    {
        auth0Client = client;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(currentUser));

    public Task LogInAsync()
    {
        var loginTask = LogInAsyncCore();
        NotifyAuthenticationStateChanged(loginTask);

        return loginTask;
    }

    private async Task<AuthenticationState> LogInAsyncCore()
    {
        var user = await LoginWithAuth0Async();
        currentUser = user;

        return new AuthenticationState(currentUser);
    }

    private async Task<ClaimsPrincipal> LoginWithAuth0Async()
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
        var loginResult = await auth0Client.LoginAsync();

        if (!loginResult.IsError)
        {
            authenticatedUser = loginResult.User;
        }

        return authenticatedUser;
    }

    public async Task LogOutAsync()
    {
        await auth0Client.LogoutAsync();
        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentUser)));
    }
}