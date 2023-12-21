using AuthModule.BL.Models.Tokens;

namespace AuthModule.BL.Models;

public class RegisterResponse
{
    public string Email { get; set; }
    public string AccessToken { get; set; }
    public long AccessTokenExpires { get; set; }
    public string RefreshToken { get; set; }
}