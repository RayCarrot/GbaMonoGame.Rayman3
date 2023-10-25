namespace OnyxCs.Gba.Engine2d;

public class GameObject
{
    public GameObject(int id, GameObjectResource gameObjectResource)
    {
        Id = id;
        Position = gameObjectResource.Pos.ToVector2();
    }

    public int Id { get; }
    public Vector2 Position { get; set; }

    // Flags
    public bool IsEnabled { get; set; } = true;

    public virtual bool ProcessMessage(Frame frame, Message message, object param)
    {
        switch (message)
        {
            case Message.Disable:
                IsEnabled = false;
                return true;

            case Message.Enable:
                IsEnabled = true;
                return true;

            default:
                return false;
        }
    }
}