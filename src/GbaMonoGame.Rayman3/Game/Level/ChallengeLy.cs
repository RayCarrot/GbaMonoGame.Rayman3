using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class ChallengeLy : FrameSideScroller
{
    public ChallengeLy(MapId mapId) : base(mapId) { }

    private int Timer { get; set; }

    public override void Init()
    {
        Timer = 0;
        base.Init();

        if (GameInfo.MapId != MapId.ChallengeLyGCN)
            Scene.AddDialog(new TextBoxDialog(), false, false);

        GameInfo.RemainingTime = GameInfo.MapId != MapId.ChallengeLyGCN ? 4200 : 3900;
        UserInfo.HideBars();
    }

    public override void Step()
    {
        base.Step();

        if (Timer <= 120)
            Timer++;

        // Wait 1 second before starting the timer
        if (Timer == 60)
            IsTimed = true;

        if (GameInfo.RemainingTime == 0)
            Scene.MainActor.ProcessMessage((Message)1060); // TODO: Name and implement
    }
}