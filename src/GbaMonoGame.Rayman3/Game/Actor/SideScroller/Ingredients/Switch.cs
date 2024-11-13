using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Switch : InteractableActor
{
    public Switch(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Links = actorResource.Links;

        AnimatedObject.ObjPriority = 55;

        State.SetTo(Fsm_Deactivated);
    }

    public byte?[] Links { get; }

    public void SetToActivated()
    {
        State.SetTo(Fsm_Activated);
    }
}