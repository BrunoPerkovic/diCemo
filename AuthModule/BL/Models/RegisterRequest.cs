namespace AuthModule.BL.Models;

public record RegisterRequest(string Email, string Password, string ConfirmPassword);