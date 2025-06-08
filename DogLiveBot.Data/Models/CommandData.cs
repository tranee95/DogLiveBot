using DogLiveBot.Data.Enums;

namespace DogLiveBot.Data.Models;

public class CommandData
{
    /// <summary>
    /// Идентификатор сущности, для обработки его в рамках выполняемой команды
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Тип команды 
    /// </summary>
    public CommandTypeEnum CommandType { get; set; }
}