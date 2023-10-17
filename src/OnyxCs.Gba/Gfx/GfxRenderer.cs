using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba;

public class GfxRenderer
{
    public GfxRenderer(SpriteBatch spriteBatch, Matrix transformMatrix)
    {
        SpriteBatch = spriteBatch;
        TransformMatrix = transformMatrix;
    }

    public SpriteBatch SpriteBatch { get; }
    public Matrix TransformMatrix { get; }

    public void Begin()
    {
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: TransformMatrix);
    }

    public void End()
    {
        SpriteBatch.End();
    }

    public void BeginAlpha()
    {
        SpriteBatch.End();
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.Additive, transformMatrix: TransformMatrix);
    }

    public void EndAlpha()
    {
        SpriteBatch.End();
        Begin();
    }
}