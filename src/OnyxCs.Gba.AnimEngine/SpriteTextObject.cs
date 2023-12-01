using System;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.AnimEngine;

public class SpriteTextObject : AObject
{
    // The game has a global variable for the current color. But maybe it'd be better if we did it per object instead?
    public static Color Color { get; set; }

    public string Text { get; set; }
    public FontSize FontSize { get; set; }

    public Vector2 ScreenPos { get; set; }
    public AffineMatrix? AffineMatrix { get; set; }

    public override void Execute(AnimationSpriteManager animationSpriteManager, Action<ushort> soundEventCallback)
    {
        Vector2 pos = ScreenPos;

        foreach (char c in Text)
        {
            Gfx.AddSprite(FontManager.GetCharacterSprite(c, FontSize, ref pos, SpritePriority, AffineMatrix, Color));
        }
    }
}