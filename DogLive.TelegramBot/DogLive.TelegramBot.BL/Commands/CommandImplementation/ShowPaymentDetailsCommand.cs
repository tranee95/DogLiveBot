using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

public class ShowPaymentDetailsCommand : ICommand
{
    public CommandTypeEnum CommandType => CommandTypeEnum.ShowPaymentDetails;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        throw new NotImplementedException();
    }
}