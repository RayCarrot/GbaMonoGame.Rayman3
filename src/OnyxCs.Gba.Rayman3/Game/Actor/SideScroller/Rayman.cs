using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public class Rayman : BaseActor
{
    public Rayman(ActorResource actorResource)
    {
        AnimatedObject = new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic);
        AnimatedObject.SetCurrentAnimation(64);
        Position = new Vector2(actorResource.Pos.X, actorResource.Pos.Y);
    }
}