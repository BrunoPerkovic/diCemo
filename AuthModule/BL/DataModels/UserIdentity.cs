using AuthModule.BL.Models.Tokens;
using SharedBL.Database;

namespace AuthModule.BL.DataModels;

public class UserIdentity : PostgresBase
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenCreatedAt { get; set; }
    public DateTime AccessTokenCreatedAt { get; set; }
    public DateTime RefreshTokenExpirationDate { get; set; }
    public DateTime AccessTokenExpirationDate { get; set; }
    
    public string Email { get; set; }
    public User User { get; set; } = null!;
}