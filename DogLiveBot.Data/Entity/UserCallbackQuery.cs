namespace DogLiveBot.Data.Entity;

public class UserCallbackQuery : BaseEntity<Guid>
{
    public string CallbackQueryId { get; set; }

    public long UserTelegramId { get; set; }

    public string Data { get; set; }
    
    public long  ChatId { get; set; }
}