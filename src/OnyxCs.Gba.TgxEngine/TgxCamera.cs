using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxCamera
{
    public abstract Vector2 Position { get; set; }
    public abstract void Move(Vector2 deltaPos);
}