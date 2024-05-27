using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Cage
{
    private void Fsm_Idle(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = InitialActionId;
                Timer = 0;
                break;
            
            case FsmAction.Step:
                Timer++;

                // Check if damaged
                if (HitPoints != PrevHitPoints)
                {
                    PrevHitPoints = HitPoints;
                    Fsm.ChangeAction(Fsm_Damaged);
                    return;
                }

                // Change idle state after 2 seconds
                if (Timer >= 120)
                {
                    Fsm.ChangeAction(Fsm_Blink);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Blink(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // If all objects are kept active we only want to make this sound when framed
                if (!Scene.KeepAllObjectsActive || AnimatedObject.IsFramed)
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__CageSnd1_Mix02__or__CageSnd2_Mix02);
                ActionId = InitialActionId + 1;
                break;

            case FsmAction.Step:
                // Check if damaged
                if (HitPoints != PrevHitPoints)
                {
                    PrevHitPoints = HitPoints;
                    Fsm.ChangeAction(Fsm_Damaged);
                    return;
                }

                // Go back to the default idle animation when finished
                if (IsActionFinished)
                {
                    Fsm.ChangeAction(Fsm_Idle);
                    return;
                }
                break;
            
            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Damaged(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__CageHit_Mix07);
                ActionId = InitialActionId + 2 + HitAction;
                PrevHitPoints = HitPoints;
                break;

            case FsmAction.Step:
                if (IsActionFinished && HitPoints != PrevHitPoints)
                {
                    PrevHitPoints = HitPoints;
                    Fsm.ChangeAction(Fsm_Destroyed);
                    return;
                }

                if (IsActionFinished)
                {
                    Fsm.ChangeAction(Fsm_IdleDamaged);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_IdleDamaged(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = InitialActionId + 3;
                break;

            case FsmAction.Step:
                if (PrevHitPoints != HitPoints)
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__CageHit_Mix07);
                    ActionId = InitialActionId + 2 + HitAction;
                    PrevHitPoints = HitPoints;
                }

                if (IsActionFinished && ActionId is 2 or 5 or 8 or 11)
                {
                    PrevHitPoints = HitPoints;
                    Fsm.ChangeAction(Fsm_Destroyed);
                    return;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Destroyed(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                GameInfo.SetCageAsCollected(CageId);
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__CageTrsh_Mix05);
                ActionId = InitialActionId + 4;
                Scene.MainActor.ProcessMessage(this, Message.Main_CollectedCage);
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                    Fsm.ChangeAction(Fsm_Idle);
                break;

            case FsmAction.UnInit:
                ProcessMessage(this, Message.Destroy);
                break;
        }
    }
}