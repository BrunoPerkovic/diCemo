using System.Security.Claims;
using AuthModule.BL.DataModels;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models.Tokens;
using AuthModule.Config;
using Microsoft.AspNetCore.Mvc;

namespace AuthModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly AuthDbContext _authContext;
    private readonly IAuthService _authService;

    public TokenController(ITokenService tokenService, AuthDbContext authContext, IAuthService authService)
    {
        _tokenService = tokenService;
        _authContext = authContext;
        _authService = authService;
    }

    [HttpPost, Route("refresh")]
    public async Task<IActionResult> Refresh(AccessTokenModel accessTokenModel)
    {
        if (accessTokenModel is null) throw new Exception("Invalid client request");

        var accessToken = accessTokenModel.AccessToken;
        var refreshToken = accessTokenModel.RefreshToken.Token;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        var email = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is null ) throw new Exception("Invalid request. No email claim found");
        
        var userIdentity = await _authService.GetUserIdentityByEmail(email);
        
        if (userIdentity.RefreshToken != refreshToken || userIdentity.RefreshTokenExpirationDate <= DateTime.Now)
        {
            return BadRequest("Invalid client request");
        }

        var user = await _authService.GetUserByEmail(email);
        
        var newAccessToken = _tokenService.GenerateJwtToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user);
        
        userIdentity.RefreshToken = newRefreshToken.Token;
        await _authContext.SaveChangesAsync();
        
        return Ok(new AccessTokenModel
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
    
}