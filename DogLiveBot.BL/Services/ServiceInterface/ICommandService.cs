using Telegram.Bot.Types;

namespace DogLiveBot.BL.Services.ServiceInterface;

public interface ICommandService
{
    /// <summary>
    /// Выполняет команду "Назад", обрабатывая входящее сообщение и, при необходимости, обратный запрос.
    /// </summary>
    /// <param name="message">Сообщение, полученное от пользователя, которое содержит информацию о команде.</param>
    /// <param name="cancellationToken">Токен отмены, который может быть использован для отмены операции.</param>
    /// <param name="callbackQuery">Обратный запрос, если имеется, который может содержать дополнительную информацию о действии пользователя.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    public Task ExecuteGoBackCommand(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null);
}