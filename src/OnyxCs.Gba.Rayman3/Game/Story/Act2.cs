namespace OnyxCs.Gba.Rayman3;

public class Act2 : Act
{
    public override void Init()
    {
        Init(Engine.Loader.Act2);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
        {
            // TODO: Load the actual level class once we create it
            FrameManager.SetNextFrame(new DummyLevel(MapId.MarshAwakening1));
        }
    }
}