namespace GbaMonoGame.Rayman3;

public class LumPosition
{
    public LumPosition(int instanceId, Vector2 position)
    {
        InstanceId = instanceId;
        Position = position;
    }

    public int InstanceId { get; }
    public Vector2 Position { get; }
}