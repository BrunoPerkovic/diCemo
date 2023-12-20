namespace SharedBL.EventBus;

public interface IMessageService
{
    bool Enqueue(string message);
}