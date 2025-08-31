using DogLive.TelegramBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandInterface;

public interface IReceivedTextCommand
{
    public CommandTypeEnum CommandType { get; }
    public CommandTypeEnum BackCommandType { get; }
    public Task ExecuteTextCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}