using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// TODO: Better name, alongside BaseCameraMode7
public abstract class CameraActor2D : CameraActor
{
    protected CameraActor2D(Scene2D scene) : base(scene) { }

    public override bool IsActorFramed(BaseActor actor)
    {
        Box viewBox = actor.GetViewBox();
        Box camBox = new(Scene.Playfield.Camera.Position, Scene.Playfield.Camera.Resolution);

        bool isFramed = viewBox.Intersects(camBox);

        if (isFramed)
            actor.AnimatedObject.ScreenPos = actor.Position - Scene.Playfield.Camera.Position;

        return isFramed;
    }
}