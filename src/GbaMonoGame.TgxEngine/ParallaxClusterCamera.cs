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
        Vector2 res = Engine.GameViewPort.GameResolution;

        if (Engine.GameViewPort.GameResolution == Engine.GameViewPort.OriginalGameResolution)
            return res;

        float maxX = Cluster.GetLayers().Min(x => x.PixelWidth);
        float maxY = Cluster.GetLayers().Min(x => x.PixelHeight);

        if (res.X > res.Y)
        {
            if (res.Y > maxY)
                res = new Vector2(maxY * res.X / res.Y, maxY);

            if (res.X > maxX)
                res = new Vector2(maxX, maxX * res.Y / res.X);
        }
        else
        {
            if (res.X > maxX)
                res = new Vector2(maxX, maxX * res.Y / res.X);

            if (res.Y > maxY)
                res = new Vector2(maxY * res.X / res.Y, maxY);
        }

        if (res != Engine.GameViewPort.GameResolution)
            Logger.Debug("Set cluster with size {0} camera resolution to {1}", Cluster.Size, res);

        return res;
    }
}