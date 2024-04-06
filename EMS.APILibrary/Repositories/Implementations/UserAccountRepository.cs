using EMS.APILibrary.Data;
using EMS.APILibrary.Helpers;
using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EMS.APILibrary.Repositories.Implementations;

public class UserAccountRepository:IUserAccount
{
    private readonly JwtSection _jwtSection;
    private readonly AppDbContext _appDbContext;

    public UserAccountRepository(IOptions<JwtSection> jwtSection, AppDbContext appDbContext)
    {
        _jwtSection = jwtSection.Value;
        _appDbContext = appDbContext;
    }

    public async Task<GeneralResponse> CreateAsync(Register? user)
    {
        if (user is null) return new GeneralResponse(false, "Model is empty");

        var checkUser = await FindByEmail(user.Email);
        if (checkUser is not null) return new GeneralResponse(false, "User registered already");

        //save user
        var applicationUser = await AddToDatabase(new ApplicationUser()
        {
            Fullname = user.FullName,
            Email = user.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
        });

        //check,create and assign role
        var checkAdminRole = await _appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.Admin));
        if (checkAdminRole is null)
        {
            var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
            await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
            return new GeneralResponse(true, "Account created");
        }

        var checkUserRole = await _appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Name.Equals(Constants.User));
        SystemRole response = new();
        if (checkUserRole is null)
        {
            response = await AddToDatabase(new SystemRole() { Name = Constants.User });
            await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
        }
        else
        {

            await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
        }
        return new GeneralResponse(true, "Account created");
    }

    


    public Task<LoginResponse> SigninAsync(Login user)
    {
        throw new NotImplementedException();
    }

    private async Task<ApplicationUser?> FindByEmail(string? email) =>
        await _appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email!.ToLower().Equals(email!.ToLower()));

    private async Task<T> AddToDatabase<T>(T model)
    {
        var result = _appDbContext.Add(model!);
        await _appDbContext.SaveChangesAsync();
        return (T)result.Entity;
    }
}