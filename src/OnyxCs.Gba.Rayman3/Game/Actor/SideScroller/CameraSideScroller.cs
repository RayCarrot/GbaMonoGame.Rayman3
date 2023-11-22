using System;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class CameraSideScroller : CameraActor2D
{
    public CameraSideScroller()
    {
        Fsm.ChangeAction(Fsm_Default);

        field16_0x2d = 120;
        field21_0x32 = 1;

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

    public byte HorizontalOffset { get; set; }
    public byte TargetX { get; set; }
    public Vector2 PreviousLinkedObjectPosition { get; set; }
    public bool IsFacingRight { get; set; }
    public uint Timer { get; set; }
    public Vector2 Speed { get; set; }

    // Unknown
    public byte field16_0x2d { get; set; }
    public byte field21_0x32 { get; set; }

    private void UpdateTargetX()
    {
        if (LinkedObject.IsFacingLeft)
            TargetX = Gfx.Platform switch
            {
                Platform.GBA => (byte)(-HorizontalOffset - 16),
                Platform.NGage => (byte)(-80 - HorizontalOffset),
                _ => throw new UnsupportedPlatformException()
            };
        else
            TargetX = HorizontalOffset;
    }

    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (LinkedObject != null)
                {
                    PreviousLinkedObjectPosition = LinkedObject.Position;
                    IsFacingRight = LinkedObject.IsFacingRight;

                    UpdateTargetX();
                    Speed = new Vector2(TargetX < LinkedObject.ScreenPosition.X ? 1 : -1, Speed.Y);
                }

                Timer = 0;
                break;

            case FsmAction.Step:
                // Reset speed y
                Speed = new Vector2(Speed.X, 0);
                
                UpdateTargetX();

                float linkedObjDeltaX = LinkedObject.Position.X - PreviousLinkedObjectPosition.X;

                // If we're within 4 pixels of the target...
                if (Math.Abs(LinkedObject.ScreenPosition.X - TargetX) <= 4)
                {
                    // Follow the linked object's movement
                    Speed = new Vector2(linkedObjDeltaX, Speed.Y);
                }
                // If far away from the target...
                else
                {
                    Timer++;

                    // Reset speed x if we're switching direction to move
                    if ((LinkedObject.ScreenPosition.X < TargetX && Speed.X > 0) ||
                        (LinkedObject.ScreenPosition.X > TargetX && Speed.X < 0))
                    {
                        Speed = new Vector2(0, Speed.Y);
                    }

                    float dir = LinkedObject.ScreenPosition.X > TargetX ? 0.5f : -0.5f;

                    if (Math.Abs(linkedObjDeltaX) > 2)
                    {
                        Speed = new Vector2(12 * dir, Speed.Y);
                    }
                    else if (LinkedObject.Speed.X == 0 || Timer < 3 || Math.Abs(Speed.X) >= 4)
                    {
                        if (LinkedObject.Speed.X == 0)
                        {
                            if ((LinkedObject.IsFacingRight && 
                                HorizontalOffset + 40 > LinkedObject.ScreenPosition.X &&
                                HorizontalOffset <= LinkedObject.ScreenPosition.X) ||
                                (LinkedObject.IsFacingLeft &&
                                 Gfx.GfxCamera.GameResolution.X - 40 - HorizontalOffset < LinkedObject.ScreenPosition.X &&
                                 Gfx.GfxCamera.GameResolution.X - HorizontalOffset > LinkedObject.ScreenPosition.X))
                            {
                                if (Timer > 2 && Math.Abs(Speed.X) > 1)
                                {
                                    Speed = new Vector2(Speed.X - dir, Speed.Y);
                                    Timer = 0;
                                }
                            }
                            else
                            {
                                if (Timer > 5 && Math.Abs(Speed.X) < 4)
                                {
                                    Speed = new Vector2(Speed.X + dir, Speed.Y);
                                    Timer = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        Speed = new Vector2(Speed.X + dir, Speed.Y);
                        Timer = 0;
                    }
                }

                // TODO: Implement

                TgxCamera2D cam = Frame.GetComponent<TgxPlayfield2D>().Camera;
                TgxCluster mainCluster = cam.GetMainCluster();

                cam.Position += Speed;
                
                PreviousLinkedObjectPosition = LinkedObject.Position;

                // Reset if changed direction
                if (!mainCluster.IsOnLimit(Edge.Left) && 
                    !mainCluster.IsOnLimit(Edge.Right) && 
                    LinkedObject.IsFacingRight != IsFacingRight)
                    Fsm.ChangeAction(Fsm_Default);

                // TODO: Implement

                break;
            
            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

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