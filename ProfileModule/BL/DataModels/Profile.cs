﻿using System.ComponentModel.DataAnnotations;
using SharedBL.Database;

namespace ProfileModule.BL.DataModels;

public class Profile : PostgresBase
{
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public string? About { get; set; } 
    public bool Deleted { get; set; }
    public Address Address { get; set; } = null!;
}