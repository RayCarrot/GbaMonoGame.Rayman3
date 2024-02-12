using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class BeneathTheSanctuary_M1 : FrameSideScroller
{
    public BeneathTheSanctuary_M1(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        // Why is this here and only on GBA?
        if (Engine.Settings.Platform == Platform.GBA)
        {
            Scene.KnotManager.GetGameObject(129).ProcessMessage(Message.Destroy);
            Scene.KnotManager.GetGameObject(130).ProcessMessage(Message.Destroy);
            Scene.KnotManager.GetGameObject(131).ProcessMessage(Message.Destroy);
            Scene.KnotManager.GetGameObject(132).ProcessMessage(Message.Destroy);
            Scene.KnotManager.GetGameObject(133).ProcessMessage(Message.Destroy);
        }
    }
}