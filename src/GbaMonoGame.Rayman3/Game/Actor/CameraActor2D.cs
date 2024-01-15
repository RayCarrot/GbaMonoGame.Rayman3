using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public abstract class CameraActor2D : CameraActor
{
    protected CameraActor2D(Scene2D scene) : base(scene) { }

    public override bool IsActorFramed(BaseActor actor)
    {
        // NOTE: The game only does this if it is framed, but we do it anyways
        //       as to make debugging the game easier.
        actor.AnimatedObject.ScreenPos = actor.Position - Scene.Playfield.Camera.Position;

        Box viewBox = actor.GetViewBox();
        Box camBox = new(Scene.Playfield.Camera.Position, Scene.Playfield.Camera.Resolution);

        return viewBox.Intersects(camBox);
    }
}