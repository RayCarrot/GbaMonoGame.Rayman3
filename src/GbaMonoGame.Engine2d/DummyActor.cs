namespace GbaMonoGame.Engine2d;

// Temporary class
public class DummyActor : BaseActor
{
    public DummyActor(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        Logger.NotImplemented("Not implemented actor of type {0}", actorResource.Type);
    }
}