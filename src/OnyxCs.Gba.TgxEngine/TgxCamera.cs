using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxCamera
{
    //public abstract void GetPosition();
    public abstract void Move(Vec2 deltaPos);
    //public abstract void SetPosition();
}