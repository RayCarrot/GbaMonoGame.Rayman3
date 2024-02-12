using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class UserInfoSideScroller : Dialog
{
    public UserInfoSideScroller(Scene2D scene, bool hasBlueLum)
    {
        LifeBar = new LifeBar(scene);

        if (GameInfo.MapId == MapId._1000Lums)
        {
            // TODO: 1000 lums bar
        }
        else
        {
            LumsBar = new LumsBar();
        }

        CagesBar = new CagesBar();
    }

    private LifeBar LifeBar { get; }
    private LumsBar LumsBar { get; }
    private CagesBar CagesBar { get; }

    public void UpdateLife()
    {
        LifeBar.UpdateLife();
    }

    public void AddLums(int count)
    {
        if (LumsBar != null)
        {
            LumsBar.AddLums(count);
        }
        else
        {
            // TODO: 1000 lums bar
        }
    }

    public void AddCages(int count)
    {
        CagesBar.AddCages(count);
    }

    public void HideBars()
    {
        if (LifeBar.Mode != 3)
            LifeBar.Mode = 1;

        if (LumsBar != null)
        {
            if (LumsBar.Mode != 3)
                LumsBar.Mode = 1;
        }
        else
        {
            // TODO: 1000 lums bar
        }

        if (CagesBar.Mode != 3)
            CagesBar.Mode = 1;
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        // TODO: Implement
        return false;
    }

    public override void Load()
    {
        LifeBar.Load();
        LumsBar?.Load();
        CagesBar.Load();

        LifeBar.Set();
        LumsBar?.Set();
        CagesBar.Set();
    }

    public override void Init() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        LifeBar.Draw(animationPlayer);
        LumsBar?.Draw(animationPlayer);
        CagesBar.Draw(animationPlayer);
    }
}