namespace OnyxCs.Gba.Engine2d;

public abstract class GameObject : Object
{
    protected GameObject(int id, Scene2D scene, GameObjectResource gameObjectResource)
    {
        Id = id;
        Scene = scene;
        Position = gameObjectResource.Pos.ToVector2();

        IsEnabled = gameObjectResource.IsEnabled;
        IsAwake = gameObjectResource.IsAwake;
        Flag_2 = false;
        IsSpawnable = gameObjectResource.IsSpawnable;
        ResurrectsImmediately = gameObjectResource.ResurrectsImmediately;
        ResurrectsLater = gameObjectResource.ResurrectsLater;
        Flag_6 = gameObjectResource.Flag_6;
        Flag_7 = gameObjectResource.Flag_7;
    }

    public int Id { get; }
    public Scene2D Scene { get; }
    public Vector2 Position { get; set; }

    // Flags
    public bool IsEnabled { get; set; }
    public bool IsAwake { get; set; }
    public bool Flag_2 { get; set; }
    public bool IsSpawnable { get; set; } // True for all always actors except the main actor(s)
    public bool ResurrectsImmediately { get; set; }
    public bool ResurrectsLater { get; set; }
    public bool Flag_6 { get; set; }
    public bool Flag_7 { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        switch (message)
        {
            case Message.None:
                return true;

            case Message.WakeUp:
                IsAwake = true;
                return true;

            case Message.Sleep:
                IsAwake = false;
                return true;

            case Message.Destroy:
                IsEnabled = false;
                return true;

            case Message.Resurrect:
                IsEnabled = true;
                return true;

            case Message.ResurrectWakeUp:
                IsEnabled = true;
                IsAwake = true;
                return true;

            default:
                return false;
        }
    }
}