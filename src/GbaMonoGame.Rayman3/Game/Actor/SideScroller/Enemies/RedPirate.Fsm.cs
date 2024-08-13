﻿using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class RedPirate
{
    private bool FsmStep_DoInteractable()
    {
        // Invulnerability lasts 2 seconds
        if (IsInvulnerable && GameTime.ElapsedFrames - InvulnerabilityTimer > 120)
            IsInvulnerable = false;

        // Damage main actor
        if (Scene.IsHitMainActor(this))
        {
            Scene.MainActor.ReceiveDamage(AttackPoints);
            Scene.MainActor.ProcessMessage(this, Message.Damaged, this);
        }

        // Killed
        if (HitPoints == 0)
        {
            State.MoveTo(Fsm_Dying);
            return false;
        }

        // Taken damage
        if (HitPoints < PrevHitPoints && State != Fsm_Hit)
        {
            PrevHitPoints = HitPoints;
            State.MoveTo(Fsm_Hit);
            return false;
        }

        // Taken damage multiple times in quick succession
        if (HitPoints < PrevHitPoints)
        {
            PrevHitPoints = HitPoints;
            State.MoveTo(Fsm_HitKnockBack);
            return false;
        }

        return true;
    }

    private void Fsm_Fall(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Fall_Right : Action.Fall_Left;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                
                // Check if landed
                PhysicalType type = GetPhysicalGroundType();
                if (type.IsSolid && ActionId is Action.Fall_Right or Action.Fall_Left)
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PiraJump_BigFoot1_Mix02);
                    ActionId = IsFacingRight ? Action.Land_Right : Action.Land_Left;
                }

                // Wait for landing to finish
                if (IsActionFinished && ActionId is Action.Land_Right or Action.Land_Left)
                    State.MoveTo(Fsm_Idle);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Idle_Right : Action.Idle_Left;
                IdleDetectionTimer = GameTime.ElapsedFrames;
                AttackTimer = GameTime.ElapsedFrames;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (!FsmStep_DoInteractable())
                    return;

                // 180 frames and not detected main actor...
                if (GameTime.ElapsedFrames - IdleDetectionTimer > 180 && !Scene.IsDetectedMainActor(this))
                {
                    State.MoveTo(Fsm_Walk);
                    return;
                }

                // 75 frames and detected main actor...
                if (GameTime.ElapsedFrames - AttackTimer > 75 && Scene.IsDetectedMainActor(this))
                {
                    State.MoveTo(Fsm_Attack);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Walk(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Walk_Right : Action.Walk_Left;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (!FsmStep_DoInteractable())
                    return;

                // Reverse direction
                if (Speed.X == 0)
                    ActionId = ActionId == Action.Walk_Right ? Action.Walk_Left : Action.Walk_Right;

                Walk();

                if (Scene.IsDetectedMainActor(this))
                    State.MoveTo(Fsm_Attack);
                break;

            case FsmAction.UnInit:
                Ammo = Random.GetNumber(1) + 1;
                break;
        }
    }

    private void Fsm_Attack(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                HasFiredShot = false;

                if (Ammo == 0)
                    Ammo = Random.GetNumber(1) + 1;

                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PiraAtk1_Mix01__or__PiraHurt_Mix02);

                ActionId = Position.X - Scene.MainActor.Position.X < 0 ? Action.Shoot_Right : Action.Shoot_Left;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (!FsmStep_DoInteractable())
                    return;

                if (AnimatedObject.CurrentFrame == 6 && 
                    ActionId is Action.Shoot_Right or Action.Shoot_Left &&
                    !HasFiredShot)
                {
                    Shoot();
                    HasFiredShot = true;
                }

                if (IsActionFinished)
                    State.MoveTo(Fsm_Idle);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Hit(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PiraHit1_Mix02__or__PiraHit3_Mix03);
                if (HitFromFront)
                    ActionId = IsFacingRight ? Action.HitBehind_Right : Action.HitBehind_Left;
                else
                    ActionId = IsFacingRight ? Action.Hit_Right : Action.Hit_Left;
                DoubleHitTimer = GameTime.ElapsedFrames;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (!FsmStep_DoInteractable())
                    return;

                // Allow 20 frames to be double hit and cause a hit knock-back
                if (GameTime.ElapsedFrames - DoubleHitTimer > 20)
                {
                    StartInvulnerability();
                    Ammo = 2;
                    State.MoveTo(Fsm_Attack);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_HitKnockBack(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                KnockBackPosition = Position;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PiraHit1_Mix02__or__PiraHit3_Mix03);
                ActionId = IsFacingRight ? Action.HitKnockBack_Right : Action.HitKnockBack_Left;
                StartInvulnerability();
                CheckAgainstMapCollision = false;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);
                PhysicalType type = Scene.GetPhysicalType(Position);

                if (IsActionFinished)
                    ActionId = IsFacingRight ? Action.Hit_Right : Action.Hit_Left;

                if (type.Value is 
                        PhysicalTypeValue.InstaKill or 
                        PhysicalTypeValue.Damage or 
                        PhysicalTypeValue.Water or 
                        PhysicalTypeValue.MoltenLava ||
                    (type.IsSolid && KnockBackPosition.Y + 16 < Position.Y))
                {
                    Ammo = 1;
                    State.MoveTo(Fsm_Dying);
                    return;
                }
                
                if (type.IsSolid)
                {
                    Ammo = 1;
                    State.MoveTo(Fsm_ReturnFromKnockBack);
                    return;
                }
                break;

            case FsmAction.UnInit:
                CheckAgainstMapCollision = true;
                break;
        }
    }

    private void Fsm_ReturnFromKnockBack(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? Action.Walk_Right : Action.Walk_Left;
                break;

            case FsmAction.Step:
                LevelMusicManager.PlaySpecialMusicIfDetected(this);

                if (IsFacingRight && Position.X > KnockBackPosition.X ||
                    IsFacingLeft && Position.X < KnockBackPosition.X)
                {
                    Position = Position with { X = KnockBackPosition.X };
                    State.MoveTo(Fsm_Attack);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Dying(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (HitFromFront)
                    ActionId = IsFacingRight ? Action.DyingBehind_Right : Action.DyingBehind_Left;
                else
                    ActionId = IsFacingRight ? Action.Dying_Right : Action.Dying_Left;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PiraHit1_Mix02__or__PiraHit3_Mix03);
                IsSolid = false;
                LevelMusicManager.FUN_08001918();
                break;

            case FsmAction.Step:
                if (!AnimatedObject.IsDelayMode && AnimatedObject.CurrentFrame == 5)
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__PiraDead_Mix05);

                if (IsActionFinished)
                {
                    SpawnRedLum(32);
                    ProcessMessage(this, Message.Destroy);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}