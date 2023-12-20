using AuthModule.BL.Models.Tokens;

namespace AuthModule.BL.Models;

public class RegisterResponse
{
    public string Email { get; set; }
    public string Token { get; set; }
    public RefreshToken RefreshToken { get; set; }
}