using System.Text;
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
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
        };
        
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.QueueDeclare("hello", false, false, false, null);
        
        Console.WriteLine($"Profile Module waiting for messages.");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (Model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Profile Module received message: {message}");
        };

        channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
        
        var profile = new Profile
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email, // napravit implementaciju da se mail uzme iz rabbitmq poruke nakon registracije
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
}