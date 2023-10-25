using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.Engine2d;

public class ActorMovementState
{
    public Action Action { get; set; }
    public Vector2 Speed { get; set; }
    public Vector2 Acceleration { get; set; }
    public Vector2 Max { get; set; }
}