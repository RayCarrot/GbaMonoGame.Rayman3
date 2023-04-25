using System.Collections.Generic;

namespace OnyxCs.Gba.Sdk;

public abstract class Vram
{
    protected Vram()
    {
        _spritePalettes = new List<Palette>();
        _backgroundPalettes = new List<Palette>();
        _sprites = new List<Sprite>();
        _backgrounds = new Background[4];

        for (int i = 0; i < _backgrounds.Length; i++)
            _backgrounds[i] = new Background();
    }

    private readonly List<Palette> _spritePalettes;
    private readonly List<Palette> _backgroundPalettes;
    private readonly List<Sprite> _sprites;
    private readonly Background[] _backgrounds;

    // Sprite palettes
    public virtual void ClearSpritePalettes() => _spritePalettes.Clear();
    public virtual void AddSpritePalette(Palette palette) => _spritePalettes.Add(palette);
    public IReadOnlyList<Palette> GetSpritePalettes() => _spritePalettes;

    // Background palettes
    public virtual void ClearBackgroundPalettes() => _backgroundPalettes.Clear();
    public virtual void AddBackgroundPalette(Palette palette) => _backgroundPalettes.Add(palette);
    public IReadOnlyList<Palette> GetBackgroundPalettes() => _backgroundPalettes;

    // Sprites
    public void ClearSprites() => _sprites.Clear();
    public void AddSprite(Sprite sprite) => _sprites.Add(sprite);
    public IReadOnlyList<Sprite> GetSprites() => _sprites;

    // Backgrounds
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