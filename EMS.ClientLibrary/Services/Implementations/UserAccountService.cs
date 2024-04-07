using System.Net.Http.Json;
using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Responses;
using EMS.ClientLibrary.Helpers;
using EMS.ClientLibrary.Services.Contracts;

namespace EMS.ClientLibrary.Services.Implementations;

public class UserAccountService:IUserAccountService
{
    private readonly GetHttpClient _getHttpClient;
    public const string AuthUrl = "api/authentication";

    public UserAccountService(GetHttpClient httpClient)
    {
        _getHttpClient = httpClient;
    }

    public async Task<GeneralResponse?> CreateAsync(Register user)
    {
        var httpClient = _getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
        if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");

        return await result.Content.ReadFromJsonAsync<GeneralResponse>();
    }

    public async Task<LoginResponse?> SignInAsync(Login user)
    {
        var httpClient = _getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
        if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");
        
        return await result.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<WeatherForecast[]> GetWeatherForecast()
    {
        var httpClient = await _getHttpClient.GetPrivateHttpClient();
        var result = await httpClient.GetFromJsonAsync<WeatherForecast[]>("api/WeatherForecast");
        return result!;
    }
}