using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class KegFire
{
    private bool Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Default_Right;
                break;

            case FsmAction.Step:
                InteractableActor hitKeg = Scene.IsHitActorOfType(this, (int)ActorType.Keg);
                hitKeg?.ProcessMessage(this, IsFacingRight ? Message.LightOnFire_Right : Message.LightOnFire_Left);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}