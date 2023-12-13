using System;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

/// <summary>
/// Debug window which renders the game.
/// </summary>
public class GameDebugWindow : DebugWindow
{
    public GameDebugWindow(GameRenderTarget gameRenderTarget)
    {
        GameRenderTarget = gameRenderTarget;
    }

    private Point PreviousWindowSize { get; set; }

    public override string Name => "Game";
    public override bool CanClose => false;
    public GameRenderTarget GameRenderTarget { get; }

    public void RefreshSize()
    {
        // Reset previous size to force it to refresh
        PreviousWindowSize = Point.Zero;
    }

    public override void Draw(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        Point newSize = new(
            (int)ImGui.GetWindowContentRegionMax().X - (int)ImGui.GetWindowContentRegionMin().X,
            (int)ImGui.GetWindowContentRegionMax().Y - (int)ImGui.GetWindowContentRegionMin().Y);

        if (newSize != PreviousWindowSize)
        {
            PreviousWindowSize = newSize;
            GameRenderTarget.ResizeGame(newSize);
        }

        if (GameRenderTarget.RenderTarget != null)
        {
            IntPtr texPtr = textureManager.BindTexture(GameRenderTarget.RenderTarget);
            JoyPad.MouseOffset = -ImGui.GetCursorScreenPos();
            ImGui.Image(texPtr, new System.Numerics.Vector2(GameRenderTarget.RenderTarget.Width, GameRenderTarget.RenderTarget.Height));
        }
    }

    public override void OnWindowOpened()
    {
        RefreshSize();
    }

    public override void OnWindowClosed()
    {
        JoyPad.MouseOffset = Vector2.Zero;
    }
}