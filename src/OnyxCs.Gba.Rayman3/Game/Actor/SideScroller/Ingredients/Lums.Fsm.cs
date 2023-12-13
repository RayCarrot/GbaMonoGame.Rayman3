using System;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class Lums
{
    private void Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (GameInfo.MapId == MapId.BossRockAndLava)
                    BossDespawnTimer = 0;
                break;

            case FsmAction.Step:
                bool collected = false;
                
                if (Scene.IsDetectedMainActor(GetViewBox()))
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
                if (AnimatedObject.EndOfAnimation)
                {
                    if (ActionId == Action.BlueLum)
                    {
                        AnimatedObject.CurrentAnimation = 0 + Random.Shared.Next(3);
                    }
                    else if (ActionId == Action.WhiteLum)
                    {
                        AnimatedObject.CurrentAnimation = 3 + Random.Shared.Next(3);
                    }
                    else if (ActionId is not (Action.BigYellowLum or Action.BigBlueLum))
                    {
                        AnimatedObject.CurrentAnimation = (byte)ActionId + Random.Shared.Next(3);
                    }
                }

                if (collected)
                    Fsm.ChangeAction(Fsm_Collected);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Collected(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (GameInfo.MapId == MapId.BossRockAndLava)
                {
                    // Check if the timer finished and the lum should just despawn
                    if (BossDespawnTimer == 1)
                        return;

                    SoundManager.Play(Rayman3SoundEvent.Play__LumBleu_Mix02);
                    Scene.MainActor.ProcessMessage((Message)(0x3f1 + ActionId)); // TODO: Name messages and handle in Rayman
                    AnimatedObject.CurrentAnimation = 9;
                    BossDespawnTimer = 0;
                }
                else
                {
                    switch (ActionId)
                    {
                        case Action.YellowLum:
                            GameInfo.SetYellowLumAsCollected(LumId);
                            SoundManager.Play(Rayman3SoundEvent.Stop__LumOrag_Mix06);
                            SoundManager.Play(Rayman3SoundEvent.Play__LumOrag_Mix06);
                            break;

                        case Action.RedLum:
                            SoundManager.Play(Rayman3SoundEvent.Stop__LumRed_Mix03);
                            SoundManager.Play(Rayman3SoundEvent.Play__LumRed_Mix03);
                            break;
                        
                        case Action.GreenLum:
                            // TODO: Implement
                            break;

                        case Action.BlueLum:
                            // TODO: Implement
                            break;

                        case Action.WhiteLum:
                            // TODO: Implement
                            break;

                        case Action.BigYellowLum:
                            // TODO: Implement
                            break;

                        case Action.BigBlueLum:
                            // TODO: Implement
                            break;
                    }

                    Scene.MainActor.ProcessMessage((Message)(0x3f1 + ActionId)); // TODO: Name messages and handle in Rayman
                    AnimatedObject.CurrentAnimation = 9;
                }
                break;

            case FsmAction.Step:
                if (AnimatedObject.EndOfAnimation)
                {
                    if (ActionId != Action.BlueLum || GameInfo.MapId == MapId.BossRockAndLava)
                    {
                        Fsm.ChangeAction(Fsm_Idle);

                        // N-Gage doesn't do this for some reason
                        if (Engine.Settings.Platform == Platform.GBA && GameInfo.MapId == MapId.BossRockAndLava)
                            BossDespawnTimer = 0;
                    }
                    else
                    {
                        // TODO: Implement
                    }
                }
                break;

            case FsmAction.UnInit:
                if (ActionId != Action.BlueLum || GameInfo.MapId == MapId.BossRockAndLava)
                    ProcessMessage(Message.Destroy);
                break;
        }
    }
}