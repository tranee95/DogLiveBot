using Telegram.Bot.Types.ReplyMarkups;

namespace DogLiveBot.BL.Services.ServiceInterface;

public interface IKeyboardService
{
    public ReplyKeyboardMarkup GetCompleteShortRegistrationMenu();
    public InlineKeyboardMarkup GetMainMenu();
    public InlineKeyboardMarkup GetSettings();
}