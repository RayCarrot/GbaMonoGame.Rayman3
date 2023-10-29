using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Onyx.Gba.Rayman3;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;
using OnyxCs.Gba.Engine2d;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

// TODO: Add options to show hitboxes and other kind of boxes
public class DebugLayout
{
    // TODO: Save in config which windows are shown
    private bool _showLogOutputWindow = true;
    private bool _showGfxWindow = true;
    private bool _showSceneWindow = true;
    private bool _showGameObjectWindow = true;
    private bool _showPlayfieldWindow = true;
    
    private bool _showViewBoxes;
    private bool _showActionBoxes;
    private bool _showDetectionBoxes;
    private bool _showAttackBoxes;
    private bool _showVulnerabilityBoxes;
    private bool _showCaptorBoxes;

    private bool _fillBoxes;

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
    private GameObject SelectedGameObject { get; set; }

    private void DrawBox(GfxRenderer renderer, GameObject gameObject, Rectangle box, Color color)
    {
        if (box == Rectangle.Empty) 
            return;

        TgxPlayfield2D playfield = Frame.GetComponent<TgxPlayfield2D>();
        
        box.Offset(gameObject.Position - playfield.Camera.Position);

        if (_fillBoxes)
            renderer.DrawFilledRectangle(box, new Color(color, 0.1f));
        else
            renderer.DrawRectangle(box, color);
    }

    private void DrawMenu()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Windows"))
            {
                if (ImGui.MenuItem("Log Output"))
                    _showLogOutputWindow = true;

                if (ImGui.MenuItem("Gfx"))
                    _showGfxWindow = true;

                if (ImGui.MenuItem("Scene"))
                    _showSceneWindow = true;

                if (ImGui.MenuItem("Game Object"))
                    _showGameObjectWindow = true;

                if (ImGui.MenuItem("Playfield"))
                    _showPlayfieldWindow = true;

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
                    for (int i = 0; i < GameInfo.Levels.Length; i++)
                    {
                        LevelInfo levelInfo = GameInfo.Levels[i];
                        if (ImGui.MenuItem(((MapId)i).ToString()))
                        {
                            FrameManager.SetNextFrame(LevelFactory.Create((MapId)i));
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

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

    private void DrawLogOutputWindow()
    {
        ImGui.Begin("Log Output", ref _showLogOutputWindow);

        ImGui.Text("TODO: Implement");

        ImGui.End();
    }

    private void DrawGfxWindow()
    {
        ImGui.Begin("Gfx", ref _showGfxWindow);

        ImGui.SeparatorText("Screens");

        if (ImGui.BeginTable("_screens", 6))
        {
            ImGui.TableSetupColumn("Enabled");
            ImGui.TableSetupColumn("Wrap");
            ImGui.TableSetupColumn("Id");
            ImGui.TableSetupColumn("Priority");
            ImGui.TableSetupColumn("Offset");
            ImGui.TableSetupColumn("Color mode");
            ImGui.TableHeadersRow();

            foreach (GfxScreen screen in Gfx.Screens)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                bool enabled = screen.IsEnabled;
                ImGui.Checkbox($"##{screen.Id}_enabled", ref enabled);
                screen.IsEnabled = enabled;

                ImGui.TableNextColumn();
                bool wrap = screen.Wrap;
                ImGui.Checkbox($"##{screen.Id}_wrap", ref wrap);
                screen.Wrap = wrap;

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Id}");

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Priority}");

                ImGui.TableNextColumn();
                ImGui.Text($"{screen.Offset.X:0.00} x {screen.Offset.Y:0.00}");

                ImGui.TableNextColumn();
                ImGui.Text($"{(screen.Is8Bit ? "8-bit" : "4-bit")}");
            }
            ImGui.EndTable();
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.SeparatorText("Sprites");

        ImGui.Text("TODO: Implement");

        ImGui.End();
    }

    private void DrawSceneWindow()
    {
        ImGui.Begin("Scene", ref _showSceneWindow);

        if (Frame.GetComponent<Scene2D>() is { } scene2D)
        {
            if (ImGui.Button("Deselect object"))
                SelectedGameObject = null;

            ImGui.SeparatorText("Always actors");

            ImGui.Text($"Count: {scene2D.Objects.AlwaysActorsCount}");

            if (ImGui.BeginListBox("##_alwaysActors"))
            {
                foreach (BaseActor actor in scene2D.Objects.Objects.
                             Take(scene2D.Objects.AlwaysActorsCount).
                             Cast<BaseActor>())
                {
                    bool isSelected = SelectedGameObject == actor;
                    if (ImGui.Selectable($"{actor.Id}. {(ActorType)actor.Type}", isSelected))
                        SelectedGameObject = actor;
                }

                ImGui.EndListBox();
            }

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.SeparatorText("Actors");

            ImGui.Text($"Count: {scene2D.Objects.ActorsCount}");

            if (ImGui.BeginListBox("##_actors"))
            {
                foreach (BaseActor actor in scene2D.Objects.Objects.
                             Skip(scene2D.Objects.AlwaysActorsCount).
                             Take(scene2D.Objects.ActorsCount).
                             Cast<BaseActor>())
                {
                    bool isSelected = SelectedGameObject == actor;
                    if (ImGui.Selectable($"{actor.Id}. {(ActorType)actor.Type}", isSelected))
                        SelectedGameObject = actor;
                }

                ImGui.EndListBox();
            }

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.SeparatorText("Captors");

            ImGui.Text($"Count: {scene2D.Objects.CaptorsCount}");

            if (scene2D.Objects.CaptorsCount > 0 && ImGui.BeginListBox("##_captors"))
            {
                foreach (Captor captor in scene2D.Objects.Objects.
                             Skip(scene2D.Objects.AlwaysActorsCount + scene2D.Objects.ActorsCount).
                             Take(scene2D.Objects.CaptorsCount).
                             Cast<Captor>())
                {
                    bool isSelected = SelectedGameObject == captor;
                    if (ImGui.Selectable($"{captor.Id}. Captor", isSelected))
                        SelectedGameObject = captor;
                }

                ImGui.EndListBox();
            }

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.SeparatorText("Knots");

            ImGui.Text("Count: 0");
            ImGui.Text("TODO: Implement");

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.SeparatorText("Boxes");

            ImGui.Checkbox("Fill", ref _fillBoxes);
            ImGui.Spacing();

            ImGui.Checkbox("Show view boxes", ref _showViewBoxes);
            ImGui.Checkbox("Show action boxes", ref _showActionBoxes);
            ImGui.Checkbox("Show detection boxes", ref _showDetectionBoxes);
            ImGui.Checkbox("Show attack boxes", ref _showAttackBoxes);
            ImGui.Checkbox("Show vulnerability boxes", ref _showVulnerabilityBoxes);
            ImGui.Checkbox("Show captor boxes", ref _showCaptorBoxes);
        }

        ImGui.End();
    }

    private void DrawGameObjectWindow()
    {
        ImGui.Begin("Game Object", ref _showGameObjectWindow);

        if (SelectedGameObject != null)
        {
            bool enabled = SelectedGameObject.IsEnabled;
            ImGui.Checkbox("Enabled", ref enabled);
            SelectedGameObject.IsEnabled = enabled;

            System.Numerics.Vector2 pos = new(SelectedGameObject.Position.X, SelectedGameObject.Position.Y);
            if (ImGui.InputFloat2("Position", ref pos))
                SelectedGameObject.Position = new Vector2(pos.X, pos.Y);
        }
        else
        {
            ImGui.Text("No object has been selected");
        }

        ImGui.End();
    }

    private void DrawPlayfieldWindow()
    {
        ImGui.Begin("Playfield", ref _showPlayfieldWindow);

        if (Frame.GetComponent<TgxPlayfield>() is TgxPlayfield2D playfield2D)
        {
            Vector2 pos = playfield2D.Camera.Position;

            ImGui.SeparatorText("Camera position");

            ImGui.SliderFloat("Camera X", ref pos.X, 0, playfield2D.Camera.GetMainCluster().MaxPosition.X);
            ImGui.SliderFloat("Camera Y", ref pos.Y, 0, playfield2D.Camera.GetMainCluster().MaxPosition.Y);
            playfield2D.Camera.Position = pos;

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.SeparatorText("Clusters");

            if (ImGui.BeginTable("_clusters", 6))
            {
                ImGui.TableSetupColumn("Id");
                ImGui.TableSetupColumn("Position");
                ImGui.TableSetupColumn("Max position");
                ImGui.TableSetupColumn("Scroll factor");
                ImGui.TableSetupColumn("Type");
                ImGui.TableSetupColumn("Layers");
                ImGui.TableHeadersRow();

                int i = 0;
                foreach (TgxCluster cluster in playfield2D.Camera.GetClusters(true))
                {
                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    ImGui.Text($"{(i == 0 ? "Main" : $"{i}")}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{cluster.Position.X:0.00} x {cluster.Position.Y:0.00}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{cluster.MaxPosition.X:0.00} x {cluster.MaxPosition.Y:0.00}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{cluster.ScrollFactor.X:0.00} x {cluster.ScrollFactor.Y:0.00}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{(cluster.Stationary ? "Stationary" : "Scrollable")}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{String.Join(", ", cluster.GetLayers().Select(x => x.LayerId))}");

                    i++;
                }
                ImGui.EndTable();
            }
        }

        ImGui.End();
    }

    public void LoadContent(GameRenderTarget gameRenderTarget, Game game, GameConfig config)
    {
        GameRenderTarget = gameRenderTarget;
        Config = config;

        GuiRenderer = new ImGuiRenderer(game);
        GuiRenderer.RebuildFontAtlas();

        ImGuiIOPtr io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
    }

    public void EnableDebugMode()
    {
        // Reset previous size to force it to refresh
        PreviousWindowSize = Point.Zero;
    }

    public void DrawGame(GfxRenderer renderer)
    {
        if (Frame.GetComponent<Scene2D>() is { } scene2D)
        {
            foreach (BaseActor actor in scene2D.Objects.EnumerateEnabledAlwaysActors().
                         Concat(scene2D.Objects.EnumerateEnabledActors()))
            {
                if (_showViewBoxes)
                    DrawBox(renderer, actor, actor.ViewBox, Color.Lime);

                if (actor is ActionActor actionActor)
                {
                    if (_showActionBoxes)
                        DrawBox(renderer, actionActor, actionActor.ActionBox, Color.Blue);

                    if (_showDetectionBoxes)
                        DrawBox(renderer, actionActor, actionActor.DetectionBox, Color.DeepPink);
                }

                if (actor is InteractableActor interactableActor)
                {
                    if (_showAttackBoxes)
                        DrawBox(renderer, interactableActor, interactableActor.AnimationBoxTable.AttackBox, Color.OrangeRed);

                    if (_showVulnerabilityBoxes)
                        DrawBox(renderer, interactableActor, interactableActor.AnimationBoxTable.VulnerabilityBox, Color.Green);
                }
            }

            if (_showCaptorBoxes)
            {
                foreach (Captor captor in scene2D.Objects.EnumerateEnabledCaptors())
                {
                    DrawBox(renderer, captor, captor.CaptorBox, Color.DeepPink);
                }
            }
        }

        if (SelectedGameObject is BaseActor selectedActor)
            DrawBox(renderer, selectedActor, selectedActor.ViewBox, Color.Red);
        else if (SelectedGameObject is Captor selectedCaptor)
            DrawBox(renderer, selectedCaptor, selectedCaptor.CaptorBox, Color.Red);
    }

    public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        GuiRenderer.BeforeLayout(gameTime);

        ImGui.DockSpaceOverViewport();

        DrawMenu();

        if (_showLogOutputWindow)
            DrawLogOutputWindow();

        if (_showGfxWindow)
            DrawGfxWindow();

        if (_showSceneWindow)
            DrawSceneWindow();

        if (_showGameObjectWindow)
            DrawGameObjectWindow();

        if (_showPlayfieldWindow)
            DrawPlayfieldWindow();

        DrawGameWindow();

        GuiRenderer.AfterLayout();

        foreach (IntPtr ptr in TexturePointers)
            GuiRenderer.UnbindTexture(ptr);
    }

    private record FrameFactory(string Name, Func<Frame> CreateFrame);
}