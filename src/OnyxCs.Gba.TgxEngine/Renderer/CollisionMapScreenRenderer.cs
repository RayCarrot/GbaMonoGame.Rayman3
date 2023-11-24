using System;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.TgxEngine;

public class CollisionMapScreenRenderer : IScreenRenderer
{
    public CollisionMapScreenRenderer(int width, int height, byte[] collisionMap)
    {
        Width = width;
        Height = height;
        CollisionMap = collisionMap;
    }

    private const int CollisionTileSize = 16;
    private const int CollisionTileSetWidth = 16;

    private static readonly Texture2D _tex = Engine.ContentManager.Load<Texture2D>("CollisionTileSet");

    public int Width { get; }
    public int Height { get; }
    public byte[] CollisionMap { get; }

    public Vector2 Size => new(Width * Constants.TileSize, Height * Constants.TileSize);

    private Rectangle GetVisibleTilesArea(Vector2 position)
    {
        Rectangle rect = new(position.ToPoint(), Size.ToPoint());

        int xStart = (Math.Max(0, rect.Left) - rect.X) / Constants.TileSize;
        int yStart = (Math.Max(0, rect.Top) - rect.Y) / Constants.TileSize;
        int xEnd = (int)Math.Ceiling((Math.Min((double)Engine.ScreenCamera.GameResolution.X, rect.Right) - rect.X) / Constants.TileSize);
        int yEnd = (int)Math.Ceiling((Math.Min((double)Engine.ScreenCamera.GameResolution.Y, rect.Bottom) - rect.Y) / Constants.TileSize);

        return new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
    }

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position)
    {
        Rectangle visibleTilesArea = GetVisibleTilesArea(position);

        float absTileY = position.Y + visibleTilesArea.Y * Constants.TileSize;

        for (int tileY = visibleTilesArea.Top; tileY < visibleTilesArea.Bottom; tileY++)
        {
            float absTileX = position.X + visibleTilesArea.X * Constants.TileSize;

            for (int tileX = visibleTilesArea.Left; tileX < visibleTilesArea.Right; tileX++)
            {
                byte type = CollisionMap[tileY * Width + tileX];
                
                if (type != 0xFF)
                {
                    renderer.Draw(
                        texture: _tex, 
                        position: new Vector2(absTileX, absTileY), 
                        sourceRectangle: new Rectangle((type % CollisionTileSetWidth) * CollisionTileSize, (type / CollisionTileSetWidth) * CollisionTileSize, CollisionTileSize, CollisionTileSize), 
                        rotation: 0,
                        origin: Vector2.Zero, 
                        scale: new Vector2(Constants.TileSize / (float)CollisionTileSize),
                        effects: SpriteEffects.None);
                }

                absTileX += Constants.TileSize;
            }

            absTileY += Constants.TileSize;
        }
    }
}