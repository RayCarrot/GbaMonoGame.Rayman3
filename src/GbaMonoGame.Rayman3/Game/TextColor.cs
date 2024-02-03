using BinarySerializer;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public static class TextColor
{
    public static Color Story { get; } = new RGB555Color(0x8aa).ToColor();
    public static Color Menu { get; } = new RGB555Color(0x2fd).ToColor();
    public static Color GameCubeMenu { get; } = new RGB555Color(0xe1f).ToColor();
    public static Color GameCubeMenuFaded { get; } = new RGB555Color(0x553).ToColor();
}