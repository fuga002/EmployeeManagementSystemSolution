using System.Net.Http.Json;
using EMS.BaseLibrary.Responses;
using EMS.ClientLibrary.Helpers;
using EMS.ClientLibrary.Services.Contracts;

namespace EMS.ClientLibrary.Services.Implementations;

public class GenericServiceImplementation<T>:IGenericServiceInterface<T>
{
    private readonly GetHttpClient _httpClient;

    public GenericServiceImplementation(GetHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Read All
    public async Task<List<T>> GetAll(string baseUrl)
    {
        var httpClient = await _httpClient.GetPrivateHttpClient();
        var results = await httpClient.GetFromJsonAsync<List<T>>($"{baseUrl}/all");
        return results!;
    }

    //Read Single {id}
    public async Task<T> GetById(int id, string baseUrl)
    {
        var httpClient = await _httpClient.GetPrivateHttpClient();
        var result = await httpClient.GetFromJsonAsync<T>($"{baseUrl}/single/{id}");
        return result!;
    }

    //Create
    public async Task<GeneralResponse> Inset(T item, string baseUrl)
    {
        var httpClient = await _httpClient.GetPrivateHttpClient();
        var response = await httpClient.PostAsJsonAsync($"{baseUrl}/add", item);
        var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

    //Update {model}
    public async Task<GeneralResponse> Update(T item, string baseUrl)
    {
        var httpClient = await _httpClient.GetPrivateHttpClient();
        var response = await httpClient.PutAsJsonAsync($"{baseUrl}/update", item);
        var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }

    //Delete {id}
    public async Task<GeneralResponse> DeleteById(int id, string baseUrl)
    {
        var httpClient = await _httpClient.GetPrivateHttpClient();
        var response = await httpClient.DeleteAsync($"{baseUrl}/Branch/delete/{id}");
        var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result!;
    }
}