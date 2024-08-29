namespace GbaMonoGame;

public abstract class MenuOption
{
    protected MenuOption(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public virtual void OnExit() { }
    public virtual void Init() { }
    public virtual void Apply() { }
    
    public abstract void Update(MenuManager menu);
}