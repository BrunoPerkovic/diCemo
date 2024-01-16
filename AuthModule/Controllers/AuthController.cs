using AuthModule.BL.DataModels;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        if (registerRequest is null) throw new Exception("Invalid request");
        
        var user = await _authService.Register(registerRequest);
        return Ok(user);
    }

    [HttpPut("verify", Name = nameof(Verify))]
    public async Task<IActionResult> Verify(string email, string verificationCode)
    {
        var verify = await _authService.VerifyUser(email, verificationCode);
        return Ok(verify);
    }
    
    [HttpPost("login", Name = nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request is null) throw new Exception("Invalid request");
        
        var response = await _authService.Login(request);
        return Ok(response);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("user", Name = nameof(GetUserByEmail))]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _authService.GetUserByEmail(email);
        return Ok(user);
    }
    
}