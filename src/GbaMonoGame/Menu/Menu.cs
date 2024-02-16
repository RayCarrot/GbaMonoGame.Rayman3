namespace GbaMonoGame;

public abstract class Menu
{
    public virtual void OnExit() { }
    public abstract void Update(MenuManager menu);
}