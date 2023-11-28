﻿using AuthModule.BL.Interfaces;
using AuthModule.BL.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register", Name = nameof(Register))]
    public async Task<IActionResult> Register(RegisterRequest userDto)
    {
        var user = await _authService.Register(userDto);
        return Ok(user);
    }
    
    [HttpPost("login", Name = nameof(Login))]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.Login(request);
        return Ok(response);
    }
    
}