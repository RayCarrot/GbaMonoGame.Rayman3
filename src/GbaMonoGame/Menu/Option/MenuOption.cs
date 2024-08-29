namespace GbaMonoGame;

public abstract class MenuOption
{
    protected MenuOption(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public virtual void OnExit() { }

    public abstract void Init();
    public abstract void Apply();
    public abstract void Update(MenuManager menu);
}