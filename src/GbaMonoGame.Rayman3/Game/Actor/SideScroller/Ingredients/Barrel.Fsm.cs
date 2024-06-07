﻿using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Barrel
{
    private void Fsm_WaitForHit(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Idle;
                LastHitBodyPartType = null;
                break;

            case FsmAction.Step:
                if (LastHitBodyPartType != null)
                {
                    InitialHitPoints = HitPoints;
                    State.MoveTo(Fsm_Hit);
                }
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
                ActionId = Action.BeginHitImpact;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__WoodImp_Mix03);
                break;

            case FsmAction.Step:
                bool changedAction = false;
                if (IsActionFinished && ActionId == Action.BeginHitImpact)
                {
                    ActionId = Action.HitImpact;
                    changedAction = true;
                }

                // Fall into water
                if ((GameInfo.Powers & Power.DoubleFist) != 0 && HitPoints != InitialHitPoints && MoveOnWater)
                {
                    InitialHitPoints = HitPoints;
                    State.MoveTo(Fsm_FallIntoWater);
                    return;
                }

                // Fall to break
                if ((GameInfo.Powers & Power.DoubleFist) != 0 && HitPoints != InitialHitPoints && !MoveOnWater)
                {
                    InitialHitPoints = HitPoints;
                    State.MoveTo(Fsm_FallToBreak);
                    return;
                }

                // Finished impact animation
                if (IsActionFinished && !changedAction)
                {
                    HitPoints = 100;
                    InitialHitPoints = 100;
                    State.MoveTo(Fsm_WaitForHit);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_FallToBreak(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = LastHitFacingLeft ? Action.FallLeft : Action.FallRight;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__WoodImp_Mix03);
                break;

            case FsmAction.Step:
                if (Scene.GetPhysicalType(Position).IsSolid)
                    State.MoveTo(Fsm_Break);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Break(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Break;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__WoodBrk1_Mix04);
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                    State.MoveTo(Fsm_WaitForHit);
                break;

            case FsmAction.UnInit:
                ProcessMessage(this, Message.Destroy);
                break;
        }
    }

    private void Fsm_FallIntoWater(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.FallRight;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__WoodImp_Mix03);
                EngineBox originalDetectionBox = ActorModel.DetectionBox;
                SetDetectionBox(new Box(-20, originalDetectionBox.MaxY - 38, 20, originalDetectionBox.MaxY));
                break;

            case FsmAction.Step:
                // Play falling animations
                if (IsActionFinished)
                {
                    ActionId = ActionId switch
                    {
                        Action.FallRight => Action.FallIntoWater1,
                        Action.FallIntoWater1 => Action.FallIntoWater2,
                        Action.FallIntoWater2 => Action.FallIntoWater3,
                        _ => ActionId
                    };
                }

                if (Scene.GetPhysicalType(Position + new Vector2(0, -16)) == PhysicalTypeValue.Water)
                    State.MoveTo(Fsm_LandInWater);
                break;

            case FsmAction.UnInit:
                WaterSplash waterSplash = Scene.CreateProjectile<WaterSplash>(ActorType.WaterSplash);
                if (waterSplash != null)
                    waterSplash.Position = Position;

                waterSplash = Scene.CreateProjectile<WaterSplash>(ActorType.WaterSplash);
                if (waterSplash != null)
                    waterSplash.Position = Position - new Vector2(16, 0);

                waterSplash = Scene.CreateProjectile<WaterSplash>(ActorType.WaterSplash);
                if (waterSplash != null)
                    waterSplash.Position = Position + new Vector2(16, 0);
                break;
        }
    }

    private void Fsm_LandInWater(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.LandInWater;

                BarrelSplash = Scene.CreateProjectile<BarrelSplash>(ActorType.BarrelSplash);
                if (BarrelSplash != null)
                    BarrelSplash.Position = new Vector2(Position.X + 4, InitialWaterPosition.Y - 10);

                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BigSplsh_SplshGen_Mix04);
                break;

            case FsmAction.Step:
                if (BarrelSplash != null)
                    BarrelSplash.Position = Position;

                if (IsActionFinished)
                    State.MoveTo(Fsm_WaitInWater);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_WaitInWater(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.IdleFloat;
                InitialWaterPosition = Position;
                MechModel.Speed = new Vector2(0, MechModel.Speed.Y);
                Timer = 0;
                break;

            case FsmAction.Step:
                bool linkedMovement = false;
                if (Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor != this)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_LinkMovement, this);
                    linkedMovement = true;
                }
                else if (!Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor == this)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_UnlinkMovement, this);
                }

                Timer++;

                if (BarrelSplash != null)
                    BarrelSplash.Position = new Vector2(Position.X + 4, InitialWaterPosition.Y - 10);

                if (linkedMovement)
                    State.MoveTo(Fsm_MoveForwardInWater);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveForwardInWater(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = MoveRight ? Action.FloatRight : Action.FloatLeft;
                break;

            case FsmAction.Step:
                bool hasMainActorMovedBack;
                Vector2 physicalPos;

                if (MoveRight)
                {
                    physicalPos = Position + new Vector2(64, -20);
                    hasMainActorMovedBack = Scene.MainActor.Position.X < InitialWaterPosition.X - 48;
                }
                else
                {
                    physicalPos = Position + new Vector2(-48, -20);
                    hasMainActorMovedBack = Scene.MainActor.Position.X > InitialWaterPosition.X + 48;
                }

                if (Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor != this)
                    Scene.MainActor.ProcessMessage(this, Message.Main_LinkMovement, this);
                else if (!Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor == this)
                    Scene.MainActor.ProcessMessage(this, Message.Main_UnlinkMovement, this);

                Timer++;

                if (BarrelSplash != null)
                    BarrelSplash.Position = new Vector2(Position.X + 4, InitialWaterPosition.Y - 10);

                if (Scene.GetPhysicalType(physicalPos).IsSolid)
                {
                    State.MoveTo(Fsm_StopMoving);
                    return;
                }
                
                if (hasMainActorMovedBack)
                {
                    MoveRight = !MoveRight;
                    State.MoveTo(Fsm_MoveBackwardsInWater);
                    return;    
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveBackwardsInWater(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = MoveRight ? Action.FloatRight : Action.FloatLeft;
                break;

            case FsmAction.Step:
                Vector2 physicalPos;
                if (MoveRight)
                    physicalPos = Position + new Vector2(78, -16);
                else
                    physicalPos = Position + new Vector2(-78, -16);

                if (Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor != this)
                    Scene.MainActor.ProcessMessage(this, Message.Main_LinkMovement, this);
                else if (!Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor == this)
                    Scene.MainActor.ProcessMessage(this, Message.Main_UnlinkMovement, this);

                Timer++;

                if (BarrelSplash != null)
                    BarrelSplash.Position = new Vector2(Position.X + 4, InitialWaterPosition.Y - 10);

                if (Scene.GetPhysicalType(physicalPos).IsSolid)
                {
                    MoveRight = !MoveRight;
                    State.MoveTo(Fsm_WaitInWater);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_StopMoving(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.IdleFloat;
                MechModel.Speed = new Vector2(0, MechModel.Speed.Y);
                EngineBox originalDetectionBox = ActorModel.DetectionBox;
                SetDetectionBox(new Box(-20, originalDetectionBox.MaxY - 38, 20, originalDetectionBox.MaxY));
                Timer = 0;
                InitialHitPoints = 0;
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor != this)
                    Scene.MainActor.ProcessMessage(this, Message.Main_LinkMovement, this);
                else if (!Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor == this)
                    Scene.MainActor.ProcessMessage(this, Message.Main_UnlinkMovement, this);

                Timer++;

                // Sink
                if (Timer < 180)
                {
                    MechModel.Speed = new Vector2(MechModel.Speed.X, 0.125f);
                    InitialHitPoints++;

                    if (InitialHitPoints == 60)
                    {
                        InitialHitPoints = 0;

                        if (AnimatedObject.IsFramed)
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LavaBubl_Mix02);
                    }
                }
                // Finished sinking
                else
                {
                    BarrelSplash?.ProcessMessage(this, Message.Destroy);

                    WaterSplash waterSplash = Scene.CreateProjectile<WaterSplash>(ActorType.WaterSplash);
                    if (waterSplash != null)
                        waterSplash.Position = Position - new Vector2(0, 48);

                    if (Scene.MainActor.LinkedMovementActor == this)
                        Scene.MainActor.ProcessMessage(this, Message.Main_UnlinkMovement, this);

                    ProcessMessage(this, Message.Destroy);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}