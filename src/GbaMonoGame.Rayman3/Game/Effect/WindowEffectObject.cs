using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public class WindowEffectObject : EffectObject
{
    public Box Window { get; set; }

    public override void Execute(Action<short> soundEventCallback)
    {
        if (Engine.Settings.Platform == Platform.NGage)
            return;

        DrawRectangle(Vector2.Zero, new Vector2(Window.MinX, Camera.Resolution.Y), Color.Black); // Left
        DrawRectangle(new Vector2(Window.MaxX, 0), new Vector2(Camera.Resolution.X - Window.MaxX, Camera.Resolution.Y), Color.Black); // Right
        DrawRectangle(new Vector2(Window.MinX, 0), new Vector2(Window.Size.X, Window.MinY), Color.Black); // Top
        DrawRectangle(new Vector2(Window.MinX, Window.MaxY), new Vector2(Window.Size.X, Camera.Resolution.Y - Window.MaxY), Color.Black); // Bottom
    }
}