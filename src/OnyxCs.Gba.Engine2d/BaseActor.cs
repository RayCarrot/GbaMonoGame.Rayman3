using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Engine2d;

public class BaseActor : GameObject
{
    public AnimatedObject AnimatedObject { get; set; }

    public virtual void Init()
    {

    }

    public virtual void Draw(AnimationPlayer animationPlayer)
    {
        if (AnimatedObject == null)
            return;

        AnimatedObject.ScreenPos = Position - ((TgxPlayfield2D)TgxPlayfield.CurrentPlayfield).Camera.Position;
        animationPlayer.AddObject1(AnimatedObject);
    }
}