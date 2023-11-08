using System;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class CameraSideScroller : CameraActor2D
{
    public CameraSideScroller()
    {
        if (!MultiplayerManager.IsInMultiplayer)
        {
            HorizontalOffset = Gfx.Platform switch
            {
                Platform.GBA => 40,
                Platform.NGage => 25,
                _ => throw new UnsupportedPlatformException()
            };
        }
        else
        {
            HorizontalOffset = 95;
        }
    }

    public int HorizontalOffset { get; set; }
    public Vector2 PreviousLinkedObjectPosition { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        return base.ProcessMessageImpl(message, param);
    }

    public override void SetFirstPosition()
    {
        TgxCamera2D tgxCamera = Frame.GetComponent<TgxCamera2D>();

        Vector2 pos;

        if (LinkedObject.Position.X < HorizontalOffset && !LinkedObject.IsFacingLeft)
        {
            pos = new Vector2(0, LinkedObject.Position.Y);
        }
        else if (LinkedObject.Position.X < (Gfx.GfxCamera.GameResolution.X - HorizontalOffset) && LinkedObject.IsFacingLeft)
        {
            pos = new Vector2(0, LinkedObject.Position.Y);
        }
        else
        {
            if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
            {
                HorizontalOffset = Gfx.Platform switch
                {
                    Platform.GBA => 120,
                    Platform.NGage => 88,
                    _ => throw new UnsupportedPlatformException()
                };
                pos = new Vector2(LinkedObject.Position.X - HorizontalOffset, LinkedObject.Position.Y);
            }
            else
            {
                if (!LinkedObject.IsFacingLeft)
                {
                    pos = new Vector2(LinkedObject.Position.X - HorizontalOffset, LinkedObject.Position.Y);
                }
                else
                {
                    pos = new Vector2(LinkedObject.Position.X + HorizontalOffset - Gfx.GfxCamera.GameResolution.X, LinkedObject.Position.Y);
                }
            }
        }

        pos.Y = Math.Max(pos.Y - 120, 0);

        tgxCamera.Position = pos;
        PreviousLinkedObjectPosition = LinkedObject.Position;
    }
}