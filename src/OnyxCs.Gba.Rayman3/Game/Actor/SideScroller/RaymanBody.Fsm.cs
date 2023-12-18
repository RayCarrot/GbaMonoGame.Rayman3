﻿namespace OnyxCs.Gba.Rayman3;

public partial class RaymanBody
{
    private void Fsm_Wait(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = 0;
                break;

            case FsmAction.Step:
                if (BodyPartType == 4)
                {
                    // TODO: Implement
                    // Fsm.ChangeAction(FUN_08034458);
                }
                else if (BodyPartType != 0)
                {
                    // TODO: Implement
                    // Fsm.ChangeAction(FUN_08034010);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}