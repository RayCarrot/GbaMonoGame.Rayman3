using System;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.Rayman3;

public static class CameraOffset
{
    public static float Multiplayer => 95;

    public static float Default => Engine.Settings.Platform switch
    {
        Platform.GBA => 40,
        Platform.NGage => 25,
        _ => throw new UnsupportedPlatformException()
    };

    public static float DefaultReversed => Engine.GameViewPort.OriginalGameResolution.X - Default;

    public static float DefaultBigger => Engine.Settings.Platform switch
    {
        Platform.GBA => throw new NotImplementedException(),
        Platform.NGage => 45,
        _ => throw new UnsupportedPlatformException()
    };

    public static float Center => Engine.GameViewPort.OriginalGameResolution.X / 2;
}