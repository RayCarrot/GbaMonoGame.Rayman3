using System;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class RaymanBody
{
    private bool FsmStep_CheckCollision()
    {
        // Check for actor collision
        InteractableActor hitActor = Scene.IsHitActor(this);

        if ((hitActor != null && hitActor != Rayman && hitActor != HitActor) || IsTouchingMap)
        {
            if (hitActor != null && hitActor != Rayman && hitActor != HitActor)
            {
                HitActor = hitActor;
                
                int damage;
                if (BodyPartType is RaymanBodyPartType.Fist or RaymanBodyPartType.Foot)
                    damage = 2;
                else if (BodyPartType is RaymanBodyPartType.SecondFist)
                    damage = 3;
                else
                    damage = 5;

                hitActor.ReceiveDamage(damage);
                hitActor.ProcessMessage(Message.Hit);
                SpawnHitEffect();
            }

            if (ActionId != BaseActionId + 3 && ActionId != BaseActionId + 4)
            {
                if (IsTouchingMap)
                    SpawnHitEffect();

                if (BodyPartType == RaymanBodyPartType.Torso)
                {
                    // TODO: Implement
                    //if (hitActor != null && hitActor.Type == (int)ActorType.BreakableGround)
                    //{

                    //}
                }
                else
                {
                    ActionId = BaseActionId + (IsFacingRight ? 4 : 3);
                    Fsm.ChangeAction(Fsm_MoveBackwards);
                    return false;
                }
            }
        }

        return true;
    }

    private void Fsm_Wait(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = 0;
                break;

            case FsmAction.Step:
                if (BodyPartType == RaymanBodyPartType.HitEffect)
                    Fsm.ChangeAction(Fsm_HitEffect);
                else if (ActionId != 0)
                    Fsm.ChangeAction(Fsm_MoveForward);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveForward(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (BodyPartType is RaymanBodyPartType.SuperFist or RaymanBodyPartType.SecondSuperFist)
                    ChargePower = 17;
                else
                    ChargePower = Math.Clamp(ChargePower / 3, 4, 16);

                if (MultiplayerManager.IsInMultiplayer)
                {
                    throw new NotImplementedException();
                }
                break;

            case FsmAction.Step:
                if (!FsmStep_CheckCollision())
                    return;

                ChargePower--;

                // Go from accelerating action to decelerating action
                if (ChargePower == 0)
                    ActionId = BaseActionId + (IsFacingRight ? 5 : 6);

                // Turn around torso
                if (BodyPartType == RaymanBodyPartType.Torso)
                {
                    if (Speed.Y < 2)
                    {
                        ActionId = IsFacingRight ? 15 : 16;
                        Fsm.ChangeAction(Fsm_MoveBackwards);
                    }
                }
                // Turn around fist or foot
                else
                {
                    if (IsFacingRight && Speed.X < 2)
                    {
                        ActionId = BaseActionId + 4;
                        Fsm.ChangeAction(Fsm_MoveBackwards);
                    }
                    else if (IsFacingLeft && Speed.X > -2)
                    {
                        ActionId = BaseActionId + 3;
                        Fsm.ChangeAction(Fsm_MoveBackwards);
                    }
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveBackwards(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                HasMapCollision = false;
                break;
            
            case FsmAction.Step:
                if (!FsmStep_CheckCollision())
                    return;

                float bodyPos1;
                float raymanPos1;
                float bodyPos2;
                float raymanPos2;
                float targetOffset1;

                if (BodyPartType == RaymanBodyPartType.Torso)
                {
                    bodyPos1 = Position.X;
                    raymanPos1 = Rayman.Position.X;
                    bodyPos2 = Position.Y;
                    raymanPos2 = Rayman.Position.Y;
                    targetOffset1 = 0;
                }
                else
                {
                    bodyPos1 = Position.Y;
                    raymanPos1 = Rayman.Position.Y;
                    bodyPos2 = Position.X;
                    raymanPos2 = Rayman.Position.X;

                    targetOffset1 = BodyPartType == RaymanBodyPartType.Foot ? 0 : 20;
                }
                
                float targetPos1 = raymanPos1 - targetOffset1;
                float speed;

                if (bodyPos1 == targetPos1)
                {
                    speed = 0;
                }
                else if (bodyPos1 < targetPos1)
                {
                    float dist = Math.Abs(raymanPos2 - bodyPos2);

                    if (dist < targetPos1 - bodyPos1)
                        speed = 7;
                    else if (3 < Rayman.Position.Y - 20 - bodyPos1)
                        speed = 4;
                    else
                        speed = 1;
                }
                else
                {
                    float dist = Math.Abs(raymanPos2 - bodyPos2);

                    if (dist < bodyPos1 - raymanPos1 - targetOffset1)
                        speed = -7;
                    else if (3 < bodyPos1 - raymanPos1 + targetOffset1)
                        speed = -4;
                    else
                        speed = -1;
                }

                if (BodyPartType == RaymanBodyPartType.Torso)
                    MechModel.Speed = new Vector2(speed, MechModel.Speed.Y);
                else
                    MechModel.Speed = new Vector2(MechModel.Speed.X, speed);

                float remainingDist;
                if (IsFacingRight && BodyPartType != RaymanBodyPartType.Torso)
                    remainingDist = raymanPos2 - bodyPos2;
                else
                    remainingDist = bodyPos2 - raymanPos2;

                if (remainingDist < 25)
                {
                    Fsm.ChangeAction(Fsm_Wait);
                    return;
                }
                break;

            case FsmAction.UnInit:
                if (BodyPartType == RaymanBodyPartType.SuperFist)
                    BodyPartType = RaymanBodyPartType.Fist;
                else if (BodyPartType == RaymanBodyPartType.SecondSuperFist)
                    BodyPartType = RaymanBodyPartType.SecondFist;
                
                Rayman.ProcessMessage(Message.RaymanBody_FinishedAttack, BodyPartType);

                HitActor = null;
                HasMapCollision = true;

                ProcessMessage(Message.Destroy);
                break;
        }
    }

    private void Fsm_HitEffect(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                    Fsm.ChangeAction(Fsm_Wait);
                break;

            case FsmAction.UnInit:
                HitActor = null;
                HasMapCollision = true;
                ProcessMessage(Message.Destroy);
                AnimatedObject.YPriority = 32;
                break;
        }
    }
}