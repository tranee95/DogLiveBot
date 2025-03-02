using System.ComponentModel;

namespace DogLiveBot.Data.Enums;

public enum CommandTypeEnum
{
    Empty,

    [Description("/start")]
    Start,

    [Description("/registratin")]
    Registration,
    
    [Description("/signUpForClass")]
    SignUpForClass,
    
    [Description("/myNotes")]
    MyNotes,
    
    [Description("/settings")]
    Settings,
    
    [Description("/showPaymentDetails")]
    ShowPaymentDetails,
    
    [Description("/rules")]
    Rules,
    
    [Description("/addDog")]
    AddDog,
    
    [Description("/rename")]
    Rename,
    
    [Description("/back")]
    Back,
    
    [Description("/menu")]
    Menu
}