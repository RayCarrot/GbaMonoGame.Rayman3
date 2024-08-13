using System;

namespace GbaMonoGame.AnimEngine;

// On GBA setting the priority is inlined. Sprite is set using the mask 0x3fff and Y is set using the mask 0xc0ff.
public abstract class AObject
{
    public byte Priority { get; private set; }

    public int SpritePriority
    {
        get => Priority >> 6;
        set
        {
            Priority &= 0x3F;
            Priority |= (byte)((value & 0x3) << 6);
        }
    }

    public int YPriority
    {
        get => Priority & 0x3F;
        set
        {
            Priority &= 0xC0;
            Priority |= (byte)(value & 0x3F);
        }
    }

    // This isn't in the base class in the original game, but easier to manage things this way
    public Vector2 ScreenPos { get; set; }

    // Custom properties to have position scale with camera resolution
    public HorizontalAnchorMode HorizontalAnchor { get; set; }
    public VerticalAnchorMode VerticalAnchor { get; set; }

    public GfxCamera Camera { get; set; } = Engine.ScreenCamera;

    public Vector2 GetAnchoredPosition()
    {
        Vector2 pos = ScreenPos;

        switch (HorizontalAnchor)
        {
            default:
            case HorizontalAnchorMode.Left:
                // Do nothing
                break;

            case HorizontalAnchorMode.Center:
                pos.X += Camera.Resolution.X / 2;
                break;
            
            case HorizontalAnchorMode.Right:
                pos.X += Camera.Resolution.X;
                break;
            
            case HorizontalAnchorMode.Scale:
                pos.X += (Camera.Resolution.X - Engine.GameViewPort.OriginalGameResolution.X) / 2;
                break;
        }

        switch (VerticalAnchor)
        {
            default:
            case VerticalAnchorMode.Top:
                // Do nothing
                break;

            case VerticalAnchorMode.Center:
                pos.Y += Camera.Resolution.Y / 2;
                break;

            case VerticalAnchorMode.Bottom:
                pos.Y += Camera.Resolution.Y;
                break;

            case VerticalAnchorMode.Scale:
                pos.Y += (Camera.Resolution.Y - Engine.GameViewPort.OriginalGameResolution.Y) / 2;
                break;
        }

        return pos;
    }

    public abstract void Execute(Action<short> soundEventCallback);
}