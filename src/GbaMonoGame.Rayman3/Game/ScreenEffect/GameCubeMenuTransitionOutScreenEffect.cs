using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public class GameCubeMenuTransitionOutScreenEffect : ScreenEffect
{
    public float Value { get; set; } // 0-80

    public override void Draw(GfxRenderer renderer)
    {
        if (Engine.Settings.Platform == Platform.NGage)
            return;

        renderer.BeginRender(new RenderOptions(false, null, Camera));

        Vector2 size = new(Value * 1.5f, Value);

        renderer.DrawFilledRectangle(Vector2.Zero, size, Color.Black); // Top-left
        renderer.DrawFilledRectangle(new Vector2(Camera.Resolution.X - size.X, 0), size, Color.Black); // Top-right
        renderer.DrawFilledRectangle(new Vector2(0, Camera.Resolution.Y - size.Y), size, Color.Black); // Bottom-left
        renderer.DrawFilledRectangle(Camera.Resolution - size, size, Color.Black); // Bottom-right
        renderer.DrawFilledRectangle(Camera.Resolution / 2 - size, size * 2, Color.Black); // Middle
    }
}