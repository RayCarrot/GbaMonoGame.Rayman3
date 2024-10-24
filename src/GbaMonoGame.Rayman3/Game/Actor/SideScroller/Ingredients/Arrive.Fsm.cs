﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Arrive
{
    private bool Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Idle;
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this))
                {
                    Scene.MainActor.ProcessMessage(this, Message.Main_LevelEnd);
                    State.MoveTo(Fsm_EndLevel);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_IdleWithLink(FsmAction action)
    {
        // TODO: Implement
        switch (action)
        {
            case FsmAction.Init:

                break;

            case FsmAction.Step:

                break;

            case FsmAction.UnInit:

                break;
        }

        return true;
    }

    private bool Fsm_EndLevel(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.EndingLevel;
                break;

            case FsmAction.Step:
                if (IsActionFinished && ActionId == Action.EndingLevel)
                {
                    if (Engine.Settings.Platform == Platform.GBA)
                    {
                        if (GameInfo.MapId == MapId.ChallengeLyGCN)
                            Scene.MainActor.ProcessMessage(this, Message.Main_LevelEnd);
                    }
                    else if (Engine.Settings.Platform == Platform.NGage)
                    {
                        if (GameInfo.MapId is MapId.ChallengeLy1 or MapId.ChallengeLy2 or MapId.ChallengeLyGCN)
                            Scene.MainActor.ProcessMessage(this, Message.Main_LevelEnd);
                    }
                    else
                    {
                        throw new UnsupportedPlatformException();
                    }

                    ActionId = Action.EndedLevel;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}