using System.Collections.Generic;

namespace GbaMonoGame.TgxEngine;

/// <summary>
/// Custom class for caching tile data in a playfield. Used to be able to quickly re-create a playfield, such as when dying.
/// </summary>
public class CachedTileKit
{
    private readonly Dictionary<int, IScreenRenderer> _renderers = new();

    public void CacheRenderer(int id, IScreenRenderer renderer) => _renderers.Add(id, renderer);
    public IScreenRenderer GetRenderer(int id) => _renderers[id];

    public static CachedTileKit FromPlayfield(TgxPlayfield playfield)
    {
        CachedTileKit cachedTileKit = new();

        // TODO: Cache for Mode7 as well
        if (playfield is TgxPlayfield2D playfield2D)
            foreach (TgxTileLayer layer in playfield2D.TileLayers)
                cachedTileKit.CacheRenderer(layer.LayerId, layer.Screen.Renderer);

        return cachedTileKit;
    }
}