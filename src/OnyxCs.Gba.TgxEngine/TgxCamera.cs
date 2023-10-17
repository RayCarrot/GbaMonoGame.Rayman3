using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxCamera
{
    //public abstract void GetPosition();
    public abstract void Move(Vector2 deltaPos);
    //public abstract void SetPosition();
}