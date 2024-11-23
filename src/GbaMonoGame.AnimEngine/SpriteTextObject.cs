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
    public AffineMatrix? AffineMatrix { get; set; }
    public bool field_0x14 { get; set; } // Unknown, is it unused?

    public int GetStringWidth() => FontManager.GetStringWidth(FontSize, TextBytes);

    public override void Execute(Action<short> soundEventCallback)
    {
        if (TextBytes == null)
            return;

        Vector2 pos = GetAnchoredPosition();

        foreach (byte c in TextBytes)
        {
            // TODO: Option to always draw with the highest resolution font (but scale to fit original size)
            Gfx.AddSprite(FontManager.GetCharacterSprite(c, FontSize, ref pos, BgPriority, AffineMatrix, Color, Camera));
        }
    }
}