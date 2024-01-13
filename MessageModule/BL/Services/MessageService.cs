using MessageModule.BL.Interfaces;

namespace MessageModule.BL.Services;

public class MessageService : IMessageService
{
    public async Task<bool> SendMessageAsync(string recipientNumber)
    {
        throw new NotImplementedException();
    }
}