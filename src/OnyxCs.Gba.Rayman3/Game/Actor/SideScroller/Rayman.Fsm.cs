using System;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public partial class Rayman : MovableActor
{
    private void Fsm_LevelStart(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingLeft ? 73 : 72;
                ChangeAction();

                Timer = 0;

                CameraSideScroller cam = (CameraSideScroller)Frame.GetComponent<Scene2D>().Camera;
                if (GameInfo.MapId == MapId.TheCanopy_M2)
                {
                    cam.HorOffset = 120;
                }
                else
                {
                    if (!MultiplayerManager.IsInMultiplayer)
                        cam.HorOffset = 40;
                    else if (IsLocalPlayer)
                        cam.HorOffset = 95;
                }

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4)
                    cam.HorOffset = 120;
                break;

            case FsmAction.Step:
                if ((RaymanFlags & 0x40) != 0)
                {
                    if (TransitionsFX.FadeCoefficient == 0)
                    {
                        if (ActionId is not (217 or 218))
                            ActionId = IsFacingLeft ? 218 : 217;
                    }
                    else
                    {
                        ActionId = IsFacingLeft ? 188 : 187;
                    }
                }

                Timer++;

                if (IsActionFinished && FUN_0802c53c())
                    NextActionId = IsFacingLeft ? 202 : 201;

                if (!IsActionFinished)
                    return;

                if (MultiplayerManager.IsInMultiplayer && Timer < 210)
                    return;

                Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Default(FsmAction action)
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        CameraSideScroller cam = (CameraSideScroller)scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                FUN_08029ec4();

                if (field19_0x94 == 32 || MechSpeedX + 1.5f <= 3)
                {
                    if (scene.Camera.LinkedObject == this)
                        SoundManager.Play(208, -1);

                    if (field19_0x94 == 32)
                        MechSpeedX = 0;

                    if (!FUN_0802c53c())
                    {
                        if (NextActionId == -1)
                        {
                            if (Random.Shared.Next(11) < 6)
                            {
                                ActionId = IsFacingLeft ? 147 : 146;
                            }
                            else
                            {
                                ActionId = IsFacingLeft ? 1 : 0;
                            }
                        }
                        else
                        {
                            ActionId = NextActionId;
                        }
                    }
                    else if (NextActionId is 201 or 202)
                    {
                        ActionId = NextActionId;
                    }
                    else
                    {
                        ActionId = IsFacingLeft ? 200 : 199;
                    }
                }
                else
                {
                    FUN_0802a2dc();
                }

                Timer = 0;
                break;

            case FsmAction.Step:
                if ((RaymanFlags & 0x2000) != 0)
                {
                    field16_0x91++;

                    if (field16_0x91 > 60)
                    {
                        cam.HorOffset = 40;
                        RaymanFlags &= 0xdfff;
                    }
                }

                if (IsLocalPlayer &&
                    !Fsm.EqualsAction(Fsm_Jump) &&
                    !Fsm.EqualsAction(FUN_0802ce54) &&
                    !Fsm.EqualsAction(FUN_080284ac) &&
                    !Fsm.EqualsAction(FUN_08033b34) &&
                    !Fsm.EqualsAction(FUN_080287d8) &&
                    (RaymanFlags & 0x40) == 0)
                {
                    // TODO: Name camera messages
                    if (!Fsm.EqualsAction(FUN_0802ddac) &&
                        CheckInput(GbaInput.Down) &&
                        (Speed.Y > 0 || Fsm.EqualsAction(FUN_080254d8)) &&
                        !Fsm.EqualsAction(FUN_08026cd4))
                    {
                        field18_0x93 = 70;
                        cam.SendMessage((Message)1040, field18_0x93);
                    }
                    else if (CheckInput(GbaInput.Up) && (Fsm.EqualsAction(Fsm_Default) || Fsm.EqualsAction(FUN_0802ea74)))
                    {
                        field18_0x93 = 160;
                        cam.SendMessage((Message)1040, field18_0x93);
                    }
                    else if (Fsm.EqualsAction(FUN_0802d44c) && field27_0x9c == 0)
                    {
                        cam.SendMessage((Message)1039, field18_0x93);
                    }
                    else if (Fsm.EqualsAction(FUN_0803283c))
                    {
                        field18_0x93 = 65;
                        cam.SendMessage((Message)1040, field18_0x93);
                    }
                    else if (Fsm.EqualsAction(FUN_08026cd4) || Fsm.EqualsAction(FUN_0802ddac))
                    {
                        field18_0x93 = 112;
                        cam.SendMessage((Message)1040, field18_0x93);
                    }
                    else
                    {
                        field18_0x93 = 120;
                        cam.SendMessage((Message)1040, field18_0x93);
                    }
                }

                if (field23_0x98 != 0)
                    field23_0x98--;

                if (!IsOnInstaKillType())
                {
                    FUN_08029ec4();
                    HorizontalMovement();

                    if (CheckForDamage())
                    {
                        Fsm.ChangeAction(FUN_08031d24);

                        if (ActionId is not (213 or 214) && field19_0x94 == 32)
                            ActionId = IsFacingLeft ? 214 : 213;
                    }
                    else
                    {
                        if (CheckSingleInput(GbaInput.A) && field19_0x94 != 32)
                        {
                            Fsm.ChangeAction(FUN_0802cb38);

                            if (ActionId is not (213 or 214) && field19_0x94 == 32)
                                ActionId = IsFacingLeft ? 214 : 213;
                        }
                        else
                        {
                            if (FUN_0802a0f8())
                            {
                                if (cam.LinkedObject == this)
                                    SoundManager.Play(208, -1);

                                Fsm.ChangeAction(FUN_0802cb38);

                                if (ActionId is not (213 or 214) && field19_0x94 == 32)
                                    ActionId = IsFacingLeft ? 214 : 213;
                            }
                            else
                            {
                                Timer++;

                                if (!CheckInput(GbaInput.Up))
                                {
                                    if (ActionId is 213 or 214)
                                    {
                                        ActionId = IsFacingLeft ? 1 : 0;
                                    }
                                }
                                else
                                {
                                    if (ActionId is not (213 or 214) && field19_0x94 == 32)
                                        ActionId = IsFacingLeft ? 214 : 213;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!MultiplayerManager.IsInMultiplayer)
                    {
                        Fsm.ChangeAction(FUN_08032650);
                    }
                    else
                    {
                        Fsm.ChangeAction(FUN_08033228);
                    }

                    if (ActionId is not (213 or 214) && field19_0x94 == 32)
                        ActionId = IsFacingLeft ? 214 : 213;
                }

                if (IsActionFinished)
                {
                    if ((ActionId == NextActionId && ActionId is not (146 or 147 or 193 or 194)) &&
                        (ActionId is not (173 or 174 or 177 or 178) || Timer > 180))
                    {
                        if (!FUN_0802c53c())
                        {
                            ActionId = IsFacingLeft ? 1 : 0;
                        }
                        else
                        {
                            ActionId = IsFacingLeft ? 200 : 199;
                        }

                        NextActionId = -1;

                        if (cam.LinkedObject == this)
                            SoundManager.Play(340, -1);
                    }
                }

                if (field19_0x94 == 32 || MechSpeedX + 1.5f <= 3)
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(208, -1);

                    if (NextActionId != -1 && NextActionId != ActionId)
                    {
                        ActionId = NextActionId;
                    }
                    else
                    {
                        if (ActionId is not (213 or 214 or 0 or 1 or 199 or 200) && ActionId != NextActionId)
                        {
                            if (!FUN_0802c53c())
                            {
                                ActionId = IsFacingLeft ? 1 : 0;
                            }
                            else
                            {
                                ActionId = IsFacingLeft ? 200 : 199;
                            }
                        }
                    }
                }
                else
                {
                    FUN_0802a2dc();
                }

                if (CheckInput(GbaInput.Left) && !IsFacingLeft)
                {
                    ActionId = 3;
                    ChangeAction();
                }
                else if (CheckInput(GbaInput.Right) && IsFacingLeft)
                {
                    ActionId = 2;
                    ChangeAction();
                }

                if (CheckSingleInput(GbaInput.A) && MultiplayerFlag_2)
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(Fsm_Jump);
                }
                else if (CheckInput(GbaInput.Down))
                {
                    NextActionId = IsFacingLeft ? 131 : 130;

                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_080254d8);
                }
                else if (Speed.Y > 1)
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_0802cfec);
                }
                else if (CheckInput(GbaInput.Left) || CheckInput(GbaInput.Right))
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_08024470);
                }
                else if (field23_0x98 == 0 && CheckSingleInput(GbaInput.B) && FUN_0802c414(2))
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Fsm.ChangeAction(FUN_0802f5d8);
                }
                else if (MechSpeedX == 0 || FUN_0802986c() == 0 || (RaymanFlags & 1) != 0)
                {
                    if (MechSpeedX != 0 || FUN_0802986c() == 0 || (RaymanFlags & 1) != 0)
                    {
                        if ((IsActionFinished && ActionId is not (213 or 214 or 146 or 147 or 193 or 194) && 360 < Timer) ||
                            (IsActionFinished && ActionId is 0x92 or 0x93 or 0xc1 or 0xc2 && 720 < Timer))
                        {
                            FUN_0802a65c();
                            Fsm.ChangeAction(Fsm_Default);
                        }
                        else
                        {
                            RaymanFlags &= 0xfffe;
                        }
                    }
                    else
                    {
                        if (cam.LinkedObject == this)
                            SoundManager.Play(340, -1);

                        Fsm.ChangeAction(Fsm_StandingNearEdge);
                    }
                }
                else
                {
                    if (cam.LinkedObject == this)
                        SoundManager.Play(340, -1);

                    Position += new Vector2(MechSpeedX < 0 ? -16 : 16);

                    Fsm.ChangeAction(FUN_0802cfec);
                }
                break;

            case FsmAction.UnInit:
                if (ActionId is 142 or 143 && cam.LinkedObject == this)
                    SoundManager.Play(240, -1);

                if (ActionId == NextActionId || ActionId is 2 or 3 or 179 or 180)
                    NextActionId = -1;

                if (GameInfo.MapId is MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4 && cam.HorOffset == 120)
                    cam.HorOffset = 40;
                break;
        }
    }

    // TODO: Implement
    private void Fsm_StandingNearEdge(FsmAction action) { }

    private void Fsm_Jump(FsmAction action)
    {
        Scene2D scene = Frame.GetComponent<Scene2D>();
        CameraSideScroller cam = (CameraSideScroller)scene.Camera;

        switch (action)
        {
            case FsmAction.Init:
                if (scene.Camera.LinkedObject == this)
                    SoundManager.Play(208, -1);

                if (ActionId is not (109 or 110))
                {
                    ActionId = IsFacingLeft ? 11 : 10;

                    if (scene.Camera.LinkedObject == this)
                        SoundManager.Play(108, -1);
                }

                NextActionId = -1;
                field18_0x93 = 70;

                // TODO: Name message
                if (IsLocalPlayer)
                    cam.SendMessage((Message)1039, field18_0x93);

                Timer = (uint)GameTime.ElapsedFrames;
                field19_0x94 = 32;
                LinkedMovementActor = null;
                break;

            case FsmAction.Step:
                // TODO: Implement

                break;

            case FsmAction.UnInit:
                MultiplayerFlag_0 = false;
                break;
        }
    }

    // TODO: Implement all of these
    private void FUN_0802ce54(FsmAction action) { }
    private void FUN_080284ac(FsmAction action) { }
    private void FUN_08033b34(FsmAction action) { }
    private void FUN_080287d8(FsmAction action) { }
    private void FUN_0802ddac(FsmAction action) { }
    private void FUN_08026cd4(FsmAction action) { }
    private void FUN_080254d8(FsmAction action) { }
    private void FUN_0802ea74(FsmAction action) { }
    private void FUN_0802d44c(FsmAction action) { }
    private void FUN_0803283c(FsmAction action) { }
    private void FUN_08031d24(FsmAction action) { }
    private void FUN_0802cb38(FsmAction action) { }
    private void FUN_08032650(FsmAction action) { }
    private void FUN_08033228(FsmAction action) { }
    private void FUN_0802cfec(FsmAction action) { }
    private void FUN_08024470(FsmAction action)
    {

    }
    private void FUN_0802f5d8(FsmAction action) { }
}