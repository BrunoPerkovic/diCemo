using AuthModule.BL.Models;
using AuthModule.BL.Services;

namespace AuthModule.BL.Interfaces;

public interface IAuthService
{
    Task<User> Register(UserDto userDto);
    Task<LoginResponse> Login(LoginRequest request);
    Task<User> GetUserById(int id);
}