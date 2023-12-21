using System.Text;
using System.Text.Json;
using AuthModule.BL.DataModels;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models;
using AuthModule.BL.Models.Tokens;
using AuthModule.Config;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using SharedBL.Cache;
using SharedBL.Messaging;


namespace AuthModule.BL.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _authContext;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;

    private bool UniqueEmail(string email)
    {
        return _authContext.Users.FirstOrDefault(u => u.Email == email) == null;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }


    private bool IsVerificationCodeValid(string userEmail, string userProvidedVerificationCode)
    {
        var stored = RetrieveStoredVerificationCode(userEmail);
        return stored == userProvidedVerificationCode;
    }

    private string RetrieveStoredVerificationCode(string email)
    {
        var user = _authContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            throw new Exception($"Not found user with username: {email}");
        }

        return user.VerificationCode;
    }

    public AuthService(AuthDbContext authContext,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration configuration,
        ICacheService cacheService)
    {
        _authContext = authContext;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
        _cacheService = cacheService;
    }

    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
            };

            _authContext.Users.Add(user);
            await _authContext.SaveChangesAsync();

            var message = new UserCreatedMessage
            {
                Id = user.Id,
                Email = user.Email
            };

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "registration", durable: false, exclusive: false, autoDelete: false,
                arguments: null);
            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish(exchange: string.Empty, routingKey: "registration", basicProperties: null, body: body);
            Console.WriteLine(" [AuthModule] Sent {0}", message);

            var registerResponse = new RegisterResponse
            {
                Email = user.Email,
                AccessToken = _tokenService.GenerateJwtAccessToken(user),
                AccessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(7)
                    .ToUnixTimeSeconds(),
                RefreshToken = _tokenService.GenerateJwtRefreshToken(user),
            };
            
            _emailService.SendVerificationEmail(user.Email);
            return registerResponse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<AccessTokenModel> VerifyUser(User user, string verificationCodeProvided)
    {
        if (!IsVerificationCodeValid(user.Email, verificationCodeProvided))
        {
            throw new Exception($"Wrong verification code for user with email: {user.Email}");
        }

        var token = _tokenService.GenerateJwtAccessToken(user);
        var refreshToken = _tokenService.GenerateJwtRefreshToken(user);
        
        return new AccessTokenModel
        {
            AccessToken = token,
            AccessTokenExpires = DateTimeOffset.UtcNow.AddMinutes(7)
                .ToUnixTimeSeconds(),
            RefreshToken = new RefreshToken
            {
                Token = refreshToken,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Expires = DateTimeOffset.UtcNow.AddDays(7)
                    .ToUnixTimeSeconds(),
            },
        };
    }
    
    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            throw new Exception($"Not found user with username: {request.Email}");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new Exception($"Wrong password for user with email: {request.Email}");
        }

        var token = _tokenService.GenerateJwtAccessToken(user);

        return new LoginResponse(token, "dummy change here");
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }
}


