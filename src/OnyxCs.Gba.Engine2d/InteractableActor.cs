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

    public Box GetAttackBox()
    {
        Box box = AnimationBoxTable.AttackBox;

        if (AnimatedObject.FlipX)
            box = box.FlipX();

        if (AnimatedObject.FlipY)
            box = box.FlipY();

        return box.Offset(Position);
    }

    public Box GetVulnerabilityBox()
    {
        Box box = AnimationBoxTable.VulnerabilityBox;

        if (AnimatedObject.FlipX)
            box = box.FlipX();

        if (AnimatedObject.FlipY)
            box = box.FlipY();

        return box.Offset(Position);
    }
}