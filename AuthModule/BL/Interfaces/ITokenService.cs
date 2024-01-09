using AuthModule.BL.DataModels;
using AuthModule.BL.Models.Tokens;

namespace AuthModule.BL.Interfaces;

public interface ITokenService
{
    string GenerateJwtAccessToken(User user);
    string GenerateJwtRefreshToken(User user);
    int ValidateJwtToken(string token);
    RefreshToken GenerateRefreshToken(User user);
    string ValidateRefreshToken(RefreshToken token);
}