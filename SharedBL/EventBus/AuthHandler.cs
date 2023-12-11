/*using Microsoft.Extensions.DependencyInjection;
using SharedBL.Messaging;

namespace SharedBL.EventBus;

public class AuthHandler
{
    private readonly IServiceProvider _serviceProvider;

    public AuthHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(UserCreatedMessage message, IMessageHandlerContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        await emailService.SendVerificationEmail(message.UserName);
    }A
}*/