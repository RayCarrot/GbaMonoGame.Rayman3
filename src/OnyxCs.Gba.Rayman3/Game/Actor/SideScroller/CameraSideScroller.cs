using System;
using BinarySerializer.Onyx.Gba;
using ImGuiNET;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class CameraSideScroller : CameraActor2D
{
    public CameraSideScroller(Scene2D scene) : base(scene)
    {
        Fsm.ChangeAction(Fsm_Default);

        TargetY = 120;
        field20_0x32 = 1;

        if (!MultiplayerManager.IsInMultiplayer)
        {
            HorizontalOffset = Engine.Settings.Platform switch
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

    private static readonly float[] UnknownTable =
    {
        1.00f, 2.00f, 4.00f, 6.00f,
        6.00f, 6.00f, 6.00f, 6.00f,
        5.75f, 5.50f, 5.25f, 5.00f,
        4.75f, 4.50f, 4.25f, 4.00f,
        3.75f, 3.50f, 3.25f, 3.00f,
        2.75f, 2.50f, 2.25f, 2.00f,
        1.75f, 1.50f, 1.25f, 1.00f,
        0.75f, 0.50f, 0.25f, 0.00f,
    };

    public byte HorizontalOffset { get; set; }
    public byte TargetX { get; set; }
    public byte TargetY { get; set; }
    public Vector2 PreviousLinkedObjectPosition { get; set; }
    public bool IsFacingRight { get; set; }
    public uint Timer { get; set; }
    public Vector2 Speed { get; set; }

    // Unknown
    public ushort field12_0x28 { get; set; }
    public ushort field13_0x2a { get; set; }
    public byte field16_0x2e { get; set; }
    public byte field18_0x30 { get; set; }
    public byte field20_0x32 { get; set; } // TODO: Enum? ScrollYMode?
    public byte field21_0x33 { get; set; }

    private void UpdateTargetX()
    {
        if (LinkedObject.IsFacingLeft)
            TargetX = Engine.Settings.Platform switch
            {
                Platform.GBA => (byte)(-HorizontalOffset - 16),
                Platform.NGage => (byte)(-80 - HorizontalOffset),
                _ => throw new UnsupportedPlatformException()
            };
        else
            TargetX = HorizontalOffset;
    }

    // What does this function do?
    private Vector2 FUN_0801d7b0(Vector2 speed)
    {
        if (field12_0x28 == 0)
            return speed;

        field13_0x2a++;

        int index = field18_0x30 * 2;

        if (field13_0x2a == (field18_0x30 + 1) * (field12_0x28 / 16)) // TODO: Different on N-Gage
        {
            field18_0x30++;
            index = field18_0x30 * 2;
        }
        else if (field13_0x2a > field18_0x30 * (field12_0x28 / 16))
        {
            index++;
        }

        if (field12_0x28 - 1 <= field13_0x2a)
            field12_0x28 = 0;

        if (field21_0x33 != 0 || (field13_0x2a & 7) == 4)
        {
            field21_0x33 = 1;

            if ((field13_0x2a & 7) == 0)
                return speed + new Vector2(0, UnknownTable[index]);
            else if ((field13_0x2a & 7) == 4)
                return speed + new Vector2(0, -UnknownTable[index]);
        }

        return speed;
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
                                 Engine.ScreenCamera.GameResolution.X - 40 - HorizontalOffset < LinkedObject.ScreenPosition.X &&
                                 Engine.ScreenCamera.GameResolution.X - HorizontalOffset > LinkedObject.ScreenPosition.X))
                            {
                                if (Timer > 2 && Math.Abs(Speed.X) > 1)
                                {
                                    Speed -= new Vector2(dir, 0);
                                    Timer = 0;
                                }
                            }
                            else
                            {
                                if (Timer > 5 && Math.Abs(Speed.X) < 4)
                                {
                                    Speed += new Vector2(dir, 0);
                                    Timer = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        Speed += new Vector2(dir, 0);
                        Timer = 0;
                    }
                }

                float linkedObjDeltaY = LinkedObject.Position.Y - PreviousLinkedObjectPosition.Y;

                if (field20_0x32 == 0)
                {
                    if ((LinkedObject.ScreenPosition.Y < 70 && linkedObjDeltaY < 0) ||
                        (LinkedObject.ScreenPosition.Y > 130 && linkedObjDeltaY > 0))
                    {
                        Speed = new Vector2(Speed.X, linkedObjDeltaY);
                    }
                }
                else
                {
                    if (Math.Abs(LinkedObject.ScreenPosition.Y - TargetY) <= 4)
                    {
                        Speed = new Vector2(Speed.X, linkedObjDeltaY);

                        if (field20_0x32 == 2)
                            field20_0x32 = 0;
                    }
                    else
                    {
                        if (TargetY < LinkedObject.ScreenPosition.Y)
                        {
                            if (linkedObjDeltaY >= 2)
                            {
                                Speed = new Vector2(Speed.X, 5);
                            }
                            else if (LinkedObject.ScreenPosition.Y - TargetY >= 21)
                            {
                                Speed = new Vector2(Speed.X, 3);
                            }
                            else if (linkedObjDeltaY >= 1)
                            {
                                Speed = new Vector2(Speed.X, 2);
                            }
                            else
                            {
                                Speed = new Vector2(Speed.X, 1);
                            }
                        }
                        else
                        {
                            if (linkedObjDeltaY <= -2)
                            {
                                Speed = new Vector2(Speed.X, -5);
                            }
                            else if (LinkedObject.ScreenPosition.Y - TargetY <= -21)
                            {
                                Speed = new Vector2(Speed.X, -3);
                            }
                            else if (linkedObjDeltaY <= -1)
                            {
                                Speed = new Vector2(Speed.X, -2);
                            }
                            else
                            {
                                Speed = new Vector2(Speed.X, -1);
                            }
                        }
                    }
                }

                Speed = FUN_0801d7b0(Speed);

                // Clamp speed
                Speed = new Vector2(Math.Clamp(Speed.X, -7, 7), Math.Clamp(Speed.Y, -7, 7));

                TgxCamera2D tgxCam = Scene.Playfield.Camera;
                TgxCluster mainCluster = tgxCam.GetMainCluster();

                tgxCam.Position += Speed;
                
                PreviousLinkedObjectPosition = LinkedObject.Position;

                // Reset if changed direction
                if (!mainCluster.IsOnLimit(Edge.Left) && 
                    !mainCluster.IsOnLimit(Edge.Right) && 
                    LinkedObject.IsFacingRight != IsFacingRight)
                    Fsm.ChangeAction(Fsm_Default);

                if (field16_0x2e == 4)
                {
                    HorizontalOffset = Engine.Settings.Platform switch
                    {
                        Platform.GBA => 40,
                        Platform.NGage => 25,
                        _ => throw new UnsupportedPlatformException()
                    };
                    field16_0x2e = 1;
                }
                break;
            
            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (base.ProcessMessageImpl(message, param))
            return false;

        // TODO: Implement remaining messages
        switch (message)
        {
            case Message.Cam_1027:
                field16_0x2e = 1;
                return true;

            case Message.Cam_1039:
                field20_0x32 = 0;
                TargetY = (byte)param;
                return true;

            case Message.Cam_1040:
                field20_0x32 = 1;
                TargetY = (byte)param;
                return true;

            default:
                return false;
        }
    }

    public override void SetFirstPosition()
    {
        Vector2 pos;

        if (LinkedObject.Position.X < HorizontalOffset && !LinkedObject.IsFacingLeft)
        {
            pos = new Vector2(0, LinkedObject.Position.Y);
        }
        else if (LinkedObject.Position.X < (Engine.ScreenCamera.GameResolution.X - HorizontalOffset) && LinkedObject.IsFacingLeft)
        {
            pos = new Vector2(0, LinkedObject.Position.Y);
        }
        else
        {
            if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
            {
                HorizontalOffset = Engine.Settings.Platform switch
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
                    pos = new Vector2(LinkedObject.Position.X + HorizontalOffset - Engine.ScreenCamera.GameResolution.X, LinkedObject.Position.Y);
                }
            }
        }

        pos.Y = Math.Max(pos.Y - 120, 0);

        Scene.Playfield.Camera.Position = pos;
        PreviousLinkedObjectPosition = LinkedObject.Position;
    }

    public override void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        base.DrawDebugLayout(debugLayout, textureManager);

        ImGui.Text($"Speed: {Speed.X} x {Speed.Y}");
        ImGui.Text($"Target: {TargetX} x {TargetY}");
    }
}