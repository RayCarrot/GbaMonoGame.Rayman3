﻿using System;
using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame.TgxEngine;

public class TileMapScreenRenderer : IScreenRenderer
{
    public TileMapScreenRenderer(
        Pointer cachePointer, 
        int width, 
        int height, 
        MapTile[] tileMap, 
        byte[] tileSet, 
        PaletteTexture paletteTexture, 
        bool is8Bit)
    {
        CachePointer = cachePointer;
        Width = width;
        Height = height;
        TileMap = tileMap;
        TileSet = tileSet;
        PaletteTexture = paletteTexture;
        Is8Bit = is8Bit;

        ReplacedTiles = new Dictionary<int, int>();
    }

    private Dictionary<int, int> ReplacedTiles { get; }

    public Pointer CachePointer { get; }
    public int Width { get; }
    public int Height { get; }
    public MapTile[] TileMap { get; }
    public byte[] TileSet { get; }
    public PaletteTexture PaletteTexture { get; }
    public bool Is8Bit { get; }

    private Rectangle GetVisibleTilesArea(Vector2 position, GfxScreen screen)
    {
        Box renderBox = new(position, GetSize(screen));

        int xStart = (int)((Math.Max(0, renderBox.MinX) - renderBox.MinX) / Tile.Size);
        int yStart = (int)((Math.Max(0, renderBox.MinY) - renderBox.MinY) / Tile.Size);
        int xEnd = (int)Math.Ceiling((Math.Min(screen.Camera.Resolution.X, renderBox.MaxX) - renderBox.MinX) / Tile.Size);
        int yEnd = (int)Math.Ceiling((Math.Min(screen.Camera.Resolution.Y, renderBox.MaxY) - renderBox.MinY) / Tile.Size);

        // Make sure we don't go out of bounds. Only needed if the camera shows more than the actual map, which isn't usually the case.
        xEnd = Math.Min(xEnd, Width);
        yEnd = Math.Min(yEnd, Height);

        return new Rectangle(xStart, yStart, xEnd - xStart, yEnd - yStart);
    }

    public void ReplaceTile(int originalTileIndex, int newTileIndex)
    {
        ReplacedTiles[originalTileIndex] = newTileIndex;
    }

    public Vector2 GetSize(GfxScreen screen) => new(Width * Tile.Size, Height * Tile.Size);

    public void Draw(GfxRenderer renderer, GfxScreen screen, Vector2 position, Color color)
    {
        renderer.BeginRender(new RenderOptions(screen.IsAlphaBlendEnabled, PaletteTexture, screen.Camera));

        Rectangle visibleTilesArea = GetVisibleTilesArea(position, screen);

        LocationCache<Texture2D> textureCache = Engine.TextureCache.GetOrCreateLocationCache(CachePointer);

        float absTileY = position.Y + visibleTilesArea.Y * Tile.Size;

        for (int tileY = visibleTilesArea.Top; tileY < visibleTilesArea.Bottom; tileY++)
        {
            float absTileX = position.X + visibleTilesArea.X * Tile.Size;

            for (int tileX = visibleTilesArea.Left; tileX < visibleTilesArea.Right; tileX++)
            {
                MapTile tile = TileMap[tileY * Width + tileX];

                int tileIndex = tile.TileIndex;

                if (tileIndex != 0)
                {
                    if (ReplacedTiles.TryGetValue(tileIndex, out int newTileIndex))
                        tileIndex = newTileIndex;

                    Texture2D tex = textureCache.GetOrCreateObject(
                        id: tileIndex,
                        data: new TileDefine(TileSet, tileIndex, tile.PaletteIndex, Is8Bit),
                        createObjFunc: static t => new IndexedTiledTexture2D(
                            tileSet: t.TileSet, 
                            tileIndex: t.TileIndex - 1, 
                            is8Bit: t.Is8Bit,
                            colorOffset: t.PaletteIndex * 16));

                    SpriteEffects effects = SpriteEffects.None;

                    if (tile.FlipX)
                        effects |= SpriteEffects.FlipHorizontally;
                    if (tile.FlipY)
                        effects |= SpriteEffects.FlipVertically;

                    renderer.Draw(tex, new Vector2(absTileX, absTileY), effects, color);
                }

                absTileX += Tile.Size;
            }

            absTileY += Tile.Size;
        }
    }

    private readonly struct TileDefine(byte[] tileSet, int tileIndex, int paletteIndex, bool is8Bit)
    {
        public byte[] TileSet { get; } = tileSet;
        public int TileIndex { get; } = tileIndex;
        public int PaletteIndex { get; } = paletteIndex;
        public bool Is8Bit { get; } = is8Bit;
    }
}