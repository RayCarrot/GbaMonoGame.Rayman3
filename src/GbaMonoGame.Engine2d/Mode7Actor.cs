namespace GbaMonoGame.Engine2d;

public abstract class Mode7Actor : MovableActor
{
    protected Mode7Actor(int instanceId, Scene2D scene, ActorResource actorResource)
        : this(instanceId, scene, actorResource, new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic)) { }

    protected Mode7Actor(int instanceId, Scene2D scene, ActorResource actorResource, AnimatedObject animatedObject)
        : base(instanceId, scene, actorResource, animatedObject)
    {
        IsAffine = true;
        Direction = 0;
        field_0x60 = 0;
        field_0x63 = 32;
        AnimatedObject.SpritePriority = 0;
    }

    public short field_0x60 { get; set; }
    public bool IsAffine { get; set; }
    public byte field_0x63 { get; set; }
    public byte Direction { get; set; }
    public float CamAngle { get; set; }
}