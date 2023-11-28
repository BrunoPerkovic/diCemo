using AuthModule.BL.Models;
using AuthModule.BL.Services;

namespace AuthModule.BL.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> Register(RegisterRequest userDto);
    Task<LoginResponse> Login(LoginRequest request);
}