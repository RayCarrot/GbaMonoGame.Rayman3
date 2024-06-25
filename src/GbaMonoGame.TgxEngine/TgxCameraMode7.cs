using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public class TgxCameraMode7 : TgxCamera
{
    public TgxCameraMode7(GameViewPort gameViewPort) : base(gameViewPort)
    {
        // TODO: Implement
    }

    protected override Vector2 GetResolution(GameViewPort gameViewPort)
    {
        // TODO: Implement
        return Engine.GameViewPort.OriginalGameResolution;
    }

    // TODO: Implement
    public override Vector2 Position { get; set; }
}