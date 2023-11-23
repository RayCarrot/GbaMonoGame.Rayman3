using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public abstract class CameraActor2D : CameraActor
{
    public override bool IsActorFramed(BaseActor actor)
    {
        actor.AnimatedObject.ScreenPos = actor.Position - Frame.GetComponent<TgxPlayfield2D>().Camera.Position;
        return true;
        //throw new NotImplementedException();
    }
}