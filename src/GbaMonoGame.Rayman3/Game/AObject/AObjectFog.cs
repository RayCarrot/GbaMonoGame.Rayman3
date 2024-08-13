using System;
using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

// Custom object for rendering fog. The game doesn't do this - it uses a normal AnimatedObject. However, there
// are issues with how it's implemented. The game has sprites wrap around at 512 because of the x position
// in the OAM being a 9-bit signed value, but the wrapping isn't seamless and causes sprites to overlap. This
// is an issue here because of alpha being used, so the overlapping sprites would have their alpha blend together.
public class AObjectFog : AObject
{
    #region Constructor

    public AObjectFog(AnimatedObjectResource resource)
    {
        Resource = resource;

        // The sprites in the first two channels are the only unique sprites. So we can use these and tile them across.
        SpriteChannels = new[]
        {
            resource.Animations[0].Channels[0],
            resource.Animations[0].Channels[1],
        };
    }

    #endregion

    #region Public Properties

    public const int Width = SpriteWidth * SpritesCount;
    public const int SpriteWidth = 32;
    public const int SpritesCount = 2;

    public AnimatedObjectResource Resource { get; }
    public AnimationChannel[] SpriteChannels { get; }

    public bool IsAlphaBlendEnabled { get; set; }
    public float Alpha { get; set; }
    public float GbaAlpha
    {
        get => Alpha * 16;
        set => Alpha = value / 16;
    }

    #endregion

    #region Private Methods

    private void DrawSprite(AnimationChannel channel, Vector2 screenPos)
    {
        Sprite sprite = new()
        {
            Texture = Engine.TextureCache.GetOrCreateObject(
                pointer: Resource.Offset,
                id: channel.TileIndex * Resource.PalettesCount + channel.PalIndex,
                data: new SpriteDefine(Resource, channel.SpriteShape, channel.SpriteSize, channel.PalIndex, channel.TileIndex),
                createObjFunc: static x =>
                {
                    Palette palette = Engine.PaletteCache.GetOrCreateObject(
                        pointer: x.Resource.Palettes.Offset,
                        id: x.PaletteIndex,
                        data: x.Resource.Palettes.Palettes[x.PaletteIndex],
                        createObjFunc: p => new Palette(p));

                    return new SpriteTexture2D(x.Resource, x.SpriteShape, x.SpriteSize, palette, x.TileIndex);
                }),
            Position = new Vector2(screenPos.X, screenPos.Y),
            FlipX = false,
            FlipY = false,
            Priority = SpritePriority,
            Center = true,
            AffineMatrix = null,
            Alpha = IsAlphaBlendEnabled ? Alpha : null,
            Camera = Camera
        };

        Gfx.AddSprite(sprite);
    }

    #endregion

    #region Public Methods

    public override void Execute(Action<short> soundEventCallback)
    {
        Vector2 pos = GetAnchoredPosition();

        float camWidth = Camera.Resolution.X;
        for (int i = 0; i < camWidth / SpriteWidth + SpritesCount; i++)
            DrawSprite(SpriteChannels[i % SpritesCount], pos + new Vector2(SpriteWidth * i, 0));
    }

    #endregion

    #region Data Types

    private readonly struct SpriteDefine
    {
        public SpriteDefine(AnimatedObjectResource resource, int spriteShape, int spriteSize, int paletteIndex, int tileIndex)
        {
            Resource = resource;
            PaletteIndex = paletteIndex;
            SpriteShape = spriteShape;
            SpriteSize = spriteSize;
            TileIndex = tileIndex;
        }

        public AnimatedObjectResource Resource { get; }
        public int SpriteShape { get; }
        public int SpriteSize { get; }
        public int PaletteIndex { get; }
        public int TileIndex { get; }
    }

    #endregion
}