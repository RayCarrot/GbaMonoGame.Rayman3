using BinarySerializer.Nintendo.GBA;

namespace GbaMonoGame.TgxEngine;

public class TiledTextureScreenRenderer : TextureScreenRenderer
{
    public TiledTextureScreenRenderer(int width, int height, byte[] tileSet, MapTile[] tileMap, Palette palette, bool is8Bit)
        : base(new TiledTexture2D(width, height, tileSet, tileMap, palette, is8Bit))
    {
        TileSet = tileSet;
        TileMap = tileMap;
        Palette = palette;
    }

    public byte[] TileSet { get; }
    public MapTile[] TileMap { get; }
    public Palette Palette { get; }
}