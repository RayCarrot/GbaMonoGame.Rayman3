using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public static class EngineConversionExtensions
{
    public static Vector2 ToVector2(this BinarySerializer.Onyx.Gba.Vector2 vector2) => new(vector2.X, vector2.Y);

    public static Rectangle ToRectangle(this EngineBox box) => new(
        x: box.MinX,
        y: box.MinY,
        width: box.MaxX - box.MinX,
        height: box.MaxY - box.MinY);
    
    public static Rectangle ToRectangle(this ChannelBox box) => new(
        x: box.MinX,
        y: box.MinY,
        width: box.MaxX - box.MinX,
        height: box.MaxY - box.MinY);
}