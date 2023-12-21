using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProfileModule.BL.DataModels;
using ProfileModule.BL.Intefaces;
using ProfileModule.BL.Models;
using ProfileModule.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProfileModule.BL.Services;

public class ProfileService : IProfileService
{
    private readonly IConfiguration _configuration;
    private readonly ProfileDbContext _profileContext;

    public ProfileService(IConfiguration configuration, ProfileDbContext profileContext)
    {
        _configuration = configuration;
        _profileContext = profileContext;
    }

    public async Task<Profile> PostProfile(PostProfileRequest request)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["ConnectionStrings:RabbitMQ:HostName"],
            UserName = _configuration["ConnectionStrings:RabbitMQ:UserName"],
            Password = _configuration["ConnectionStrings:RabbitMQ:Password"],
            Port = int.Parse(_configuration["ConnectionStrings:RabbitMQ:Port"] ?? "5672")
        };
        
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.QueueDeclare("registration", false, false, false, null);
        
        Console.WriteLine($"Profile Module waiting for messages.");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Profile Module received message: {message}");
        };

        var authPayload = JsonSerializer.Serialize(consumer.Model);
        
        var profile = new Profile
        {
            Email = JsonSerializer.Serialize(authPayload), // napravit implementaciju da se mail uzme iz rabbitmq poruke nakon registracije
            PhoneNumber = request.PhoneNumber,
            Address = new Address
            {
                Street = request.Street,
                City = request.City,
                State = request.State,
                Zip = request.Zip,
                Country = request.Country,
                CountryCode = request.CountryCode
            },
            Deleted = false,
            ProfilePicture = request.ProfilePicture,
            About = request.About,
        };

        await _profileContext.Profiles.AddAsync(profile);

        var profileAddress = new Address
        {
            Street = request.Street,
            City = request.City,
            State = request.State,
            Zip = request.Zip,
            Country = request.Country,
        };

        await _profileContext.Addresses.AddAsync(profileAddress);
        await _profileContext.SaveChangesAsync();

        return profile;
    }
    
    public async Task<Profile> GetProfile(string email)
    {
        var profile = await _profileContext.Profiles.FirstOrDefaultAsync(x => x.Email == email);
        if (profile == null)
        {
            throw new Exception("Profile not found");
        }
        return profile;
    }
}