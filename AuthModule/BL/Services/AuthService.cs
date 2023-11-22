using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models;
using AuthModule.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using SharedBL.Database;
using SharedBL.Messaging;

namespace AuthModule.BL.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _authContext;

    private string GenerateJwtToken(User user)
    {
        const string jwtkey = "keykeykeykeykeykeykeykeykeykeykeykeykey";
        var key = Encoding.Default.GetBytes(jwtkey);
        // generates token that is valid for 15 minutes
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

    public AuthService(AuthDbContext authContext)
    {
        _authContext = authContext;
    }

    public async Task<User> Register(UserDto request)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PasswordHash = passwordHash
        };

        _authContext.Users.Add(user);
        await _authContext.SaveChangesAsync();

        var message = new UserCreatedMessage
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName
        };

        //var connection = _rabbitMq.GetConnection();
        // var channel = _rabbitMq.GetModel(connection);
        var factory = new ConnectionFactory
            { HostName = "localhost", UserName = "user", Password = "password", VirtualHost = "/" };
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("user.created", true, true, false, null);

        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);
        channel.BasicPublish("", "user.created", null, body: body);

        return user;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user == null)
        {
            throw new EntityNotFoundException($"User with username: {request.UserName} not found in the database");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new Exception("Invalid password");
        }

        var token = GenerateJwtToken(user);
        return new LoginResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Token = token
        };
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new EntityNotFoundException($"User with id: {id} not found in the database");
        }
        return user;
    }
}