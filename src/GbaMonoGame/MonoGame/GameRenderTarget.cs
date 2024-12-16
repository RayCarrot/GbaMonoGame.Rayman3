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

    private Point? _pendingResize;

    public GraphicsDevice GraphicsDevice { get; }
    public GameViewPort GameViewPort { get; }
    public RenderTarget2D RenderTarget { get; private set; }

    public void ResizeGame(Point newSize)
    {
        // Save resizing for next render so we don't create a black texture during this frame
        _pendingResize = newSize;
    }

    public void BeginRender()
    {
        if (_pendingResize != null)
        {
            GameViewPort.Resize(_pendingResize.Value.ToVector2());
            RenderTarget?.Dispose();
            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                // Make sure the size doesn't reach 0 during resizing
                Math.Max(_pendingResize.Value.X, 1),
                Math.Max(_pendingResize.Value.Y, 1),
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            _pendingResize = null;
        }

        GraphicsDevice.SetRenderTarget(RenderTarget);
        GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
    }

    public void EndRender()
    {
        GraphicsDevice.SetRenderTarget(null);
    }
}