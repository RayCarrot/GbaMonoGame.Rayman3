using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class BossScaleMan : FrameSideScroller
{
    public BossScaleMan(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();
        Scene.Camera.LinkedObject = Scene.KnotManager.GetGameObject<MovableActor>(1);
        Scene.MainActor.ProcessMessage((Message)1057); // TODO: Name and implement
    }
}