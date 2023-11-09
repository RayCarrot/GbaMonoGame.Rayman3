using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Engine2d;

public class InteractableActor : ActionActor
{
    public InteractableActor(int id, ActorResource actorResource) : base(id, actorResource)
    {
        AnimationBoxTable = new BoxTable();
        AnimatedObject.BoxTable = AnimationBoxTable;
    }

    private BoxTable AnimationBoxTable { get; }

    public Box GetAttackBox() => AnimationBoxTable.AttackBox.Offset(Position);
    public Box GetVulnerabilityBox() => AnimationBoxTable.VulnerabilityBox.Offset(Position);
}