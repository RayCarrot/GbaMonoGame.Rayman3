﻿using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class BreakableGround
{
    private bool Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                IsSolid = true;
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this))
                    Scene.MainActor.ProcessMessage(this, Message.Main_1056);

                if (Scene.IsDetectedMainActor(this) && Scene.MainActor.LinkedMovementActor != this && Scene.MainActor.Position.Y <= Position.Y)
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_LinkMovement, this);
                }
                else if (Scene.MainActor.LinkedMovementActor == this && 
                         (!Scene.IsDetectedMainActor(this) || Scene.MainActor.Position.Y > Position.Y))
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_UnlinkMovement, this);
                }

                if (ActionId == Action.Destroyed)
                {
                    State.MoveTo(Fsm_Destroyed);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                if (Scene.IsDetectedMainActor(this))
                    Scene.MainActor.ProcessMessage(this, Message.Main_UnlinkMovement, this);
                break;
        }

        return true;
    }

    private bool Fsm_Destroyed(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                IsSolid = false;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__WoodBrk1_Mix04);
                break;

            case FsmAction.Step:
                bool multiplayerFinished = false;

                if (IsActionFinished)
                {
                    if (Engine.Settings.Platform == Platform.NGage && RSMultiplayer.IsActive)
                        multiplayerFinished = true;
                    else
                        ProcessMessage(this, Message.Destroy);
                }

                if (multiplayerFinished)
                {
                    IsDestroyed = true;
                    State.MoveTo(Fsm_MultiplayerRespawn);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_MultiplayerRespawn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                bool outOfView = true;

                Box viewBox = GetViewBox();
                viewBox = new Box(
                    minX: viewBox.MinX - Scene.Resolution.X,
                    minY: viewBox.MinY - Scene.Resolution.Y,
                    maxX: viewBox.MaxX + Scene.Resolution.X,
                    maxY: viewBox.MaxY + Scene.Resolution.Y);
                
                for (int playerId = 0; playerId < RSMultiplayer.PlayersCount; playerId++)
                {
                    Box playerDetectionBox = Scene.GetGameObject<ActionActor>(playerId).GetDetectionBox();

                    if (viewBox.Intersects(playerDetectionBox))
                        outOfView = false;
                }

                if (outOfView)
                {
                    IsDestroyed = false;
                    ActionId = Action.Idle_Default;
                    State.MoveTo(Fsm_Idle);
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