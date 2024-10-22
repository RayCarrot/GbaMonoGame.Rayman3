using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Engine2d;

public abstract class InteractableActor : ActionActor
{
    protected InteractableActor(int instanceId, Scene2D scene, ActorResource actorResource)
        : this(instanceId, scene, actorResource, new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic)) { }

    protected InteractableActor(int instanceId, Scene2D scene, ActorResource actorResource, AnimatedObject animatedObject)
        : base(instanceId, scene, actorResource, animatedObject)
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