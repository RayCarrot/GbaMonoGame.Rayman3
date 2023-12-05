namespace OnyxCs.Gba.Rayman3;

public class Act5 : Act
{
    public override void Init()
    {
        Init(Engine.Loader.Act5);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
            FrameManager.SetNextFrame(new FrameSideScroller(MapId.PirateShip_M1));
    }
}