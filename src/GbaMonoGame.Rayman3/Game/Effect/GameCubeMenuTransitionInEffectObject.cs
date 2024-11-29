using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public class GameCubeMenuTransitionInEffectObject : EffectObject
{
    public float Value { get; set; } // 0-240

    public override void Execute(Action<short> soundEventCallback)
    {
        if (Engine.Settings.Platform == Platform.NGage)
            return;

        // 3 rects with heights 54, 52 and 54
        DrawRectangle(Vector2.Zero, new Vector2(Camera.Resolution.X - Value, 54), Color.Black);
        DrawRectangle(new Vector2(Value, 54), new Vector2(Camera.Resolution.X - Value, 52), Color.Black);
        DrawRectangle(new Vector2(0, 54 + 52), new Vector2(Camera.Resolution.X - Value, 54), Color.Black);
    }
}