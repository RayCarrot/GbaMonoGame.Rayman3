using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.Rayman3;

public enum GameResource
{
    [GameResourceDefine(Game.Rayman3, Platform.GBA, 117)]
    [GameResourceDefine(Game.Rayman3, Platform.NGage, 131)]
    IntroPlayfield,

    [GameResourceDefine(Game.Rayman3, Platform.GBA, 118)]
    [GameResourceDefine(Game.Rayman3, Platform.NGage, 132)]
    IntroAnimations,
}