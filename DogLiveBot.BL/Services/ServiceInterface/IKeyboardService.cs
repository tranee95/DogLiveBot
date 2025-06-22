using DogLiveBot.Data.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DogLiveBot.BL.Services.ServiceInterface;

public interface IKeyboardService
{
    public ReplyKeyboardMarkup GetCompleteShortRegistrationMenu();
    public InlineKeyboardMarkup GetMainMenu();
    public InlineKeyboardMarkup GetSettings();
    public InlineKeyboardMarkup GetDeleteDogs(ICollection<DogDeleteModel> models);
    public InlineKeyboardMarkup GetDays(ICollection<DaysModel> days);
}