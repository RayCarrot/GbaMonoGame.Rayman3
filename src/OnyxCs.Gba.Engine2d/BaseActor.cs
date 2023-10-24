using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Engine2d;

public class BaseActor : GameObject
{
    public AnimatedObject AnimatedObject { get; set; }

    public virtual void Init()
    {

    }
}