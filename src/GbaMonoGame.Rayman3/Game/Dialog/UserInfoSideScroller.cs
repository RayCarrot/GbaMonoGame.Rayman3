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

    public override void ProcessMessage()
    {
        // TODO: Implement
    }

    public override void Load()
    {
        LifeBar.Init();
        LumsBar?.Init();
        CagesBar.Init();

        LifeBar.Load();
        LumsBar?.Load();
        CagesBar.Load();
    }

    public override void Init() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        LifeBar.Draw(animationPlayer);
        LumsBar?.Draw(animationPlayer);
        CagesBar.Draw(animationPlayer);
    }
}