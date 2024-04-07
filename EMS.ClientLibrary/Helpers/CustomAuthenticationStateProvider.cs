using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EMS.BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace EMS.ClientLibrary.Helpers;

public class CustomAuthenticationStateProvider:AuthenticationStateProvider
{
    private readonly LocalStorageService _localStorageService;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(LocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var stringToken = await _localStorageService.GetToken();
        if (string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(_anonymous));

        var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
        if (deserializeToken is null) return await Task.FromResult(new AuthenticationState(_anonymous));

        var getUserClaims = DecryptToken(deserializeToken.Token);
        if (getUserClaims is null) return await Task.FromResult(new AuthenticationState(_anonymous));

        var claimsprincipal = SetClaimPrincipal(getUserClaims);
        return await Task.FromResult(new AuthenticationState(claimsprincipal));
    }

    private ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
    {
        if (claims.Email is null) return new ClaimsPrincipal();
        return new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
            {
                new (ClaimTypes.NameIdentifier,claims.Id),
                new(ClaimTypes.Name,claims.Name),
                new(ClaimTypes.Email,claims.Email),
                new(ClaimTypes.Role,claims.Role)
            },
            "JwtAuth"));
    }


    private static CustomUserClaims DecryptToken(string? jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var name = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        var email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        var role = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        return new CustomUserClaims(userId?.Value!, name?.Value!, email?.Value!, role?.Value!);
    }
}