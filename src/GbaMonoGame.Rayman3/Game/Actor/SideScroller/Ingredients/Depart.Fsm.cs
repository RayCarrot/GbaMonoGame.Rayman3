﻿namespace GbaMonoGame.Rayman3;

public partial class Depart
{
    private bool Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this))
                {
                    Scene.MainActor.ProcessMessage(this, MessageToSend);
                    State.MoveTo(null);
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