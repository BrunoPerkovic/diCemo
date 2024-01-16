using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AuthModule.BL.DataModels;
using AuthModule.BL.Interfaces;
using AuthModule.BL.Models;
using AuthModule.BL.Models.Tokens;
using AuthModule.Config;
using MessageModule.BL.Interfaces;
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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWhatsAppService _whatsAppService;

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

        var storedKey = _cacheService.Get($"{email}.verificationCode");
        return storedKey;
    }

    public AuthService(AuthDbContext authContext,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration configuration,
        ICacheService cacheService,
        IHttpContextAccessor httpContextAccessor,
        IWhatsAppService whatsAppService)
    {
        _authContext = authContext;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
        _cacheService = cacheService;
        _httpContextAccessor = httpContextAccessor;
        _whatsAppService = whatsAppService;
    }

    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        try
        {
            if (!IsValidEmail(request.Email))
            {
                throw new Exception($"Invalid email format: {request.Email}");
            }

            if (!UniqueEmail(request.Email))
            {
                throw new Exception(
                    $"User already registered with: {request.Email} . Please try registering with another email or login with existing one.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PhoneNumber = request.PhoneNumber
            };

            _authContext.Users.Add(user);
            await _authContext.SaveChangesAsync();

            var message = new UserCreatedMessage
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
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
                PhoneNumber = user.PhoneNumber
            };

            //var email = _emailService.SendVerificationEmail(user.Email);
            var whappMessage = await _whatsAppService.SendMessageAsync(user.PhoneNumber);
            var dummyCodeFromEmail = "12345678";
            _cacheService.Set($"{user.Email}.verificationCode", whappMessage.Message, TimeSpan.FromMinutes(10));
            return registerResponse;
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<AccessTokenModel> VerifyUser(string email, string verificationCodeProvided)
    {
        if (!IsVerificationCodeValid(email, verificationCodeProvided))
        {
            throw new Exception($"Wrong verification code for user with email: {email}");
        }
        
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        if (user == null)
        {
            throw new Exception($"Not found user with username: {email}");
        }

        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateJwtRefreshToken(user);

        user.Verified = true;

        var currentTimeUtc = DateTime.UtcNow;
        
        var userIdentity = new UserIdentity
        {
            AccessToken = token,
            AccessTokenCreatedAt = currentTimeUtc,
            AccessTokenExpirationDate = currentTimeUtc.AddMinutes(10),
            Email = email,
            Id = user.Id,
            RefreshToken = refreshToken,
            RefreshTokenCreatedAt = currentTimeUtc,
            RefreshTokenExpirationDate = currentTimeUtc.AddMonths(3),
            User = user
        };

        
        _authContext.UserIdentities.Add(userIdentity);
        await _authContext.SaveChangesAsync();
    
        return new AccessTokenModel
        {
            AccessToken = token,
            AccessTokenExpires = DateTime.Now.AddMinutes(15),
            RefreshToken = new RefreshToken
            {
                Token = refreshToken,
                CreatedAt = DateTime.Now,
            },
        };
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            throw new Exception($"User with email: {request.Email} not found");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new Exception($"Wrong password for user with email: {request.Email}");
        }

        var accessToken = _tokenService.GenerateJwtAccessToken(user);

        var isValidToken = _tokenService.ValidateJwtToken(accessToken.AccessToken);
        if (!isValidToken)
        {
            throw new Exception($"Invalid token for user with email: {request.Email}");
        }

        return new LoginResponse(accessToken);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new Exception($"Not found user with username: {email}");
        }
        return user;
    }

    public async Task<UserIdentity> GetUserIdentityByEmail(string email)
    {
        var userIdentity = await _authContext.UserIdentities.FirstOrDefaultAsync(u => u.Email == email);
        if (userIdentity == null)
        {
            throw new Exception($"Not found user with username: {email}");
        }
        return userIdentity;
    }

    public string GetMyEmail()
    {
        if (_httpContextAccessor.HttpContext is null) throw new Exception("HttpContext is null");
        var result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        return result;
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new Exception($"Not found user with id: {id}");
        }

        return user;
    }
}