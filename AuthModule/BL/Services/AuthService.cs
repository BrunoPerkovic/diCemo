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
    private readonly IEmailService _emailService;

    public AuthService(AuthDbContext authContext, IJwtService jwtService, IEmailService emailService)
    {
        _authContext = authContext;
        _jwtService = jwtService;
        _emailService = emailService;
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
            PasswordHash = passwordHash,
            Deleted = false,
            About = ""
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

        var factory = new ConnectionFactory
            { HostName = "localhost", UserName = "user", Password = "password", VirtualHost = "/" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        /*
        channel.QueueDeclare("user.created", true, true, false, null);

        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);
        channel.BasicPublish("", "user.created", null, body: body);
        */

        channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
        const string messagea = "Hello World!";
        var body = Encoding.UTF8.GetBytes(messagea);
        
        channel.BasicPublish(exchange: string.Empty, routingKey: "hello", basicProperties: null, body: body);   
        Console.WriteLine(" [AuthModule] Sent {0}", messagea);
        
        var registerResponse = new RegisterResponse
        {
            Username = user.UserName,
            Token = "dummy change here",
            RefreshToken = "dummy change here"
        };

        _emailService.SendVerificationEmail(user.Email);
        return registerResponse;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Email);
        if (user == null)
        {
            throw new Exception($"Not found user with username: {request.Email}");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new Exception($"Wrong password for user with email: {request.Email}");
        }

        var token = _jwtService.GenerateJwtToken(user);

        return new LoginResponse(token, "dummy change here");
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }
}