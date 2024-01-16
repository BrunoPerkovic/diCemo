using System.Security.Claims;
using AuthModule.BL.DataModels;
using AuthModule.BL.Models.Tokens;

namespace AuthModule.BL.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    AccessTokenModel GenerateJwtAccessToken(User user);
    string GenerateJwtRefreshToken(User user);
    bool ValidateJwtToken(string token);
    RefreshToken GenerateRefreshToken(User user);
    string ValidateRefreshToken(RefreshToken token);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
}