﻿using AuthModule.BL.DataModels;
using AuthModule.BL.Models;
using AuthModule.BL.Models.Tokens;
using AuthModule.BL.Services;

namespace AuthModule.BL.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> Register(RegisterRequest userDto);
    Task<AccessTokenModel> VerifyUser(string email, string verificationCode);
    Task<LoginResponse> Login(LoginRequest request);
    Task<User> GetUserByEmail(string email);
    Task<UserIdentity> GetUserIdentityByEmail(string email);
    string GetMyEmail();
}