using Microsoft.Xna.Framework;

namespace GbaMonoGame.Rayman3;

public class SquareTransitionScreenEffect : ScreenEffect
{
    public Box Square { get; set; }

    public override void Draw(GfxRenderer renderer)
    {
        renderer.BeginRender(new RenderOptions(false, null, Camera));

        renderer.DrawFilledRectangle(Vector2.Zero, new Vector2(Square.MinX, Camera.Resolution.Y), Color.Black); // Left
        renderer.DrawFilledRectangle(new Vector2(Square.MaxX, 0), new Vector2(Camera.Resolution.X - Square.MaxX, Camera.Resolution.Y), Color.Black); // Right
        renderer.DrawFilledRectangle(new Vector2(Square.MinX, 0), new Vector2(Square.Size.X, Square.MinY), Color.Black); // Top
        renderer.DrawFilledRectangle(new Vector2(Square.MinX, Square.MaxY), new Vector2(Square.Size.X, Camera.Resolution.Y - Square.MaxY), Color.Black); // Bottom
    }
}