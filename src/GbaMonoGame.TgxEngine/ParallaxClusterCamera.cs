using System.Linq;
using Microsoft.Xna.Framework;

namespace GbaMonoGame.TgxEngine;

public class ParallaxClusterCamera : GfxCamera
{
    public ParallaxClusterCamera(GameViewPort gameViewPort, TgxCluster cluster) : base(gameViewPort)
    {
        Cluster = cluster;
    }

    public TgxCluster Cluster { get; }

    protected override Vector2 GetResolution(GameViewPort gameViewPort)
    {
        // We want the parallax backgrounds to target the screen resolution since that
        // most closely matches how they were meant to be rendered. But since you can
        // play in higher internal resolution (like for widescreen) we have to make
        // sure it doesn't exceed the size of the smallest tile layer.
        Vector2 res = Engine.ScreenCamera.Resolution;

        if (Engine.ScreenCamera.Resolution == Engine.GameViewPort.OriginalGameResolution)
            return res;

        Vector2 max = Cluster.GetLayers().Min(x => new Vector2(x.PixelWidth, x.PixelHeight));

        if (res.X > res.Y)
        {
            if (res.Y > max.Y)
                res = new Vector2(max.Y * res.X / res.Y, max.Y);

            if (res.X > max.X)
                res = new Vector2(max.X, max.X * res.Y / res.X);
        }
        else
        {
            if (res.X > max.X)
                res = new Vector2(max.X, max.X * res.Y / res.X);

            if (res.Y > max.Y)
                res = new Vector2(max.Y * res.X / res.Y, max.Y);
        }

        if (res != Engine.ScreenCamera.Resolution)
            Logger.Debug("Set cluster with size {0} camera resolution to {1}", Cluster.Size, res);

        return res;
    }
}