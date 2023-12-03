using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.TgxEngine;

public class TextureScreenRenderer : IScreenRenderer
{
    public TextureScreenRenderer(Texture2D texture, byte[] tileSet, MapTile[] tileMap, Palette palette)
    {
        Texture = texture;
        TileSet = tileSet;
        TileMap = tileMap;
        Palette = palette;
    }

    public byte[] TileSet { get; }
    public MapTile[] TileMap { get; }
    public Palette Palette { get; }
    public Texture2D Texture { get; }
    public Vector2 Size => new(Texture.Width, Texture.Height);

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        renderer.Draw(Texture, position, color);
    }
}