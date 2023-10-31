using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxCamera
{
    protected TgxCamera()
    {
        Frame.RegisterComponent(this);
    }

    public abstract Vector2 Position { get; set; }
}