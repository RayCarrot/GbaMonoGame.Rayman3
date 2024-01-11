namespace GbaMonoGame.Rayman3;

public class Act5 : Act
{
    public override void Init()
    {
        Init(Engine.Loader.Rayman3_Act5);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
            FrameManager.SetNextFrame(new FrameSideScroller(MapId.PirateShip_M1));
    }
}