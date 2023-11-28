using AuthModule.BL.DataModels;

namespace AuthModule.BL.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    int ValidateJwtToken(string token);
    string GenerateRefreshToken();
    string ValidateRefreshToken(string token);
    
}