using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Piranha
{
    private void Fsm_Wait(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Position = InitPos;
                ActionId = IsFacingRight ? Action.Dying1_Right : Action.Dying1_Left;
                Timer = 0;
                ShouldDraw = false;
                break;

            case FsmAction.Step:
                Timer++;
                
                if (Scene.IsDetectedMainActor(this) && Timer > 120)
                    Fsm.ChangeAction(Fsm_Move);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Move(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ShouldDraw = true;
                ActionId = IsFacingRight ? Action.Move_Right : Action.Move_Left;
                SpawnSplash();
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                {
                    SpawnSplash();
                }
                else
                {
                    if (Scene.IsHitMainActor(this))
                        Scene.MainActor.ReceiveDamage(AttackPoints);
                }

                if (HitPoints == 0)
                    Fsm.ChangeAction(Fsm_Dying);
                else if (IsActionFinished)
                    Fsm.ChangeAction(Fsm_Wait);   
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
                ActionId = IsFacingRight ? Action.Dying1_Right : Action.Dying1_Left;
                break;

            case FsmAction.Step:
                // TODO: Implement
                break;

            case FsmAction.UnInit:
                SpawnSplash();
                ProcessMessage(Message.Destroy);
                break;
        }
    }
}