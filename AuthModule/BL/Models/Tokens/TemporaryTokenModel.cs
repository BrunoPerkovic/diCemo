namespace AuthModule.BL.Models.Tokens;

public class TemporaryTokenModel
{
    public string Token { get; set; }
    public long Expires { get; set; }
}