using System.ComponentModel;

namespace DogLiveBot.Data.Enums;

public enum CommandTypeEnum
{
    [Description("/start")]
    Start,

    [Description("/registratin")]
    Registration
}