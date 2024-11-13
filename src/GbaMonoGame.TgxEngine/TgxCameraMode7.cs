using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public class TgxCameraMode7 : TgxCamera
{
    public TgxCameraMode7(GameViewPort gameViewPort) : base(gameViewPort)
    {
        // TODO: Implement
    }

    // TODO: Implement
    public override Vector2 Position { get; set; }

    public int MaxDist { get; set; }
    public byte field_0xb49 { get; set; }
    public byte Direction { get; set; }

    protected override Vector2 GetResolution(GameViewPort gameViewPort)
    {
        // TODO: Implement
        return Engine.GameViewPort.OriginalGameResolution;
    }
}