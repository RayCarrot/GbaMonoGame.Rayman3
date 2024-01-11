using BinarySerializer.Onyx.Gba.Rayman3;

namespace OnyxCs.Gba.Rayman3;

public class EchoingCaves_M2 : FrameSideScroller
{
    public EchoingCaves_M2(MapId mapId) : base(mapId) { }

    // TODO: Implement lightning effect

    public override void UnInit()
    {
        base.UnInit();

        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__barrel);
    }
}