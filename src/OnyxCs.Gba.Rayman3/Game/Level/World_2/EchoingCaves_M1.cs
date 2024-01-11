using BinarySerializer.Onyx.Gba.Rayman3;

namespace OnyxCs.Gba.Rayman3;

public class EchoingCaves_M1 : FrameSideScroller
{
    public EchoingCaves_M1(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        // TODO: Initialize intro cutscene
    }

    public override void UnInit()
    {
        base.UnInit();

        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__barrel);
    }

    public override void Step()
    {
        // TODO: Implement stepping intro cutscene
        base.Step();
    }
}