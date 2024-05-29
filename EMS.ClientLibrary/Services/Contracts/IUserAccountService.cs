using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;

namespace EMS.ClientLibrary.Services.Contracts;

public interface IUserAccountService
{
    Task<GeneralResponse?> CreateAsync(Register user);
    Task<LoginResponse?> SignInAsync(Login user);
    Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
    Task<List<ManageUser>> GetUsers();
    Task<GeneralResponse> UpdateUser(ManageUser manageUser);
    Task<List<SystemRole>> GetRoles();
    Task<GeneralResponse> DeleteUser(int userId);
}