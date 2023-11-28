using AuthModule.BL.DataModels;

namespace AuthModule.BL.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    string ValidateJwtToken(string token);
}