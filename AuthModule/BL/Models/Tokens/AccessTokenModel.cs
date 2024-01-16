using SharedBL.Profile;

namespace AuthModule.BL.Models.Tokens;

public class AccessTokenModel
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpires { get; set; }
    public RefreshToken RefreshToken { get; set; }
    //public Dictionary<string, string[]> Permissions { get; set; }
}