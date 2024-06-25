using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// TODO: Better name, alongside CameraActor2D
public abstract class BaseCameraMode7 : CameraActor
{
    protected BaseCameraMode7(Scene2D scene) : base(scene) { }

    public override bool IsActorFramed(BaseActor actor)
    {
        // TODO: Implement
        return true;
    }
}