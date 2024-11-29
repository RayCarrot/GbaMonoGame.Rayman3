using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public class GameCubeMenuTransitionOutEffectObject : EffectObject
{
    public float Value { get; set; } // 0-80

    public override void Execute(Action<short> soundEventCallback)
    {
        if (Engine.Settings.Platform == Platform.NGage)
            return;

        Vector2 size = new(Value * 1.5f, Value);

        DrawRectangle(Vector2.Zero, size, Color.Black); // Top-left
        DrawRectangle(new Vector2(Camera.Resolution.X - size.X, 0), size, Color.Black); // Top-right
        DrawRectangle(new Vector2(0, Camera.Resolution.Y - size.Y), size, Color.Black); // Bottom-left
        DrawRectangle(Camera.Resolution - size, size, Color.Black); // Bottom-right
        DrawRectangle(Camera.Resolution / 2 - size, size * 2, Color.Black); // Middle
    }
}