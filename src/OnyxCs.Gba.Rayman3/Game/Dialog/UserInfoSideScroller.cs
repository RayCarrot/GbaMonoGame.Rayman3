using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public class UserInfoSideScroller : Dialog
{
    public UserInfoSideScroller(bool hasBlueLum)
    {
        Fsm = () => { };

        LifeBar = new LifeBar();

        if (GameInfo.MapId == MapId.LEVEL_1000LUMS)
        {
            // TODO: 1000 lums bar
        }
        else
        {
            LumsBar = new LumsBar();
        }

        CagesBar = new CagesBar();
    }

    public LifeBar LifeBar { get; }
    public LumsBar LumsBar { get; }
    public CagesBar CagesBar { get; }

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