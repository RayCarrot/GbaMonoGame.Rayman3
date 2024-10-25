using GbaMonoGame.AnimEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

/// <summary>
/// A custom object base class for emulating GBA effects, such as fading and windows
/// </summary>
public abstract class EffectObject : AObject
{
    protected void DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
        Gfx.AddSprite(new Sprite
        {
            Texture = Gfx.Pixel,
            Position = position,
            Priority = SpritePriority,
            Center = false,
            AffineMatrix = new AffineMatrix(0, size),
            Color = color,
            Camera = Camera,
        });
    }
}