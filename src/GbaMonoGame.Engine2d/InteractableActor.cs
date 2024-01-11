using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Engine2d;

public class InteractableActor : ActionActor
{
    public InteractableActor(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        AnimationBoxTable = new BoxTable();
        AnimatedObject.BoxTable = AnimationBoxTable;
    }

    private BoxTable AnimationBoxTable { get; }

    public virtual Box GetAttackBox()
    {
        Box box = AnimationBoxTable.AttackBox;

        if (AnimatedObject.FlipX)
            box = box.FlipX();

        if (AnimatedObject.FlipY)
            box = box.FlipY();

        return box.Offset(Position);
    }

    public virtual Box GetVulnerabilityBox()
    {
        Box box = AnimationBoxTable.VulnerabilityBox;

        if (AnimatedObject.FlipX)
            box = box.FlipX();

        if (AnimatedObject.FlipY)
            box = box.FlipY();

        return box.Offset(Position);
    }
}