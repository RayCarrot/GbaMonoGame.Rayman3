using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.Rayman3;

public enum GameResource
{
    [GameResourceDefine(Game.Rayman3, Platform.GBA, 70)]
    [GameResourceDefine(Game.Rayman3, Platform.NGage, 79)]
    HudAnimations,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 91)]
    [GameResourceDefine(Game.Rayman3, Platform.NGage, 103)]
    MenuPlayfield,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 92)]
    [GameResourceDefine(Game.Rayman3, Platform.NGage, 105)]
    MenuPropAnimations,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 93)]
    //[GameResourceDefine(Game.Rayman3, Platform.NGage, )]
    MenuStartEraseAnimations,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 96)]
    //[GameResourceDefine(Game.Rayman3, Platform.NGage, )]
    MenuGameLogoAnimations,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 97)]
    //[GameResourceDefine(Game.Rayman3, Platform.NGage, )]
    MenuGameModeAnimations,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 98)]
    //[GameResourceDefine(Game.Rayman3, Platform.NGage, )]
    MenuLanguageListAnimations,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 105)]
    //[GameResourceDefine(Game.Rayman3, Platform.NGage, )]
    MenuSteamAnimations,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 117)]
    [GameResourceDefine(Game.Rayman3, Platform.NGage, 131)]
    IntroPlayfield,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 118)]
    [GameResourceDefine(Game.Rayman3, Platform.NGage, 132)]
    IntroAnimations,
}