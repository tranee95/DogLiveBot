using System.Text.Json;
using DogLiveBot.BL.Services.ServiceInterface.Keyboard;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Extensions;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Models.CommadData;
using DogLiveBot.Data.Models.CommandData;
using DogLiveBot.Data.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace DogLiveBot.BL.Services.ServiceImplementation.Keyboard;

public class KeyboardService : IKeyboardService
{
    
    private JsonSerializerOptions _options { get; }

    public KeyboardService()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public InlineKeyboardMarkup GetMainMenu()
    {
        var menu = new List<InlineKeyboardButton[]>()
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    ButtonText.CreateBooking, CommandTypeEnum.CreateBooking.GetDescription()),
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public InlineKeyboardMarkup GetDeleteDogs(ICollection<DogDeleteDto> dogsModel)
    {
        var menu = new List<InlineKeyboardButton[]>();

        foreach (var item in dogsModel)
        {
            var commandData = new CommandData(item.CommandType, item.Id);
            var jsonData = JsonSerializer.Serialize(commandData);
            menu.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(item.Name, jsonData),
            });
        }

        return new InlineKeyboardMarkup(menu);
    }

    /// <inheritdoc/>
    public InlineKeyboardMarkup GetDays(ICollection<DaysDto> days)
    {
        var menu = new List<InlineKeyboardButton[]>();

        foreach (var day in days)
        {
            var data = new BookingPayload(day.DayOfWeek, null, null);
            var commandData = new CommandData(day.CommandType, data);

            menu.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(day.Text, JsonSerializer.Serialize(commandData, _options)),
            });
        }

        return new InlineKeyboardMarkup(menu);
    }

    /// <inheritdoc/>
    public InlineKeyboardMarkup GetTimes(DayOfWeek dayOfWeek, ICollection<AvailableTimeDto> times)
    {
        var rows = new List<InlineKeyboardButton[]>();

        foreach (var time in times)
        {
            var data = new BookingPayload(dayOfWeek, time.TimeSlotId, null);
            var commandData = new CommandData(CommandTypeEnum.CreateBooking, data);

            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(time.Label, JsonSerializer.Serialize(commandData, _options)),
            });
        }

        return new InlineKeyboardMarkup(rows);
    }
}