﻿using SharedBL.Profile;

namespace AuthModule.BL.Models.Tokens;

public class AccessTokenModel
{
    public string AccessToken { get; set; }
    public long AccessTokenExpires { get; set; }
    public RefreshToken RefreshToken { get; set; }
    public long RefreshTokenExpires { get; set; }
    public Dictionary<string, string[]> Permissions { get; set; }
    public AccessContext Context { get; set; }
    public string ClientId { get; set; }
    public string RedirectUrl { get; set; }
}

public class AccessContext
{
    public SubjectType Type { get; set; }
    public string Id { get; set; }
}