using System;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.TgxEngine;

public abstract class TgxCamera : GfxCamera
{
    protected TgxCamera(GameWindow gameWindow) : base(gameWindow)
    {
        Engine.Config.ConfigChanged += Config_ConfigChanged;
    }

    public abstract Vector2 Position { get; set; }

    private void Config_ConfigChanged(object sender, EventArgs e) => UpdateResolution();

    protected override Vector2 GetResolution(GameWindow gameWindow) => gameWindow.GameResolution * Engine.Config.Scale;

    public override void UnInit()
    {
        base.UnInit();
        Engine.Config.ConfigChanged -= Config_ConfigChanged;
    }
}