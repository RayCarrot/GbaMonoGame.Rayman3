using System;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public abstract class CameraActor2D : CameraActor
{
    public override bool IsActorFramed(BaseActor actor, Vector2 screenPosition)
    {
        throw new NotImplementedException();
    }
}