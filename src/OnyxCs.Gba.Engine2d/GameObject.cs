using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.Engine2d;

public class GameObject
{
    public GameObject(int id, GameObjectResource gameObjectResource)
    {
        Id = id;
        Position = gameObjectResource.Pos.ToVector2();

        IsEnabled = gameObjectResource.IsEnabled;
        Flag_1 = gameObjectResource.Flag_1;
        Flag_2 = false;
        Flag_3 = gameObjectResource.Flag_3;
        ResurrectsImmediately = gameObjectResource.ResurrectsImmediately;
        ResurrectsInKnot = gameObjectResource.ResurrectsInKnot;
        Flag_6 = gameObjectResource.Flag_6;
        Flag_7 = gameObjectResource.Flag_7;
    }

    public int Id { get; }
    public Vector2 Position { get; set; }

    // Flags
    public bool IsEnabled { get; set; }
    public bool Flag_1 { get; set; }
    public bool Flag_2 { get; set; }
    public bool Flag_3 { get; set; }
    public bool ResurrectsImmediately { get; set; }
    public bool ResurrectsInKnot { get; set; }
    public bool Flag_6 { get; set; }
    public bool Flag_7 { get; set; }

    public Rectangle GetAbsoluteBox(Rectangle box)
    {
        box.Offset(Position); // TODO: This casts floats to int - a problem?
        return box;
    }

    public void SendMessage(Message message) => ProcessMessage(message, null);
    public void SendMessage(Message message, object param) => ProcessMessage(message, param);

    protected virtual bool ProcessMessage(Message message, object param)
    {
        switch (message)
        {
            case Message.None:
                return true;

            case Message.SetFlag1:
                Flag_1 = true;
                return true;

            case Message.ClearFlag1:
                Flag_1 = false;
                return true;

            case Message.Disable:
                IsEnabled = false;
                return true;

            case Message.Enable:
                IsEnabled = true;
                return true;

            case Message.Spawn:
                IsEnabled = true;
                Flag_1 = true;
                return true;

            default:
                return false;
        }
    }
}