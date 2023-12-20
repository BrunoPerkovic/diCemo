namespace AuthModule.BL.Models.Tokens;

public class RefreshToken
{
    public required string Token { get; set; }
    public long CreatedAt { get; set; }
    public long Expires { get; set; }
}