using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public class DebugLayout
{
    private readonly List<DebugWindow> _windows = new();
    private readonly List<DebugMenu> _menus = new();
    private DebugLayoutTextureManager _textureManager;
    private ImGuiRenderer _guiRenderer;

    public void AddWindow(DebugWindow window) => _windows.Add(window);
    public T GetWindow<T>() where T : DebugWindow => _windows.FirstOrDefault(x => x is T) as T;
    public IReadOnlyCollection<DebugWindow> GetWindows() => _windows;

    public void AddMenu(DebugMenu menu) => _menus.Add(menu);
    public T GetMenu<T>() where T : DebugMenu => _menus.FirstOrDefault(x => x is T) as T;
    public IReadOnlyCollection<DebugMenu> GetMenus() => _menus;

    public void LoadContent(Game game)
    {
        _guiRenderer = new ImGuiRenderer(game);
        _guiRenderer.RebuildFontAtlas();

        ImGuiIOPtr io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        _textureManager = new DebugLayoutTextureManager(_guiRenderer);
    }

    public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    {
        _guiRenderer.BeforeLayout(gameTime);

        ImGui.DockSpaceOverViewport();

        // Draw menus
        if (ImGui.BeginMainMenuBar())
        {
            foreach (DebugMenu menu in _menus)
            {
                if (ImGui.BeginMenu(menu.Name))
                {
                    menu.Draw(this, _textureManager);
                    ImGui.EndMenu();
                }
            }

            ImGui.EndMainMenuBar();
        }

        // Draw windows
        foreach (DebugWindow window in _windows)
        {
            if (window.IsOpen || !window.CanClose)
            {
                if (window.CanClose)
                {
                    bool open = window.IsOpen;

                    open = ImGui.Begin(window.Name, ref open);

                    if (open)
                    {
                        window.Draw(this, _textureManager);
                        ImGui.End();
                    }
                }
                else
                {
                    ImGui.Begin(window.Name);
                    window.Draw(this, _textureManager);
                    ImGui.End();
                }
            }
        }

        _guiRenderer.AfterLayout();

        _textureManager.UnbindTextures();
    }

    public void DrawGame(GfxRenderer renderer)
    {
        foreach (DebugWindow window in _windows)
        {
            if (window.IsOpen || !window.CanClose)
                window.DrawGame(renderer);
        }
    }

}