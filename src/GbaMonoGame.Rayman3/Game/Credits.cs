using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame.Rayman3;

// TODO: Implement
// TODO: Add to frames selection
public class Credits : Frame
{
    public override void Init()
    {
        Storage.LoadResource<AnimActor>(126);
        Storage.LoadResource<TextureTable>(127);
    }

    public override void Step()
    {
        
    }
}