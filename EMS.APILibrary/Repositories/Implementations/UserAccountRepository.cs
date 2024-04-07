using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EMS.APILibrary.Data;
using EMS.APILibrary.Helpers;
using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

    


    public async Task<LoginResponse> SigninAsync(Login? user)
    {
        if (user is null) return new LoginResponse(false, "User is empty");

        var applicationUser = await FindByEmail(user.Email);
        if (applicationUser is  null) return new LoginResponse(false, "User not found");

        if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
            return new LoginResponse(false, "Email or Password is not valid");

        var getUserRole = await FindUserRole(applicationUser.Id);
        if (getUserRole is null) return new LoginResponse(false, "user role not found");

        var getRoleName = await FindRoleName(getUserRole.RoleId);
        if (getRoleName is null) return new LoginResponse(false, "user role not found");

        string jwtToken = GenerateToken(applicationUser, getRoleName.Name);
        string refreshToken = GenerateRefreshToken();

        var findUser = await _appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(rf => rf.UserId == applicationUser.Id);
        if (findUser is not null)
        {
            findUser!.Token = refreshToken;
            await _appDbContext.SaveChangesAsync();
        }
        else
        {
            await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken,UserId = applicationUser.Id});
        }

        return new LoginResponse(true, "Login successfully", jwtToken, refreshToken);
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshToken? token)
    {
        if (token is null) return new LoginResponse(false, "Model is empty");

        var findToken = await _appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(rf => rf.Token!.Equals(token.Token));
        if (findToken is null) return new LoginResponse(false, "Refresh token is required");

        //get user details
        var user = await _appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == findToken.UserId);
        if (user is null) return new LoginResponse(false, "Refresh token could be generated because user not found");

        var userRole = await FindUserRole(user.Id);
        var roleName = await FindRoleName(userRole!.RoleId);
        string jwtToken = GenerateToken(user, roleName!.Name);
        string refreshToken = GenerateRefreshToken();

        var updateRefreshToken = await _appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(rf => rf.UserId == user.Id);
        if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token could not be generated because user has not signed in");

        updateRefreshToken.Token = refreshToken;
        await _appDbContext.SaveChangesAsync();
        return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
    }

    private string GenerateToken(ApplicationUser user, string? role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSection.Key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Fullname!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, role!)
        };
        var token = new JwtSecurityToken(
            issuer:_jwtSection.Issuer,
            audience:_jwtSection.Audience,
            claims:userClaims,
            expires:DateTime.Now.AddDays(1),
            signingCredentials:credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private async Task<UserRole?> FindUserRole(int userId) => await _appDbContext.UserRoles.FirstOrDefaultAsync(r => r.UserId == userId);

    private async Task<SystemRole?> FindRoleName(int roleId) => await _appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Id == roleId);

    private async Task<ApplicationUser?> FindByEmail(string? email) =>
        await _appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email!.ToLower().Equals(email!.ToLower()));

    private async Task<T> AddToDatabase<T>(T model)
    {
        var result = _appDbContext.Add(model!);
        await _appDbContext.SaveChangesAsync();
        return (T)result.Entity;
    }
}