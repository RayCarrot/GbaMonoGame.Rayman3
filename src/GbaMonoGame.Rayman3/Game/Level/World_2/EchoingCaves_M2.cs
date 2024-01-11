using BinarySerializer.Ubisoft.GbaEngine.Rayman3;

namespace GbaMonoGame.Rayman3;

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