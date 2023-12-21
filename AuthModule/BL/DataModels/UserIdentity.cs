using SharedBL.Database;

namespace AuthModule.BL.DataModels;

public class UserIdentity : PostgresBase
{
    public string Email { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public long RefreshTokenExpires { get; set; }
}