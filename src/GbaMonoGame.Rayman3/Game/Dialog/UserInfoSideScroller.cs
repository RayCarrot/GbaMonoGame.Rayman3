using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class UserInfoSideScroller : Dialog
{
    public UserInfoSideScroller(Scene2D scene, bool hasBlueLum)
    {
        LifeBar = new LifeBar(scene);

        if (GameInfo.MapId == MapId._1000Lums)
            Lums1000Bar = new Lums1000Bar();
        else
            LumsBar = new LumsBar();

        CagesBar = new CagesBar();

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
            // TODO: Boss bar
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

        if (GameInfo.MapId is
            MapId.BossMachine or
            MapId.BossBadDreams or
            MapId.BossRockAndLava or
            MapId.BossScaleMan or
            MapId.BossFinal_M1 or
            MapId.BossFinal_M2)
        {
            GetLumsBar().Mode = 3;
            CagesBar.Mode = 3;
        }
    }

    private LifeBar LifeBar { get; }
    private LumsBar LumsBar { get; }
    private Lums1000Bar Lums1000Bar { get; }
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

    public void HideBars()
    {
        if (LifeBar.Mode != 3)
            LifeBar.Mode = 1;
        
        Bar lumsBar = GetLumsBar();
        if (lumsBar.Mode != 3)
            lumsBar.Mode = 1;

        if (CagesBar.Mode != 3)
            CagesBar.Mode = 1;
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

        LifeBar.Set();
        GetLumsBar().Set();
        CagesBar.Set();
    }

    public override void Init() { }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        LifeBar.Draw(animationPlayer);
        GetLumsBar().Draw(animationPlayer);
        CagesBar.Draw(animationPlayer);
    }
}