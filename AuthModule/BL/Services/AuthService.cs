using System.Text;
using System.Text.Json;
using AuthModule.BL.DataModels;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models;
using AuthModule.Config;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using SharedBL.Messaging;


namespace AuthModule.BL.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _authContext;
    private readonly IJwtService _jwtService;
    
    public AuthService(AuthDbContext authContext, IJwtService jwtService)
    {
        _authContext = authContext;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
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

        var registerResponse = new RegisterResponse
        {
            Username = user.UserName,
            Token = "dummy change here",
            RefreshToken = "dummy change here"
        };
        return registerResponse;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user == null)
        {
            throw new Exception($"Not found user with username: {request.UserName}");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return null;
        }

        var token = await _jwtService.GenerateToken(user);

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
        return user;
    }
}