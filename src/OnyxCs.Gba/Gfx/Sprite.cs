using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class Sprite
{
    public Sprite(Texture2D texture, Vector2 position, bool flipX, bool flipY, int priority, AffineMatrix? affineMatrix)
    {
        Texture = texture;
        Position = position;
        FlipX = flipX;
        FlipY = flipY;
        Priority = priority;

        if (affineMatrix != null)
        {
            // The following affine sprite rendering code has been re-implemented from Ray1Map. Credits to Droolie for writing it!

            Rotation = MathF.Atan2(affineMatrix.Value.Pb / 256f, affineMatrix.Value.Pa / 256f);

            float a = affineMatrix.Value.Pa;
            float b = affineMatrix.Value.Pb;
            float c = affineMatrix.Value.Pc;
            float d = affineMatrix.Value.Pd;
            float delta = a * d - b * c;

            Vector2 scale;

            // Apply the QR-like decomposition.
            if (a != 0 || b != 0)
            {
                float r = MathF.Sqrt(a * a + b * b);
                scale = new Vector2(r, delta / r);
            }
            else if (c != 0 || d != 0)
            {
                float s = MathF.Sqrt(c * c + d * d);
                scale = new Vector2(delta / s, s);
            }
            else
            {
                scale = Vector2.Zero;
            }

            if (scale.X != 0)
                scale.X = 1f / scale.X;

            if (scale.Y != 0)
                scale.Y = 1f / scale.Y;

            Scale = scale;
        }
        else
        {
            Rotation = 0;
            Scale = Vector2.One;
        }

        // Since we can't set a negative sprite scale in MonoGame we
        // instead get the absolute scale and flip the sprite accordingly

        Effects = SpriteEffects.None;

        if (FlipX || Scale.X < 0)
            Effects |= SpriteEffects.FlipHorizontally;
        if (FlipY || Scale.Y < 0)
            Effects |= SpriteEffects.FlipVertically;

        Scale = new Vector2(Math.Abs(Scale.X), Math.Abs(Scale.Y));
        Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
    }

    public Texture2D Texture { get; }
    public Vector2 Position { get; }
    public bool FlipX { get; }
    public bool FlipY { get; }
    public int Priority { get; }

    public float Rotation { get; }
    public Vector2 Origin { get; }
    public Vector2 Scale { get; }
    public SpriteEffects Effects { get; }

    public void Draw(GfxRenderer renderer)
    {
        renderer.Draw(Texture, Position + Origin, null, Rotation, Origin, Scale, Effects, Color.White);
    }
}