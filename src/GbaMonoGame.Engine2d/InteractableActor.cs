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

        _debugAttackBoxAObject = new DebugBoxAObject()
        {
            Color = DebugBoxColor.AttackBox,
            Camera = Scene.Playfield.Camera
        };
        _debugVulnerabilityBoxAObject = new DebugBoxAObject()
        {
            Color = DebugBoxColor.VulnerabilityBox,
            Camera = Scene.Playfield.Camera
        };
    }

    private readonly DebugBoxAObject _debugAttackBoxAObject;
    private readonly DebugBoxAObject _debugVulnerabilityBoxAObject;

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

    public override void DrawDebugBoxes(AnimationPlayer animationPlayer)
    {
        base.DrawDebugBoxes(animationPlayer);

        _debugAttackBoxAObject.Position = Position + AnimationBoxTable.AttackBox.Position - Scene.Playfield.Camera.Position;
        _debugAttackBoxAObject.Size = AnimationBoxTable.AttackBox.Size;
        animationPlayer.PlayFront(_debugAttackBoxAObject);

        _debugVulnerabilityBoxAObject.Position = Position + AnimationBoxTable.VulnerabilityBox.Position - Scene.Playfield.Camera.Position;
        _debugVulnerabilityBoxAObject.Size = AnimationBoxTable.VulnerabilityBox.Size;
        animationPlayer.PlayFront(_debugVulnerabilityBoxAObject);
    }
}