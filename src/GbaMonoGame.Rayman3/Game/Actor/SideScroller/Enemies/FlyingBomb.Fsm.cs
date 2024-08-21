﻿using System;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class FlyingBomb
{
    private bool FsmStep_CheckDeath()
    {
        if (HitPoints == 0)
        {
            State.MoveTo(Fsm_Destroyed);
            return false;
        }

        return true;
    }

    private bool Fsm_Move(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = CurrentDirectionalType switch
                {
                    null => Action.Move_Left,
                    PhysicalTypeValue.Enemy_Left => Action.Move_Left,
                    PhysicalTypeValue.Enemy_Right => Action.Move_Right,
                    PhysicalTypeValue.Enemy_Up => Action.Move_Up,
                    PhysicalTypeValue.Enemy_Down => Action.Move_Down,
                    _ => throw new ArgumentOutOfRangeException(nameof(CurrentDirectionalType), CurrentDirectionalType, null)
                };
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckDeath())
                    return false;

                // Play sound for helicopter bombs
                if ((ActorType)Type == ActorType.HelicopterBomb)
                {
                    if (SoundDelay != 0)
                    {
                        SoundDelay--;
                    }
                    else if (AnimatedObject.IsFramed && (GameInfo.ActorSoundFlags & ActorSoundFlags.FlyingBomb) == 0)
                    {
                        if (SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__BombFly_Mix03))
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BombFly_Mix03);

                        SoundDelay = 60;
                    }

                    if (AnimatedObject.IsFramed)
                        GameInfo.ActorSoundFlags |= ActorSoundFlags.FlyingBomb;
                }

                // Damage main actor
                if (Scene.IsHitMainActor(this))
                {
                    Scene.MainActor.ReceiveDamage(AttackPoints);
                    Destroyed = true;
                    Scene.MainActor.ProcessMessage(this, Message.Damaged);
                }

                // Boss machine
                if (CurrentDirectionalType == null)
                {
                    // Unused behavior, probably for showing help text box
                    if (Destroyed)
                        Scene.GetGameObject(5).ProcessMessage(this, Message.Murfy_Spawn);

                    Rayman rayman = (Rayman)Scene.MainActor;

                    if (rayman.IsInCutsceneState &&
                        Math.Abs(Position.X - Scene.MainActor.Position.X) < 300)
                    {
                        Destroyed = true;
                    }

                    Box vulnerabilityBox = GetVulnerabilityBox();
                    for (int i = 0; i < 2; i++)
                    {
                        RaymanBody activeFist = rayman.ActiveBodyParts[i];

                        if (activeFist == null) 
                            continue;
                        
                        if (!activeFist.GetAttackBox().Intersects(vulnerabilityBox)) 
                            continue;
                        
                        activeFist.ProcessMessage(this, Message.RaymanBody_FinishedAttack);
                        Destroyed = true;
                        break;
                    }
                }
                // Default
                else
                {
                    CurrentDirectionalType = Scene.GetPhysicalType(Position);
                }

                if (Destroyed || HitWall())
                {
                    State.MoveTo(Fsm_Destroyed);
                    return false;
                }

                if (CurrentDirectionalType is 
                    PhysicalTypeValue.Enemy_Left or 
                    PhysicalTypeValue.Enemy_Right or 
                    PhysicalTypeValue.Enemy_Up or 
                    PhysicalTypeValue.Enemy_Down)
                {
                    State.MoveTo(Fsm_Move);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_Destroyed(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                Explosion explosion = Scene.CreateProjectile<Explosion>(ActorType.Explosion);

                if (AnimatedObject.IsFramed)
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BangGen1_Mix07);
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BombFly_Mix03);
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BangGen1_Mix07);
                }

                if (explosion != null)
                    explosion.Position = Position;

                State.MoveTo(Fsm_Move);
                return false;

            case FsmAction.UnInit:
                CurrentDirectionalType = null;
                Destroyed = false;
                HitPoints = 1;
                ProcessMessage(this, Message.Destroy);
                break;
        }

        return true;
    }

    // FUN_0803a2fc
    private bool FUN_10011270(FsmAction action)
    {
        throw new NotImplementedException();
    }

    private bool Fsm_Stationary(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Stationary;
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckDeath())
                    return false;

                // Damage main actor
                if (Scene.IsHitMainActor(this))
                {
                    Scene.MainActor.ReceiveDamage(AttackPoints);
                    Destroyed = true;
                    Scene.MainActor.ProcessMessage(this, Message.Damaged);
                }

                if (Destroyed)
                {
                    State.MoveTo(Fsm_Destroyed);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}