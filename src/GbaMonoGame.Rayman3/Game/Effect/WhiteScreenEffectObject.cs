using System;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public class WhiteScreenEffectObject : EffectObject
{
    public override void Execute(Action<short> soundEventCallback)
    {
        DrawRectangle(Vector2.Zero, Camera.Resolution, Color.White);
    }
}