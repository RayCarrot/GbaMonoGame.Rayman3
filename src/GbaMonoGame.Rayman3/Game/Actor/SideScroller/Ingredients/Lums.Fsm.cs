﻿using System;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Lums
{
    private bool Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (GameInfo.MapId == MapId.BossRockAndLava)
                    BossDespawnTimer = 0;
                break;

            case FsmAction.Step:
                bool collected = false;
                
                if (Scene.MainActor.GetDetectionBox().Intersects(GetViewBox()))
                {
                    if (ActionId == Action.BlueLum)
                        collected = CheckCollision();
                    else
                        collected = CheckCollisionAndAttract();
                }

                if (GameInfo.MapId == MapId.BossRockAndLava && !collected)
                {
                    BossDespawnTimer++;
                    
                    if (BossDespawnTimer > 120)
                    {
                        collected = true;
                        BossDespawnTimer = 1;
                    }
                }

                // Lums have 3 random animations they cycle between, showing different sparkles
                if (IsActionFinished)
                {
                    if (ActionId == Action.BlueLum)
                    {
                        AnimatedObject.CurrentAnimation = 0 + Random.GetNumber(3);
                    }
                    else if (ActionId == Action.WhiteLum)
                    {
                        AnimatedObject.CurrentAnimation = 3 + Random.GetNumber(3);
                    }
                    else if (ActionId is not (Action.BigYellowLum or Action.BigBlueLum))
                    {
                        AnimatedObject.CurrentAnimation = (byte)ActionId * 3 + Random.GetNumber(3);
                    }
                }

                if (collected)
                {
                    State.MoveTo(Fsm_Collected);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_Collected(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (GameInfo.MapId == MapId.BossRockAndLava)
                {
                    // Check if the timer finished and the lum should just despawn
                    if (BossDespawnTimer == 1)
                        return true;

                    BossDespawnTimer = 0;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumBleu_Mix02);
                }
                else
                {
                    switch (ActionId)
                    {
                        case Action.YellowLum:
                            GameInfo.SetYellowLumAsCollected(LumId);
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__LumOrag_Mix06);
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumOrag_Mix06);
                            break;

                        case Action.RedLum:
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__LumRed_Mix03);
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumRed_Mix03);
                            break;
                        
                        case Action.GreenLum:
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__LumGreen_Mix04);
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumGreen_Mix04);

                            Vector2 pos = Position;
                            while (Scene.GetPhysicalType(pos) == PhysicalTypeValue.None)
                                pos += new Vector2(0, Tile.Size);

                            GameInfo.SetCheckpoint(pos);
                            break;

                        case Action.BlueLum:
                            // TODO: Implement
                            break;

                        case Action.WhiteLum:
                            GameInfo.HasCollectedWhiteLum = true;
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__LumSlvr_Mix02);
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumSlvr_Mix02);
                            break;

                        case Action.BigYellowLum:
                            // TODO: Implement
                            break;

                        case Action.BigBlueLum:
                            // TODO: Implement
                            break;
                    }
                }

                Scene.MainActor.ProcessMessage(this, ActionId switch
                {
                    Action.YellowLum => Message.Main_CollectedYellowLum,
                    Action.RedLum => Message.Main_CollectedRedLum,
                    Action.GreenLum => Message.Main_CollectedGreenLum,
                    Action.BlueLum => Message.Main_CollectedBlueLum,
                    Action.WhiteLum => Message.Main_CollectedWhiteLum,
                    Action.UnusedLum => Message.Main_CollectedUnusedLum,
                    Action.BigYellowLum => Message.Main_CollectedBigYellowLum,
                    Action.BigBlueLum => Message.Main_CollectedBigBlueLum,
                    _ => throw new ArgumentOutOfRangeException(nameof(ActionId), ActionId, null)
                });
                AnimatedObject.CurrentAnimation = 9;
                break;

            case FsmAction.Step:
                if (IsActionFinished && ActionId == Action.BlueLum && GameInfo.MapId != MapId.BossRockAndLava)
                {
                    State.MoveTo(FUN_0805e6b8);
                    return false;
                }
                
                if (IsActionFinished)
                {
                    State.MoveTo(Fsm_Idle);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                if (ActionId != Action.BlueLum || GameInfo.MapId == MapId.BossRockAndLava)
                    ProcessMessage(this, Message.Destroy);
                break;
        }

        return true;
    }

    // TODO: Implement
    private bool FUN_0805ed40(FsmAction action) => true;
    private bool FUN_0805e6b8(FsmAction action) => true;
    private bool FUN_0805e83c(FsmAction action) => true;
    private bool FUN_0805e844(FsmAction action) => true;
}