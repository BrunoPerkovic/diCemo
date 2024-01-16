using MessageModule.BL.DataModels;

namespace MessageModule.BL.Interfaces;

public interface IWhatsAppService
{
    void SendMessageAsync(string recipientNumber);
    
}