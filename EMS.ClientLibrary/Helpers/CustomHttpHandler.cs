using System.Net;
using EMS.BaseLibrary.DTOs;
using EMS.ClientLibrary.Services.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace EMS.ClientLibrary.Helpers;

public class CustomHttpHandler:DelegatingHandler
{
     private readonly LocalStorageService _localStorageService;
     private readonly GetHttpClient _getHttpClient;
     private readonly IUserAccountService _accountService;

     public CustomHttpHandler(LocalStorageService localStorageService, GetHttpClient getHttpClient, IUserAccountService accountService)
     {
          _localStorageService = localStorageService;
          _getHttpClient = getHttpClient;
          _accountService = accountService;
     }

     protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
     {
          bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login");
          bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("register");
          bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("refresh-token");
          if(registerUrl || loginUrl || refreshTokenUrl)  return await base.SendAsync(request, cancellationToken);
          
          var result  = await base.SendAsync(request, cancellationToken);
          if (result.StatusCode == HttpStatusCode.Unauthorized)
          {
               var stringToken = await _localStorageService.GetToken();
               if (stringToken is null) return result;
               string token = string.Empty;
               try
               {
                    token = request.Headers.Authorization!.Parameter!;
               }
               catch
               {
                    
               }

               var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
               if (deserializedToken is null) return result;
               
               if (string.IsNullOrEmpty(token))
               {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",deserializedToken.Token);
               }

               var newJwtToken = await GetRefreshToken(deserializedToken.RefreshToken);
               if (string.IsNullOrEmpty(newJwtToken)) return result;
               
               request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",newJwtToken);
               return await base.SendAsync(request, cancellationToken);
          }

          return result;
     }

     private async Task<string> GetRefreshToken(string? refreshToken)
     {
          var result = await _accountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
          string serializedToken = Serializations.SerializationObj(new UserSession()
               { Token = result.Token, RefreshToken = result.RefreshToken });

          return result.Token;
     }
}