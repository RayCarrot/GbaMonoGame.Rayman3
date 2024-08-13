using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class UserInfoSideScroller : Dialog
{
    public UserInfoSideScroller(Scene2D scene, bool hasBlueLum) : base(scene)
    {
        LifeBar = new LifeBar(scene);

        if (GameInfo.MapId == MapId._1000Lums)
            Lums1000Bar = new Lums1000Bar(scene);
        else
            LumsBar = new LumsBar(scene);

        CagesBar = new CagesBar(scene);

        if (hasBlueLum)
        {
            // TODO: Blue lums bar
        }

        if (GameInfo.MapId == MapId.EchoingCaves_M1)
        {
            // TODO: Switch bar
        }

        if (GameInfo.MapId == MapId.BossMachine)
        {
            BossBar = new BossMachineBar(scene);
        }
        else if (GameInfo.MapId == MapId.BossRockAndLava)
        {
            // TODO: Boss bar
        }
        else if (GameInfo.MapId == MapId.BossScaleMan)
        {
            // TODO: Boss bar
        }
        else if (GameInfo.MapId == MapId.BossFinal_M1)
        {
            // TODO: Boss bar
        }
        else if (GameInfo.MapId == MapId.BossFinal_M2)
        {
            // TODO: Boss bar
        }

        // Disable lums and cages bars in bosses
        if (GameInfo.MapId is
            MapId.BossMachine or
            MapId.BossBadDreams or
            MapId.BossRockAndLava or
            MapId.BossScaleMan or
            MapId.BossFinal_M1 or
            MapId.BossFinal_M2)
        {
            GetLumsBar().Disable();
            CagesBar.Disable();
        }
    }

    private LifeBar LifeBar { get; }
    private LumsBar LumsBar { get; }
    private Lums1000Bar Lums1000Bar { get; }
    private Bar BossBar { get; }
    private CagesBar CagesBar { get; }

    private Bar GetLumsBar() => LumsBar != null ? LumsBar : Lums1000Bar;

    public void UpdateLife()
    {
        LifeBar.UpdateLife();
    }

    public void AddLums(int count)
    {
        if (LumsBar != null)
            LumsBar.AddLums(count);
        else
            Lums1000Bar.AddLastLums();
    }

    public void AddCages(int count)
    {
        CagesBar.AddCages(count);
    }

    public void AddBossDamage()
    {
        BossBar.Set();
    }

    public void HideBars()
    {
        LifeBar.SetToStayHidden();
        GetLumsBar().SetToStayHidden();
        CagesBar.SetToStayHidden();
    }

    public void MoveOutBars()
    {
        LifeBar.DrawStep = BarDrawStep.MoveOut;
        GetLumsBar().DrawStep = BarDrawStep.MoveOut;
        CagesBar.DrawStep = BarDrawStep.MoveOut;
    }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        // TODO: Implement
        return false;
    }

    public override void Load()
    {
        LifeBar.Load();
        GetLumsBar().Load();
        CagesBar.Load();
        // TODO: Blue lums bar
        // TODO: Switch bar
        BossBar?.Load();

        LifeBar.Set();
        GetLumsBar().Set();
        CagesBar.Set();
        // TODO: Blue lums bar
        // TODO: Switch bar
    }

    public override void Init() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        LifeBar.Draw(animationPlayer);
        GetLumsBar().Draw(animationPlayer);
        CagesBar.Draw(animationPlayer);
        // TODO: Blue lums bar
        // TODO: Switch bar
        BossBar?.Draw(animationPlayer);
    }
}