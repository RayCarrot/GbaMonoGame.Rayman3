using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.AnimEngine;

// On GBA setting the position is inlined. X is set using the mask 0xfe00 and Y is set using the mask 0xff00.
public class SpriteTextObject : AObject
{
    private string _text;

    // The game has a global variable for the current color. But maybe it'd be better if we did it per object instead?
    public static Color Color { get; set; }

    private byte[] TextBytes { get; set; }

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            TextBytes = FontManager.GetTextBytes(value);
        }
    }

    public FontSize FontSize { get; set; }

    public Vector2 ScreenPos { get; set; }
    public AffineMatrix? AffineMatrix { get; set; }

    public int GetStringWidth() => FontManager.GetStringWidth(FontSize, TextBytes);

    public override void Execute(AnimationSpriteManager animationSpriteManager, Action<ushort> soundEventCallback)
    {
        if (TextBytes == null)
            return;

        Vector2 pos = ScreenPos;

        foreach (byte c in TextBytes)
        {
            Gfx.AddSprite(FontManager.GetCharacterSprite(c, FontSize, ref pos, SpritePriority, AffineMatrix, Color, Engine.ScreenCamera));
        }
    }
}