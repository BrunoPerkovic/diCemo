using AuthModule.BL.DataModels;
using AuthModule.BL.Models.Tokens;

namespace AuthModule.BL.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    int ValidateJwtToken(string token);
    RefreshToken GenerateRefreshToken();
    string ValidateRefreshToken(RefreshToken token);
    Task<AccessTokenModel> GenerateAccessToken();
    Task<TemporaryTokenModel> GenerateTemporaryToken(User user);
}