using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Responses;

namespace EMS.APILibrary.Repositories.Contracts;

public interface IUserAccount
{
    Task<GeneralResponse> CreateAsync(Register? user);
    Task<LoginResponse> SigninAsync(Login user);
}