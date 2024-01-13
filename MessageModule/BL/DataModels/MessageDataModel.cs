using SharedBL.Database;

namespace MessageModule.BL.DataModels;

public class MessageDataModel : PostgresBase
{
    public string SenderNumber { get; set; }
    public string RecipientNumber { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsRead { get; set; }
    public bool IsReceived { get; set; }
    public bool IsSent { get; set; }
}