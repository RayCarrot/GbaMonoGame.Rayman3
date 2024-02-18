using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public abstract class TgxCamera : GfxCamera
{
    protected TgxCamera(GameWindow gameWindow) : base(gameWindow)
    {
        Engine.Config.ConfigChanged += Config_ConfigChanged;
    }

    public abstract Vector2 Position { get; set; }

    private void Config_ConfigChanged(object sender, EventArgs e) => UpdateResolution();

    public override void UnInit()
    {
        base.UnInit();
        Engine.Config.ConfigChanged -= Config_ConfigChanged;
    }
}