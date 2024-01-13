﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using AuthModule.BL.DataModels;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models.Tokens;
using AuthModule.OptionsSetup;
using AuthModule.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthModule.BL.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly JwtOptions _jwtOptions;
    
    public TokenService(IConfiguration configuration, IOptions<JwtOptions> jwtOptions)
    {
        _configuration = configuration;
        _jwtOptions = jwtOptions.Value;
    }
    
    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }
    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            /*new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),*/
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "User")
        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
            signingCredentials: credentials,
            issuer:  _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(10));
        
        //var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);
        // var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abc123dfg456ghjk789lmn0opqrs1tuv2wxyz3")), SecurityAlgorithms.HmacSha256);
        // var token = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, null, DateTime.Now.AddMinutes(10), signingCredentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(securityToken);
        //var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public AccessTokenModel GenerateJwtAccessToken(User user)
    {
        return new AccessTokenModel
        {
            AccessToken = GenerateJwtToken(user),
            AccessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(5)
                .ToUnixTimeSeconds(),
            RefreshToken = new RefreshToken
            {
                Token = GenerateJwtRefreshToken(user),
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            }
        };
    }

    public string GenerateJwtRefreshToken(User user)
    {
        const string jwtRefreshKey = "keykeykeykeykeykeykeykeykeykeykeykeykeyRefresh";
        var key = Encoding.Default.GetBytes(jwtRefreshKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }


    public bool ValidateJwtToken(string authToken)
    {
        /*if (token is null) throw new Exception($"No token provided");

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

            return userId;
        }
        catch
        {
            throw new Exception($"Provided token is not valid");
        }*/
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        SecurityToken validatedToken;
        IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
        return true;
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        return new RefreshToken
        {
            Token = GenerateJwtToken(user),
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
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
                ValidateIssuer = true,
                ValidateAudience = true,
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
}