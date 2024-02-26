using System;

namespace GbaMonoGame;

/// <summary>
/// An affine matrix for rotation/scaling.
/// </summary>
public readonly struct AffineMatrix
{
    public AffineMatrix(float rotation, Vector2 scale)
    {
        Rotation = rotation;
        Scale = scale;
        FlipX = false;
        FlipY = false;
    }

    public AffineMatrix(float rotation, Vector2 scale, bool flipX, bool flipY)
    {
        Rotation = rotation;
        Scale = scale;
        FlipX = flipX;
        FlipY = flipY;
    }

    public AffineMatrix(float pa, float pb, float pc, float pd)
    {
        // The following affine sprite rendering code has been re-implemented from Ray1Map. Credits to Droolie for writing it!
        Rotation = MathF.Atan2(pb, pa);

        float delta = pa * pd - pb * pc;

        Vector2 scale;

        // Apply the QR-like decomposition.
        if (pa != 0 || pb != 0)
        {
            float r = MathF.Sqrt(pa * pa + pb * pb);
            scale = new Vector2(r, delta / r);
        }
        else if (pc != 0 || pd != 0)
        {
            float s = MathF.Sqrt(pc * pc + pd * pd);
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

        // Since we can't set a negative sprite scale in MonoGame we
        // instead get the absolute scale and flip the sprite accordingly
        if (scale.X < 0)
        {
            scale.X = -scale.X;
            FlipX = true;
        }
        else
        {
            FlipX = false;
        }
        if (scale.Y < 0)
        {
            scale.Y = -scale.Y;
            FlipY = true;
        }
        else
        {
            FlipY = false;
        }

        Scale = scale;
    }

    public AffineMatrix(float rotation256, float scaleX, float scaleY)
        : this(
            pa: scaleX * MathHelpers.Cos256(rotation256),
            pb: scaleX * MathHelpers.Sin256(rotation256),
            pc: scaleY * -MathHelpers.Sin256(rotation256),
            pd: scaleY * MathHelpers.Cos256(rotation256))
    {

    }

    public float Rotation { get; }
    public Vector2 Scale { get; }
    public bool FlipX { get; }
    public bool FlipY { get; }

    public static AffineMatrix Identity => new(1, 0, 0, 1);
}