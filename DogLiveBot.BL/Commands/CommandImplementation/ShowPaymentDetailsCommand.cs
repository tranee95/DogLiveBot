using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class ShowPaymentDetailsCommand : ICommand
{
    public CommandTypeEnum CommandType => CommandTypeEnum.ShowPaymentDetails;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        throw new NotImplementedException();
    }
}