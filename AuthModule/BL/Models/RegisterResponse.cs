﻿using AuthModule.BL.Models.Tokens;

namespace AuthModule.BL.Models;

public class RegisterResponse
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}