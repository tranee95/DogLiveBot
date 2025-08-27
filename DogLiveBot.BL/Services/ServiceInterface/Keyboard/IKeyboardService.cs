using DogLiveBot.Data.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace DogLiveBot.BL.Services.ServiceInterface.Keyboard;

/// <summary>
/// Интерфейс для сервиса клавиатуры, предоставляющий методы для создания различных меню.
/// </summary>
public interface IKeyboardService
{
    /// <summary>
    /// Получает клавиатуру для завершенной короткой регистрации.
    /// </summary>
    /// <returns>Объект <see cref="ReplyKeyboardMarkup"/>, представляющий клавиатуру для короткой регистрации.</returns>
    ReplyKeyboardMarkup GetCompleteShortRegistrationMenu();

    /// <summary>
    /// Получает основное меню в виде встроенной клавиатуры.
    /// </summary>
    /// <returns>Объект <see cref="InlineKeyboardMarkup"/>, представляющий основное меню.</returns>
    InlineKeyboardMarkup GetMainMenu();

    /// <summary>
    /// Получает клавиатуру для настроек пользователя.
    /// </summary>
    /// <returns>Объект <see cref="InlineKeyboardMarkup"/>, представляющий меню настроек.</returns>
    InlineKeyboardMarkup GetSettings();

    /// <summary>
    /// Получает клавиатуру для удаления собак на основе предоставленных моделей.
    /// </summary>
    /// <param name="models">Коллекция объектов <see cref="DogDto"/>, представляющих собак для удаления.</param>
    /// <returns>Объект <see cref="InlineKeyboardMarkup"/>, представляющий клавиатуру для удаления собак.</returns>
    InlineKeyboardMarkup GetDeleteDogs(ICollection<DogDto> models);

    /// <summary>
    /// Получает клавиатуру для выбора дней.
    /// </summary>
    /// <param name="days">Коллекция объектов <see cref="DaysDto"/>, представляющих доступные дни.</param>
    /// <returns>Объект <see cref="InlineKeyboardMarkup"/>, представляющий клавиатуру для выбора дней.</returns>
    InlineKeyboardMarkup GetDays(ICollection<DaysDto> days);

    /// <summary>
    /// Получает клавиатуру для выбора времени на определенный день недели.
    /// </summary>
    /// <param name="dayOfWeek">День недели для выбора времени.</param>
    /// <param name="times">Коллекция объектов <see cref="AvailableTimeDto"/>, представляющих доступное время.</param>
    /// <returns>Объект <see cref="InlineKeyboardMarkup"/>, представляющий клавиатуру для выбора времени.</returns>
    InlineKeyboardMarkup GetTimes(DayOfWeek dayOfWeek, ICollection<AvailableTimeDto> times);

    InlineKeyboardMarkup GetDogs(DayOfWeek dayOfWeek, int timeSlotId, ICollection<DogDto> dogs);
}