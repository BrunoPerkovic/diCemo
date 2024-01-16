namespace AuthModule.BL.Models.Tokens;

public class RefreshToken
{
    public RefreshToken()
    {
        ExpirationDate = CreatedAt.Date.AddMonths(1);
    }

    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate { get; set; }
}