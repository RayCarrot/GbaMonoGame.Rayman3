using System;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Jano
{
    private bool FsmStep_CheckHit()
    {
        // NOTE: In the original game this changes the alpha one step every second frame and
        //       uses direct equality checks rather than less than or greater than
        if (IsBeingAttacked())
        {
            if (AlphaBlend > 4)
            {
                AlphaBlend -= 0.5f;

                if (AlphaBlend <= 4 && !SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__JanoRire_Mix01))
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__JanoRire_Mix01);
            }
        }
        else
        {
            if (AlphaBlend < 0x10)
                AlphaBlend += 0.5f;
        }

        AnimatedObject.GbaAlpha = AlphaBlend;

        return true;
    }

    private bool Fsm_Intro(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Timer = 0;
                ActionId = Action.Idle_Left;
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckHit())
                    return false;

                // Begin intro sequence when Rayman reaches its position
                if (Scene.MainActor.Position.X > 375 && Timer == 0)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_Stop);

                    if (Scene.MainActor.IsFacingLeft)
                        Scene.MainActor.AnimatedObject.FlipX = false;

                    Timer = 1;
                }
                // Move on screen
                else if (Timer == 1)
                {
                    if (Position.X <= 480)
                        Timer = 2;
                    else
                        Position -= new Vector2(2, 0);
                }
                // Wait
                else if (Timer is >= 2 and < 30)
                {
                    Timer++;
                }
                // Grimace
                else if (Timer == 30)
                {
                    ((Rayman)Scene.MainActor).ActionId = Rayman.Action.Idle_Grimace_Right;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Grimace1_Mix04);
                    Timer = 31;
                    ActionId = Action.Grimace_Left;
                }
                // Wait
                else if (Timer > 30)
                {
                    Timer++;
                }

                if (IsActionFinished && ActionId is Action.Grimace_Right or Action.Grimace_Left && Timer > 60)
                {
                    State.MoveTo(Fsm_MoveAway);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Create a checkpoint to avoid showing the intro cutscene each time
                GameInfo.SetCheckpoint(new Vector2(378, 208));
                break;
        }

        return true;
    }

    private bool Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsOnLeftSide ? Action.Idle_Right : Action.Idle_Left;
                Timer = 0;
                field_0x6c = Scene.Playfield.Camera.Position.X + (Scene.Resolution.X - 30);
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckHit())
                    return false;

                Timer++;

                // Move back
                if (!IsOnLeftSide && Position.X - Scene.MainActor.Position.X > 170)
                {
                    Position -= new Vector2(2, 0);
                    return true;
                }
                else if (IsOnLeftSide && Scene.MainActor.Position.X - Position.X > 170)
                {
                    Position += new Vector2(2, 0);
                    return true;
                }

                // Move away if Rayman gets too close
                if (!IsOnLeftSide && Position.X - Scene.MainActor.Position.X < 50)
                {
                    State.MoveTo(Fsm_BeginMoveAway);
                    return false;
                }

                if (!IsOnLeftSide && Position.X - Scene.MainActor.Position.X < 120 && Scene.MainActor.Speed.Y == 0)
                {
                    State.MoveTo(Fsm_BeginMoveAway);
                    return false;
                }

                if (IsOnLeftSide && Scene.MainActor.Position.X - Position.X < 50)
                {
                    State.MoveTo(Fsm_BeginMoveAway);
                    return false;
                }

                if (IsOnLeftSide && Scene.MainActor.Position.X - Position.X < 120 && Scene.MainActor.Speed.Y == 0)
                {
                    State.MoveTo(Fsm_BeginMoveAway);
                    return false;
                }

                // Attack if there is remaining ammo
                if (Timer > 60 && Ammo != 0)
                {
                    State.MoveTo(Fsm_Attack);
                    return false;
                }

                // If no ammo then create skull platform
                if (Timer > 60)
                {
                    RefillAmmo();
                    State.MoveTo(Fsm_CreateSkullPlatform);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_BeginMoveAway(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsOnLeftSide ? Action.Grimace_Right : Action.Grimace_Left;
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckHit())
                    return false;

                float distX = Math.Abs(Position.X - Scene.MainActor.Position.X);
                if (distX < 50 && Position.X < 1850)
                {
                    if (IsOnLeftSide)
                        Position -= new Vector2(3, 0);
                    else
                        Position += new Vector2(3, 0);
                }

                if (IsActionFinished && ActionId is Action.Grimace_Right or Action.Grimace_Left)
                {
                    State.MoveTo(Fsm_MoveAway);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_MoveAway(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (field_0x7e == 0)
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Janogrrr_Mix03);
                else
                    field_0x7e = 0;

                if (ActionId is not (Action.Move_Right or Action.Move_Left))
                    ActionId = IsOnLeftSide ? Action.TurnAroundSlow_Right : Action.TurnAroundSlow_Left;
                
                Timer = 0;
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckHit())
                    return false;

                Timer++;

                float distX = Math.Abs(Position.X - Scene.MainActor.Position.X);
                if (distX < 50 && Position.X < 1850)
                {
                    if (IsOnLeftSide)
                        Position -= new Vector2(3, 0);
                    else
                        Position += new Vector2(3, 0);
                }

                // Finish initial turning around
                if (ActionId is Action.TurnAroundSlow_Right or Action.TurnAroundSlow_Left && IsActionFinished)
                {
                    ActionId = IsOnLeftSide ? Action.Move_Left : Action.Move_Right;
                    Timer = 0;
                }

                if (IsReadyToTurnBackAround() && GameTime.ElapsedFrames - Timer > 10 && ActionId is Action.Move_Right or Action.Move_Left)
                {
                    // Update y position if Rayman is not on a skull platform
                    if (Scene.MainActor.LinkedMovementActor == null)
                    {
                        Timer = (ushort)Random.GetNumber(751);

                        int phase = CheckCurrentPhase();

                        if (phase == 1)
                        {
                            if (Timer < 376)
                                Position = Position with { Y = OffsetY + 110 };
                            else
                                Position = Position with { Y = OffsetY + 150 };
                        }
                        else if (phase is 2 or 3)
                        {
                            if (Timer < 250)
                                Position = Position with { Y = OffsetY + 150 };
                            else if (Timer < 500)
                                Position = Position with { Y = OffsetY + 110 };
                            else
                                Position = Position with { Y = OffsetY + 200 };
                        }
                    }

                    ActionId = IsOnLeftSide ? Action.TurnAroundFast_Right : Action.TurnAroundFast_Left;
                    ChangeAction();
                }

                if (Position.X > 2050)
                {
                    State.MoveTo(FUN_1001c98c);
                    return false;
                }

                if (IsActionFinished && 
                    ActionId is Action.TurnAroundFast_Right or Action.TurnAroundFast_Left && 
                    Scene.MainActor.Position.X > 1400 &&
                    Scene.MainActor.Speed.Y == 0)
                {
                    State.MoveTo(FUN_1001c98c);
                    return false;
                }

                if (IsActionFinished && 
                    ActionId is Action.TurnAroundFast_Right or Action.TurnAroundFast_Left)
                {
                    State.MoveTo(Fsm_MoveBack);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                Scene.MainActor.ProcessMessage(this, Message.Main_ExitStopOrCutscene);
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__Grimace1_Mix04);
                break;
        }

        return true;
    }

    private bool Fsm_MoveBack(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsOnLeftSide ? Action.Idle_Right : Action.Idle_Left;
                Timer = 1;
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckHit())
                    return false;

                Timer++;

                // Move back
                if (!IsOnLeftSide && Position.X - Scene.MainActor.Position.X > 170)
                    Position -= new Vector2(2, 0);
                else if (IsOnLeftSide && Scene.MainActor.Position.X - Position.X > 170)
                    Position += new Vector2(2, 0);

                if (Timer > 45 && Scene.MainActor.Speed.Y == 0)
                {
                    State.MoveTo(Fsm_Default);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    // TODO: Implement
    private bool Fsm_CreateSkullPlatform(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:

                break;

            case FsmAction.Step:

                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    // TODO: Implement
    private bool Fsm_Attack(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:

                break;

            case FsmAction.Step:

                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    // TODO: Implement
    // FUN_0806acd4
    private bool FUN_1001c98c(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:

                break;

            case FsmAction.Step:

                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}