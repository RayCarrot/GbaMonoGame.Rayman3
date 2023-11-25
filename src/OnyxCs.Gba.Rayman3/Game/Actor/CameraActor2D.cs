using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public abstract class CameraActor2D : CameraActor
{
    protected CameraActor2D(Scene2D scene) : base(scene) { }

    public override bool IsActorFramed(BaseActor actor)
    {
        actor.AnimatedObject.ScreenPos = actor.Position - Scene.Playfield.Camera.Position;
        return true;
        //throw new NotImplementedException();
    }
}