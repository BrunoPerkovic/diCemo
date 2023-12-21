using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthModule.BL.DataModels;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthModule.BL.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtAccessToken(User user)
    {
        const string jwtAccessKey = "keykeykeykeykeykeykeykeykeykeykeykeykeyAccess";
        var key = Encoding.Default.GetBytes(jwtAccessKey);
        // generates token that is valid for 15 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateJwtRefreshToken(User user)
    {
        const string jwtRefreshKey = "keykeykeykeykeykeykeykeykeykeykeykeykeyRefresh";
        var key = Encoding.Default.GetBytes(jwtRefreshKey);
        // generates token that is valid for 15 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(15),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public int ValidateJwtToken(string token)
    {
        if (token is null) throw new Exception($"No token provided");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id")
                .Value);

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            throw new Exception($"Provided token is not valid");
        }
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        return new RefreshToken
        {
            Token = GenerateJwtAccessToken(user),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Expires = DateTimeOffset.UtcNow.AddDays(7)
                .ToUnixTimeSeconds()
        };
    }

    public string ValidateRefreshToken(RefreshToken token)
    {
        if (token is null) throw new Exception($"No token provided");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

        try
        {
            tokenHandler.ValidateToken(token.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "id")
                .Value;

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            throw new Exception($"Provided token is not valid");
        }
    }

    /*public async Task<AccessTokenModel> GenerateAccessToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        var refreshToken = GenerateRefreshToken();
        return new AccessTokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }*/

    /*public async Task<TemporaryTokenModel> GenerateTemporaryToken(User user)
    {
        var token = GenerateJwtToken(user);
        var expires = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
        var temporaryToken = new TemporaryTokenModel
        {
            Expires = expires,
            Token = token
        };

        return temporaryToken;
    }*/
}