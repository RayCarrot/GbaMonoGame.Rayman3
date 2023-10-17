using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class Sprite
{
    public Sprite(Texture2D texture, Vector2 position, bool flipX, bool flipY, int priority, OBJ_ATTR_ObjectMode mode, AffineMatrix affineMatrix)
    {
        Texture = texture;
        Position = position;
        FlipX = flipX;
        FlipY = flipY;
        Priority = priority;
        Mode = mode;
        AffineMatrix = affineMatrix;
    }

    public Texture2D Texture { get; }
    public Vector2 Position { get; }
    public bool FlipX { get; }
    public bool FlipY { get; }
    public int Priority { get; }

    public OBJ_ATTR_ObjectMode Mode { get; }
    public AffineMatrix AffineMatrix { get; }

    public void Draw(GfxRenderer renderer)
    {
        if (Texture == null || Mode == OBJ_ATTR_ObjectMode.HIDE)
            return;

        // TODO: Implement affine sprite rendering
        if (Mode is OBJ_ATTR_ObjectMode.AFF or OBJ_ATTR_ObjectMode.AFF_DBL)
            return;

        SpriteEffects effects = SpriteEffects.None;

        if (FlipX)
            effects |= SpriteEffects.FlipHorizontally;
        if (FlipY)
            effects |= SpriteEffects.FlipVertically;

        renderer.SpriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Vector2.One, effects, 0);
    }
}