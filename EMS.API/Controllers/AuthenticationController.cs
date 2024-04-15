﻿using EMS.APILibrary.Repositories.Contracts;
using EMS.BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IUserAccount _account;

    public AuthenticationController(IUserAccount account)
    {
        _account = account;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAsync(Register? user)
    {
        if (user is null) return BadRequest("Model is empty");
        var result = await _account.CreateAsync(user);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> SignInAsync(Login? user)
    {
        if (user is null) return BadRequest("Model is empty");
        var result = await _account.SigninAsync(user);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshToken? token)
    {
        if(token is null) return BadRequest("Model is empty");
        var result = await _account.RefreshTokenAsync(token);
        return Ok(result);
    }

}