using System.Net.Http.Json;
using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Entities;
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

    public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
    {
        var httpClient = _getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);
        if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");
        
        return await result.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<List<ManageUser>> GetUsers()
    {
        var httpClient = await _getHttpClient.GetPrivateHttpClient();
        var result = await httpClient.GetFromJsonAsync<List<ManageUser>>($"{AuthUrl}/users");
        return result!;
    }

    public async Task<GeneralResponse> UpdateUser(ManageUser manageUser)
    {
        var httpClient =  _getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PutAsJsonAsync($"{AuthUrl}/update-user",manageUser);
        return await result.Content.ReadFromJsonAsync<GeneralResponse>()!;
    }

    public async Task<List<SystemRole>> GetRoles()
    {
        var httpClient = await _getHttpClient.GetPrivateHttpClient();
        var result = await httpClient.GetFromJsonAsync<List<SystemRole>>($"{AuthUrl}/roles");
        return result!;
    }

    public async Task<GeneralResponse> DeleteUser(int userId)
    {
        var httpClient = await _getHttpClient.GetPrivateHttpClient();
        var result = await httpClient.DeleteAsync($"{AuthUrl}/delete-user/{userId}");

        if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");
        return await result!.Content.ReadFromJsonAsync<GeneralResponse>();
    }

}