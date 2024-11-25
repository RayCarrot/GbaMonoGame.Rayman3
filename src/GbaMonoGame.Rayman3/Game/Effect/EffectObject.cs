using System;
using GbaMonoGame.AnimEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

/// <summary>
/// A custom object base class for emulating GBA effects, such as fading and windows
/// </summary>
public abstract class EffectObject : AObject
{
    protected void DrawLine(Vector2 point1, Vector2 point2, Color color)
    {
        const float thickness = 1;

        float distance = Vector2.Distance(point1, point2);
        float angle = MathF.Atan2(point2.Y - point1.Y, point2.X - point1.X);

        Gfx.AddSprite(new Sprite
        {
            Texture = Gfx.Pixel,
            Position = point1,
            Priority = BgPriority,
            Center = false,
            AffineMatrix = new AffineMatrix(angle, new Vector2(distance, thickness)),
            Color = color,
            Camera = Camera,
        });
    }

    protected void DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
        Gfx.AddSprite(new Sprite
        {
            Texture = Gfx.Pixel,
            Position = position,
            Priority = BgPriority,
            Center = false,
            AffineMatrix = new AffineMatrix(0, size),
            Color = color,
            Camera = Camera,
        });
    }
}