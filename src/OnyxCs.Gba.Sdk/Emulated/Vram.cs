using System.Collections.Generic;

namespace OnyxCs.Gba.Sdk;

public abstract class Vram
{
    protected Vram()
    {
        _sprites = new List<Sprite>();
        _backgrounds = new Background[4];

        for (int i = 0; i < _backgrounds.Length; i++)
            _backgrounds[i] = new Background();
    }

    private readonly List<Sprite> _sprites;
    private readonly Background[] _backgrounds;

    public virtual PaletteManager? SpritePaletteManager => null;
    public virtual PaletteManager? BackgroundPaletteManager => null;

    public void ClearSprites() => _sprites.Clear();
    public void AddSprite(Sprite sprite) => _sprites.Add(sprite);
    public IReadOnlyList<Sprite> GetSprites() => _sprites;

    public void ClearBackgrounds()
    {
        foreach (Background bg in _backgrounds)
        {
            bg.Map = null;
            bg.TileSet = null;
            bg.Width = 0;
            bg.Height = 0;
        }
    }
    public Background GetBackground(int bg) => _backgrounds[bg];
    public IReadOnlyList<Background> GetBackgrounds() => _backgrounds;
}