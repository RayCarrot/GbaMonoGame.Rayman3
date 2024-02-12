using System;

namespace GbaMonoGame.Rayman3;

public class FrameMode7 : Frame
{
    public FrameMode7(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }

    public override void UnInit()
    {
        throw new NotImplementedException();
    }

    public override void Step()
    {
        throw new NotImplementedException();
    }
}