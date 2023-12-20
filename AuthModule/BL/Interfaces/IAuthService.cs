using AuthModule.BL.Models;
using AuthModule.BL.Models.Tokens;
using AuthModule.BL.Services;

namespace AuthModule.BL.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> Register(RegisterRequest userDto);
    Task<AccessTokenModel> VerifyUser(string verificationCode);
    Task<LoginResponse> Login(LoginRequest request);
}