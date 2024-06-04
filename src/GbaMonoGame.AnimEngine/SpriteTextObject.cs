using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.AnimEngine;

// On GBA setting the position is inlined. X is set using the mask 0xfe00 and Y is set using the mask 0xff00.
public class SpriteTextObject : AObject
{
    private string _text;

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

    public Color Color { get; set; }
    public FontSize FontSize { get; set; }

    public Vector2 ScreenPos { get; set; }
    public AffineMatrix? AffineMatrix { get; set; }

    public int GetStringWidth() => FontManager.GetStringWidth(FontSize, TextBytes);

    public override void Execute(Action<short> soundEventCallback)
    {
        if (TextBytes == null)
            return;

        Vector2 pos = ScreenPos;

        foreach (byte c in TextBytes)
        {
            Gfx.AddSprite(FontManager.GetCharacterSprite(c, FontSize, ref pos, SpritePriority, AffineMatrix, Color, Camera));
        }
    }
}