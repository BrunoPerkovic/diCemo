using AuthModule.BL.Models.Tokens;

namespace AuthModule.BL.Models;

public record LoginResponse(AccessTokenModel AccessToken);