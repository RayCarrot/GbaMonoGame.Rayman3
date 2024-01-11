using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame;

public class DebugLayoutTextureManager
{
    public DebugLayoutTextureManager(ImGuiRenderer guiRenderer)
    {
        _guiRenderer = guiRenderer;
    }

    private readonly ImGuiRenderer _guiRenderer;
    private readonly List<IntPtr> _textures = new();

    public IntPtr BindTexture(Texture2D texture)
    {
        IntPtr ptr = _guiRenderer.BindTexture(texture);
        _textures.Add(ptr);
        return ptr;
    }

    public void UnbindTextures()
    {
        foreach (IntPtr ptr in _textures)
            _guiRenderer.UnbindTexture(ptr);
    }
}