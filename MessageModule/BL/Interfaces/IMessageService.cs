namespace MessageModule.BL.Interfaces;

public interface IMessageService
{
    Task<bool> SendMessageAsync(string recipientNumber);
    
}