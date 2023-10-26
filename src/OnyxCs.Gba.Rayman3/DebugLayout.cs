using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class DebugLayout
{
    private bool _showPlayfieldWindow;

    private List<IntPtr> TexturePointers { get; } = new();
    private Point PreviousWindowSize { get; set; }
    private ImGuiRenderer GuiRenderer { get; set; }
    private GameRenderTarget GameRenderTarget { get; set; }
    private GameConfig Config { get; set; }
    private FrameFactory[] FrameFactories { get; } = 
    {
        new("Intro", () => new Intro()),
        new("Menu", () => new MenuAll(MenuAll.Page.Language)),
    };

    private void DrawGameWindow()
    {
        ImGui.Begin("Game");

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
            IntPtr texPtr = GuiRenderer.BindTexture(GameRenderTarget.RenderTarget);
            ImGui.Image(texPtr, new System.Numerics.Vector2(GameRenderTarget.RenderTarget.Width, GameRenderTarget.RenderTarget.Height));
            TexturePointers.Add(texPtr);
        }

        ImGui.End();
    }

    private void DrawMenu()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Windows"))
            {
                ImGui.MenuItem("Sprites");
                ImGui.MenuItem("Screens");
                ImGui.Separator();
                ImGui.MenuItem("Scene");

                if (Frame.GetComponent<TgxPlayfield>() != null)
                    _showPlayfieldWindow = ImGui.MenuItem("Playfield");

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Options"))
            {
                if (ImGui.MenuItem("Pause", "", Config.Paused))
                    Config.Paused = !Config.Paused;
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Frames"))
            {
                foreach (FrameFactory frameFactory in FrameFactories)
                {
                    if (ImGui.MenuItem(frameFactory.Name))
                    {
                        FrameManager.SetNextFrame(frameFactory.CreateFrame());
                    }
                }

                if (ImGui.BeginMenu("Levels"))
                {
                    foreach (LevelInfo levelInfo in GameInfo.Levels)
                    {
                        if (ImGui.MenuItem(levelInfo.LevelId.ToString()))
                        {
                            FrameManager.SetNextFrame((Frame)Activator.CreateInstance(levelInfo.FrameType, levelInfo.LevelId));
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    private void DrawPlayfieldWindow()
    {
        if (Frame.GetComponent<TgxPlayfield>() is TgxPlayfield2D playfield2D)
        {
            ImGui.Begin("Playfield2D", ref _showPlayfieldWindow);

            Vector2 pos = playfield2D.Camera.Position;

            ImGui.SliderFloat("Camera X", ref pos.X, 0, playfield2D.Camera.GetMainCluster().MaxPosition.X);
            ImGui.SliderFloat("Camera Y", ref pos.Y, 0, playfield2D.Camera.GetMainCluster().MaxPosition.Y);

            ImGui.Spacing();
            ImGui.Spacing();

            playfield2D.Camera.Position = pos;

            int i = 0;
            foreach (TgxCluster cluster in playfield2D.Camera.GetClusters(true))
            {
                if (ImGui.CollapsingHeader(i == 0 ? "Main cluster" : $"Cluster {i}"))
                {
                    ImGui.Text($"Position: {cluster.Position}");
                    ImGui.Text($"Max position: {cluster.MaxPosition}");
                    ImGui.Text($"Stationary: {cluster.Stationary}");

                    ImGui.Indent();

                    foreach (TgxTileLayer tileLayer in cluster.GetLayers())
                    {
                        if (ImGui.CollapsingHeader($"Layer {tileLayer.LayerId}"))
                        {
                            bool isEnabled = tileLayer.Screen.IsEnabled;
                            ImGui.Checkbox($"Enabled##{tileLayer.LayerId}", ref isEnabled);
                            tileLayer.Screen.IsEnabled = isEnabled;

                            ImGui.Text($"Size: {tileLayer.Width}x{tileLayer.Height}");
                            ImGui.Text($"Dynamic: {tileLayer.IsDynamic}");
                            ImGui.Text($"Color mode: {(tileLayer.Screen.Is8Bit ? "8-bit" : "4-bit")}");
                            ImGui.Text($"Offset: {tileLayer.Screen.Offset}");
                            ImGui.Text($"Priority: {tileLayer.Screen.Priority}");
                            ImGui.Text($"Wrap: {tileLayer.Screen.Wrap}");
                            ImGui.Text($"Renderer: {tileLayer.Screen.Renderer.GetType().Name}");
                        }
                    }

                    ImGui.Unindent();
                }

                i++;
            }

            ImGui.End();
        }
    }

    public void LoadContent(GameRenderTarget gameRenderTarget, Game game, GameConfig config)
    {
        GameRenderTarget = gameRenderTarget;
        Config = config;

        GuiRenderer = new ImGuiRenderer(game);
        GuiRenderer.RebuildFontAtlas();
    }

    public void EnableDebugMode()
    {
        // Reset previous size to force it to refresh
        PreviousWindowSize = Point.Zero;
    }

    public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        GuiRenderer.BeforeLayout(gameTime);

        ImGuiIOPtr io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        ImGui.DockSpaceOverViewport();

        DrawMenu();

        if (_showPlayfieldWindow)
            DrawPlayfieldWindow();

        DrawGameWindow();

        GuiRenderer.AfterLayout();

        foreach (IntPtr ptr in TexturePointers)
            GuiRenderer.UnbindTexture(ptr);
    }

    private record FrameFactory(string Name, Func<Frame> CreateFrame);
}