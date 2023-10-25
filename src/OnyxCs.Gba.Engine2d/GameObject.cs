using Microsoft.Xna.Framework;

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

    public Rectangle GetAbsoluteBox(Rectangle box)
    {
        box.Offset(Position);
        return box;
    }

    public void SendMessage(Message message) => ProcessMessage(message, null);
    public void SendMessage(Message message, object param) => ProcessMessage(message, param);

    protected virtual bool ProcessMessage(Message message, object param)
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