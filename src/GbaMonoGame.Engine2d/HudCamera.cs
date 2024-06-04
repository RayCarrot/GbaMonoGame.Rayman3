using System;

namespace GbaMonoGame.Engine2d;

public class HudCamera : GfxCamera
{
    public HudCamera(GameViewPort gameViewPort) : base(gameViewPort)
    {
        Engine.Config.ConfigChanged += Config_ConfigChanged;
    }

    private void Config_ConfigChanged(object sender, EventArgs e) => UpdateResolution();

    protected override Vector2 GetResolution(GameViewPort gameViewPort)
    {
        return gameViewPort.GameResolution * Engine.Config.HudCameraScale;
    }

    public override void UnInit()
    {
        base.UnInit();
        Engine.Config.ConfigChanged -= Config_ConfigChanged;
    }
}