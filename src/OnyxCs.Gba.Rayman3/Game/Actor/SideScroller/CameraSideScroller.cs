using System;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class CameraSideScroller : CameraActor2D
{
    public CameraSideScroller()
    {
        HorOffset = MultiplayerManager.IsInMultiplayer ? 95 : 40;
    }

    public int HorOffset { get; set; }
    public Vector2 PreviousLinkedObjectPosition { get; set; }

    protected override bool ProcessMessage(Message message, object param)
    {
        return base.ProcessMessage(message, param);
    }

    public override void SetFirstPosition()
    {
        TgxCamera2D tgxCamera = Frame.GetComponent<TgxCamera2D>();

        Vector2 pos;

        if (LinkedObject.Position.X < HorOffset && !LinkedObject.IsFacingLeft)
        {
            pos = new Vector2(0, LinkedObject.Position.Y);
        }
        else if (LinkedObject.Position.X < (Gfx.GfxCamera.GameResolution.X - HorOffset) && LinkedObject.IsFacingLeft)
        {
            pos = new Vector2(0, LinkedObject.Position.Y);
        }
        else
        {
            if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
            {
                HorOffset = 120;
                pos = new Vector2(LinkedObject.Position.X - HorOffset, LinkedObject.Position.Y);
            }
            else
            {
                if (!LinkedObject.IsFacingLeft)
                {
                    pos = new Vector2(LinkedObject.Position.X - HorOffset, LinkedObject.Position.Y);
                }
                else
                {
                    pos = new Vector2(LinkedObject.Position.X + HorOffset - Gfx.GfxCamera.GameResolution.X, LinkedObject.Position.Y);
                }
            }
        }

        pos.Y = Math.Max(pos.Y - 120, 0);

        tgxCamera.Position = pos;
        PreviousLinkedObjectPosition = LinkedObject.Position;
    }
}