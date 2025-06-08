using System.Text;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Extensions;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Text;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace DogLiveBot.BL.Services.ServiceImplementation;

public class KeyboardService : IKeyboardService
{
    public ReplyKeyboardMarkup GetCompleteShortRegistrationMenu()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            KeyboardButton.WithRequestContact(ButtonText.SendPhoneNumber)
        })
        {
            ResizeKeyboard = true,
        };
    }

    public InlineKeyboardMarkup GetMainMenu()
    {
        var menu = new List<InlineKeyboardButton[]>()
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.SignUpForClass, CommandTypeEnum.SignUpForClass.GetDescription()),
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.MyNotes, CommandTypeEnum.MyNotes.GetDescription()),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.Settings, CommandTypeEnum.Settings.GetDescription()),
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.Rules, CommandTypeEnum.Rules.GetDescription()),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.ShowPaymentDetails, CommandTypeEnum.ShowPaymentDetails.GetDescription())
            },
        };

        return new InlineKeyboardMarkup(menu);
    }

    public InlineKeyboardMarkup GetSettings()
    {
        var menu = new List<InlineKeyboardButton[]>()
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.AddDog, CommandTypeEnum.AddDog.GetDescription()),
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.DeleteDog, CommandTypeEnum.DeleteDog.GetDescription()),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.Rename, CommandTypeEnum.Rename.GetDescription()),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.Back, CommandTypeEnum.Back.GetDescription())
            },
        };

        return new InlineKeyboardMarkup(menu);
    }

    public InlineKeyboardMarkup GetDeleteDogs(ICollection<DogDeleteModel> dogsModel)
    {
        const int maxByteCount = 64;
        var menu = new List<InlineKeyboardButton[]>();

        foreach (var item in dogsModel)
        {
            var jsonData = JsonConvert.SerializeObject(new { Id = item.Id, CommandType = item.CommandType });

            if (Encoding.UTF8.GetByteCount(jsonData) > maxByteCount)
            {
                throw new Exception("Data size is too big");
            }

            menu.Add(new []
            {
                InlineKeyboardButton.WithCallbackData(item.Name, jsonData),
            });
        }

        return new InlineKeyboardMarkup(menu);
    }
}
