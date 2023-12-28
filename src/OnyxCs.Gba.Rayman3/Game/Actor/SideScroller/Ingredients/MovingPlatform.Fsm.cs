﻿using System;
using BinarySerializer.Onyx.Gba.Rayman3;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public partial class MovingPlatform
{
    private bool FsmStep_UpdatePreviousValidDirectionalType()
    {
        PhysicalType type = Scene.GetPhysicalType(Position);

        if (IsDirectionalType(type))
            PreviousValidDirectionalType = type;

        return true;
    }

    private void Fsm_Move(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Action value = DirectionalTypeToMoveAction(CurrentDirectionalType);
                if (ActionId == Action.Impact)
                {
                    if (ActionAfterImpact != value)
                    {
                        ActionAfterImpact = value;
                        int frame = AnimatedObject.CurrentFrame;
                        ActionId = value;
                        ChangeAction();
                        ActionId = Action.Impact;
                        if (frame < AnimatedObject.GetAnimation().FramesCount)
                            AnimatedObject.CurrentFrame = frame;
                    }
                }
                else
                {
                    if (ActionId != value)
                        ActionId = value;
                }
                ChangeAction();
                break;

            case FsmAction.Step:
                if (!FsmStep_UpdatePreviousValidDirectionalType())
                    return;

                // Return to normal action if the impact has finished
                if (ActionId == Action.Impact && IsActionFinished)
                {
                    ActionId = ActionAfterImpact;
                    ChangeAction();
                }

                MovableActor mainActor = Scene.MainActor;

                // Link with main actor if it collides with it
                if (Scene.IsDetectedMainActor(this) && mainActor.LinkedMovementActor != this && mainActor.Position.Y <= Position.Y)
                {
                    if (InitialAction is Action.WaitForContact or Action.WaitForContactWithReturn)
                        IsActivated = true;

                    mainActor.ProcessMessage(Message.Main_LinkMovement, this);

                    // Check for impact if the main actor is moving downwards
                    if (mainActor.Speed.Y > 0)
                    {
                        ActionAfterImpact = ActionId;
                        ActionId = Action.Impact;
                        ChangeAction();
                        SoundManager.Play(Rayman3SoundEvent.Play__VibraFLW_Mix02);
                    }
                }
                // Unlink from main actor if no longer colliding
                else if (mainActor.LinkedMovementActor == this)
                {
                    if (!Scene.IsDetectedMainActor(this) || mainActor.Position.Y > Position.Y)
                    {
                        mainActor.ProcessMessage(Message.Main_UnlinkMovement, this);
                    }
                }

                PhysicalType type = Scene.GetPhysicalType(Position);

                // Check for rotational type
                if (PreviousPhysicalType != type && IsRotationalType(type))
                {
                    PhysicalType newType = Rotate(type);
                    CurrentDirectionalType = newType;
                    PreviousValidDirectionalType = newType;
                }
                else
                {
                    CurrentDirectionalType = type;
                }

                PreviousPhysicalType = type;

                // If the new type if empty then we keep the last direction
                if (CurrentDirectionalType == PhysicalTypeValue.None)
                    CurrentDirectionalType = PreviousValidDirectionalType;

                if (InitialAction == Action.WaitForProximity)
                {
                    if (Scene.IsDetectedMainActor(this))
                    {
                        IsActivated = true;
                    }
                    else if (mainActor.Position.X >= Position.X - 100 &&
                             Position.X + 100 >= mainActor.Position.X &&
                             mainActor.Position.Y >= Position.Y &&
                             Position.Y + 200 >= mainActor.Position.Y)
                    {
                        IsActivated = true;
                    }
                }

                // Burn up if we stop moving
                if (CurrentDirectionalType == PhysicalTypeValue.MovingPlatform_Stop && Fire == null)
                {
                    Fire = (FlowerFire)Scene.GameObjects.SpawnActor(ActorType.FlowerFire);
                    AnimatedObject.CurrentAnimation = 5;
                    SoundManager.Play(Rayman3SoundEvent.Play__BBQ_Mix10);
                    Fire?.AttachPlatform(this);
                }

                // Burn if we have a fire projectile
                if (Fire != null)
                {
                    Fsm.ChangeAction(Fsm_Burn);
                    return;
                }

                // Return platform if main actor has moved away
                if (InitialAction == Action.WaitForContactWithReturn && IsActivated && mainActor.Position.X < ReturnXPosition)
                {
                    PreviousPhysicalType = PhysicalTypeValue.None;
                    Fsm.ChangeAction(Fsm_MoveReverse);
                    return;
                }

                if (IsDirectionalType(CurrentDirectionalType) && IsActivated)
                {
                    Fsm.ChangeAction(Fsm_Move);
                    return;
                }

                if (CurrentDirectionalType == PhysicalTypeValue.MovingPlatform_FullStop)
                {
                    Fsm.ChangeAction(Fsm_Stop);
                    return;
                }
                break;
            
            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveReverse(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Action value = DirectionalTypeToMoveAction(CurrentDirectionalType);
                value = ReverseDirection(value);
                if (ActionId == Action.Impact)
                {
                    if (ActionAfterImpact != value)
                    {
                        ActionAfterImpact = value;
                        int frame = AnimatedObject.CurrentFrame;
                        ActionId = value;
                        ChangeAction();
                        ActionId = Action.Impact;
                        if (frame < AnimatedObject.GetAnimation().FramesCount)
                            AnimatedObject.CurrentFrame = frame;
                    }
                }
                else
                {
                    if (ActionId != value)
                        ActionId = value;
                }
                ChangeAction();
                break;

            case FsmAction.Step:
                if (!FsmStep_UpdatePreviousValidDirectionalType())
                    return;

                // Return to normal action if the impact has finished
                if (ActionId == Action.Impact && IsActionFinished)
                {
                    ActionId = ActionAfterImpact;
                    ChangeAction();
                }

                MovableActor mainActor = Scene.MainActor;
                bool collided = false;

                // Link with main actor if it collides with it
                if (Scene.IsDetectedMainActor(this) && mainActor.LinkedMovementActor != this && mainActor.Position.Y <= Position.Y)
                {
                    if (InitialAction is Action.WaitForContact or Action.WaitForContactWithReturn)
                        IsActivated = true;

                    mainActor.ProcessMessage(Message.Main_LinkMovement, this);
                    collided = true;

                    // Check for impact if the main actor is moving downwards
                    if (mainActor.Speed.Y > 0)
                    {
                        ActionAfterImpact = ActionId;
                        ActionId = Action.Impact;
                        ChangeAction();
                        SoundManager.Play(Rayman3SoundEvent.Play__VibraFLW_Mix02);
                    }
                }
                // Unlink from main actor if no longer colliding
                else if (mainActor.LinkedMovementActor == this)
                {
                    if (!Scene.IsDetectedMainActor(this) || mainActor.Position.Y > Position.Y)
                    {
                        mainActor.ProcessMessage(Message.Main_UnlinkMovement, this);
                    }
                }

                PhysicalType type = Scene.GetPhysicalType(Position);

                // Check for rotational type
                if (PreviousPhysicalType != type && IsRotationalType(type))
                {
                    PhysicalType newType = Rotate(type);
                    CurrentDirectionalType = newType;
                    PreviousValidDirectionalType = newType;
                }
                else
                {
                    CurrentDirectionalType = type;
                }

                PreviousPhysicalType = type;

                // Reverse movement if finished
                if (CurrentDirectionalType == PhysicalTypeValue.MovingPlatform_FullStop)
                    Position -= Speed;

                // Finished reversing
                if (collided || CurrentDirectionalType == PhysicalTypeValue.MovingPlatform_FullStop)
                {
                    if (!collided)
                        IsActivated = false;

                    CurrentDirectionalType = PhysicalTypeValue.MovingPlatform_Stop;
                    PreviousPhysicalType = PhysicalTypeValue.None;
                    Fsm.ChangeAction(Fsm_Move);
                    return;
                }

                if (IsDirectionalType(CurrentDirectionalType) && IsActivated)
                {
                    Fsm.ChangeAction(Fsm_MoveReverse);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Stop(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (ActionId == Action.Impact && IsActionFinished)
                    ActionAfterImpact = Action.Stationary;
                else
                    ActionId = Action.Stationary;
                ChangeAction();
                break;

            case FsmAction.Step:
                if (ActionId == Action.Impact && IsActionFinished)
                {
                    ActionId = ActionAfterImpact;
                    ChangeAction();
                }

                MovableActor mainActor = Scene.MainActor;

                // Link with main actor if it collides with it
                if (Scene.IsDetectedMainActor(this) && mainActor.LinkedMovementActor != this && mainActor.Position.Y <= Position.Y)
                {
                    mainActor.ProcessMessage(Message.Main_LinkMovement, this);

                    // Check for impact if the main actor is moving downwards
                    if (mainActor.Speed.Y > 0)
                    {
                        ActionAfterImpact = ActionId;
                        ActionId = Action.Impact;
                        ChangeAction();
                        SoundManager.Play(Rayman3SoundEvent.Play__VibraFLW_Mix02);
                    }
                }
                // Unlink from main actor if no longer colliding
                else if (mainActor.LinkedMovementActor == this)
                {
                    if (!Scene.IsDetectedMainActor(this) || mainActor.Position.Y > Position.Y)
                    {
                        mainActor.ProcessMessage(Message.Main_UnlinkMovement, this);
                    }
                }
                break;
            
            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Burn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;
            
            case FsmAction.Step:
                PhysicalType type = Scene.GetPhysicalType(Position);

                if (PreviousPhysicalType != type && IsRotationalType(type))
                    CurrentDirectionalType = Rotate(type);
                else
                    CurrentDirectionalType = type;

                PreviousPhysicalType = type;

                if (CurrentDirectionalType == PhysicalTypeValue.MovingPlatform_FullStop)
                {
                    Mechanic.Speed = Vector2.Zero;
                }
                else if (IsDirectionalType(CurrentDirectionalType))
                {
                    // Change action while retaining the animation
                    int originalAnim = AnimatedObject.CurrentAnimation;
                    int originalFrame = AnimatedObject.CurrentFrame;
                    int originalTimer = AnimatedObject.Timer;
                    bool originalHasExecutedFrame = AnimatedObject.HasExecutedFrame;

                    Action directionalAction = DirectionalTypeToMoveAction(CurrentDirectionalType);
                    if (directionalAction != ActionId)
                        ActionId = directionalAction;
                    ChangeAction();

                    AnimatedObject.CurrentAnimation = originalAnim;
                    AnimatedObject.CurrentFrame = originalFrame;
                    AnimatedObject.Timer = originalTimer;
                    AnimatedObject.HasExecutedFrame = originalHasExecutedFrame;
                }

                MovableActor mainActor = Scene.MainActor;

                // Link with main actor if it collides with it
                if (Scene.IsDetectedMainActor(this) && mainActor.LinkedMovementActor != this && mainActor.Position.Y <= Position.Y)
                {
                    mainActor.ProcessMessage(Message.Main_LinkMovement, this);
                }
                // Unlink from main actor if no longer colliding
                else if (mainActor.LinkedMovementActor == this)
                {
                    if (!Scene.IsDetectedMainActor(this) || mainActor.Position.Y > Position.Y)
                    {
                        mainActor.ProcessMessage(Message.Main_UnlinkMovement, this);
                    }
                }

                UpdateFire();
                break;
            
            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Respawn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Timer = 0;
                ShouldDraw = false;
                break;

            case FsmAction.Step:
                Timer++;
                if (Timer == 120)
                {
                    Position = Resource.Pos.ToVector2();
                    ActionId = Action.MoveAccelerated_Left;
                    ShouldDraw = true;
                }
                else if (Timer == 121)
                {
                    // TODO: If anim is framed! Implement frame bool and use like game does.
                    SoundManager.Play(Rayman3SoundEvent.Play__Appear_SocleFX1_Mix01);
                }

                if (IsActionFinished && ActionId == Action.MoveAccelerated_Left)
                    Setup();
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveAccelerated(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Action value = DirectionalTypeToMoveAccelerated(CurrentDirectionalType);
                if (ActionId == Action.Impact)
                {
                    if (ActionAfterImpact != value)
                    {
                        ActionAfterImpact = value;
                        int frame = AnimatedObject.CurrentFrame;
                        ActionId = value;
                        ChangeAction();
                        ActionId = Action.Impact;
                        if (frame < AnimatedObject.GetAnimation().FramesCount)
                            AnimatedObject.CurrentFrame = frame;
                        PlatformSpeed = 0;
                        IsAccelerating = true;
                    }
                }
                else
                {
                    if (ActionId != value)
                    {
                        ActionId = value;
                        PlatformSpeed = 0;
                        IsAccelerating = true;
                    }
                }
                ChangeAction();
                break;
            
            case FsmAction.Step:
                Action currentActionId = ActionId == Action.Impact ? ActionAfterImpact : ActionId;

                if (currentActionId is Action.MoveAccelerated_Left or Action.MoveAccelerated_Right)
                    Mechanic.Speed = new Vector2(PlatformSpeed, Mechanic.Speed.Y);
                else
                    Mechanic.Speed = new Vector2(Mechanic.Speed.X, PlatformSpeed);

                if (ActionId == Action.Impact && IsActionFinished)
                {
                    ActionId = ActionAfterImpact;
                    ChangeAction();
                }

                MovableActor mainActor = Scene.MainActor;

                // Link with main actor if it collides with it
                if (Scene.IsDetectedMainActor(this) && mainActor.LinkedMovementActor != this && mainActor.Position.Y <= Position.Y)
                {
                    mainActor.ProcessMessage(Message.Main_LinkMovement, this);

                    // Check for impact if the main actor is moving downwards
                    if (mainActor.Speed.Y > 0)
                    {
                        ActionAfterImpact = ActionId;
                        ActionId = Action.Impact;
                        ChangeAction();
                        SoundManager.Play(Rayman3SoundEvent.Play__VibraFLW_Mix02);
                    }
                }
                // Unlink from main actor if no longer colliding
                else if (mainActor.LinkedMovementActor == this)
                {
                    if (!Scene.IsDetectedMainActor(this) || mainActor.Position.Y > Position.Y)
                    {
                        mainActor.ProcessMessage(Message.Main_UnlinkMovement, this);
                    }
                }

                CurrentDirectionalType = Scene.GetPhysicalType(Position);

                if (IsAccelerating)
                {
                    if (Math.Abs(PlatformSpeed) < 1)
                        PlatformSpeed += AcceleratedInfos[currentActionId].Acceleration;
                }
                else
                {
                    if (Math.Abs(PlatformSpeed) > 0.0625)
                        PlatformSpeed -= AcceleratedInfos[currentActionId].Deceleration;
                }

                if (IsAccelerating)
                {
                    PhysicalType type = Scene.GetPhysicalType(Position + AcceleratedInfos[currentActionId].DecelerationDistance);
                    if (IsDirectionalType(type))
                        IsAccelerating = false;
                }

                if (CurrentDirectionalType == AcceleratedInfos[currentActionId].NextType)
                {
                    Fsm.ChangeAction(Fsm_MoveAccelerated);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}