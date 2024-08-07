﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class GameRenderTarget
{
    public GameRenderTarget(GraphicsDevice graphicsDevice, GameViewPort gameViewPort)
    {
        GraphicsDevice = graphicsDevice;
        GameViewPort = gameViewPort;
    }

    private Point? PendingResize { get; set; }

    public GraphicsDevice GraphicsDevice { get; }
    public GameViewPort GameViewPort { get; }
    public RenderTarget2D RenderTarget { get; private set; }

    public void ResizeGame(Point newSize)
    {
        // Save resizing for next render so we don't create a black texture during this frame
        PendingResize = newSize;
    }

    public void BeginRender()
    {
        if (PendingResize != null)
        {
            GameViewPort.Resize(PendingResize.Value.ToVector2());
            RenderTarget?.Dispose();
            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                // Make sure the size doesn't reach 0 during resizing
                Math.Max(PendingResize.Value.X, 1),
                Math.Max(PendingResize.Value.Y, 1),
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            PendingResize = null;
        }

        GraphicsDevice.SetRenderTarget(RenderTarget);
        GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
    }

    public void EndRender()
    {
        GraphicsDevice.SetRenderTarget(null);
    }
}