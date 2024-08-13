using System;
using System.Collections.Generic;
using System.IO;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework.Graphics;

namespace GbaMonoGame.AnimEngine;

// The game has different types of AnimatedObject. They however all act the same, just with some different properties
// depending on the class type. Doing that here would be a mess, so better we handle it using properties in this class.

/// <summary>
/// An object which can execute a sprite animation
/// </summary>
public class AnimatedObject : AObject
{
    #region Constructor

    public AnimatedObject(AnimatedObjectResource resource, bool isDynamic)
    {
        IsSoundEnabled = true;
        IsDynamic = isDynamic;
        Resource = resource;
        VisibleSpriteChannels = UInt32.MaxValue;
    }

    #endregion

    #region Private Fields

    private int _currentAnimation;
    private int _currentFrame;

    #endregion

    #region Public Properties

    public AnimatedObjectResource Resource { get; }

    // Flags
    public bool IsSoundEnabled { get; set; }
    public bool IsDynamic { get; set; } // TODO: If not dynamic we might want to pre-load the sprites?
    public bool EndOfAnimation { get; set; }
    public bool IsDelayMode { get; set; }
    public bool IsPaused { get; set; }

    public uint VisibleSpriteChannels { get; set; }

    public bool IsBackSprite { get; set; }

    public bool FlipX { get; set; }
    public bool FlipY { get; set; }

    public bool IsFramed { get; set; }

    public int CurrentAnimation
    {
        get => _currentAnimation;
        set
        {
            _currentAnimation = value;
            Rewind();
        }
    }

    public int CurrentFrame
    {
        get => _currentFrame;
        set
        {
            Animation anim = GetAnimation();

            if (value != _currentFrame)
            {
                if (value == 0)
                {
                    ChannelIndex = 0;
                }
                else if (value > _currentFrame)
                {
                    int framesDiff = value - _currentFrame;

                    for (int i = 0; i < framesDiff; i++)
                        ChannelIndex += anim.ChannelsPerFrame[_currentFrame + i];
                }
                else
                {
                    int framesDiff = _currentFrame - value;

                    for (int i = 0; i < framesDiff; i++)
                        ChannelIndex += anim.ChannelsPerFrame[_currentFrame - i - 1];
                }

                _currentFrame = value;
            }

            Timer = anim.Speed;
            IsDelayMode = false;
            EndOfAnimation = false;
        }
    }

    public int ChannelIndex { get; set; }
    public int Timer { get; set; }

    public AffineMatrix? AffineMatrix { get; set; }
    public int BasePaletteIndex { get; set; }

    public bool IsAlphaBlendEnabled { get; set; }
    public float Alpha { get; set; }
    public float GbaAlpha
    {
        get => Alpha * 16;
        set => Alpha = value / 16;
    }

    public BoxTable BoxTable { get; set; }

    #endregion

    #region Private Methods

    private IEnumerable<AnimationChannel> EnumerateCurrentChannels()
    {
        Animation anim = GetAnimation();

        for (int i = 0; i < anim.ChannelsPerFrame[CurrentFrame]; i++)
            yield return anim.Channels[i + ChannelIndex];
    }

    private static Texture2D CreateSpriteTexture(SpriteDefine spriteDefine)
    {
        AnimatedObjectResource resource = spriteDefine.Resource;

        Palette palette = Engine.PaletteCache.GetOrCreateObject(
            pointer: resource.Palettes.Offset,
            id: spriteDefine.PaletteIndex,
            data: resource.Palettes.Palettes[spriteDefine.PaletteIndex],
            createObjFunc: p => new Palette(p));

        SpriteTexture2D tex = new(resource, spriteDefine.SpriteShape, spriteDefine.SpriteSize, palette, spriteDefine.TileIndex);

        if (Engine.Config.DumpSprites)
        {
            string outputDir = Path.Combine("Sprites", resource.Offset.StringAbsoluteOffset);
            Directory.CreateDirectory(outputDir);
            using Stream fileStream = File.Create(Path.Combine(outputDir, $"{spriteDefine.TileIndex}_{spriteDefine.PaletteIndex}.png"));
            tex.SaveAsPng(fileStream, tex.Width, tex.Height);
        }

        return tex;
    }

    #endregion

    #region Public Methods

    public Animation GetAnimation() => Resource.Animations[CurrentAnimation];

    public bool IsChannelVisible(int channel) => (VisibleSpriteChannels & (1 << channel)) != 0;
    public void SetChannelVisible(int channel) => VisibleSpriteChannels = (uint)((int)VisibleSpriteChannels | (1 << channel));
    public void SetChannelInvisible(int channel) => VisibleSpriteChannels = (uint)((int)VisibleSpriteChannels & ~(1 << channel));

    public void Rewind()
    {
        _currentFrame = 0;
        ChannelIndex = 0;
        Timer = GetAnimation().Speed;
        IsDelayMode = false;
        EndOfAnimation = false;
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;

    public void ComputeNextFrame()
    {
        EndOfAnimation = false;

        PlayChannelBox();

        Animation anim = GetAnimation();

        EndOfAnimation = false;

        if (IsPaused)
        {
            if (Timer != 0)
                IsDelayMode = true;

            if (CurrentFrame >= anim.FramesCount)
                EndOfAnimation = true;

            return;
        }

        if (Timer == 0)
        {
            ChannelIndex += anim.ChannelsPerFrame[CurrentFrame];
            _currentFrame++;
            Timer = anim.Speed;
            IsDelayMode = false;

            if (CurrentFrame < anim.FramesCount)
                return;

            if (anim.DoNotRepeat)
            {
                _currentFrame--;
                ChannelIndex -= anim.ChannelsPerFrame[CurrentFrame];
                IsDelayMode = true;
            }
            else
            {
                _currentFrame = 0;
                ChannelIndex = 0;
                IsDelayMode = false;
            }

            EndOfAnimation = true;
        }
        else
        {
            Timer--;
            IsDelayMode = true;
        }
    }

    public void PlayChannelBox()
    {
        if (IsDelayMode || BoxTable == null)
            return;

        BoxTable.AttackBox = new Box();
        BoxTable.VulnerabilityBox = new Box();

        foreach (AnimationChannel channel in EnumerateCurrentChannels())
        {
            switch (channel.ChannelType)
            {
                case AnimationChannelType.AttackBox:
                    BoxTable.AttackBox = new Box(channel.Box);
                    break;

                case AnimationChannelType.VulnerabilityBox:
                    BoxTable.VulnerabilityBox = new Box(channel.Box);
                    break;
            }
        }
    }

    public void PlayChannelSound(AnimationPlayer animationPlayer)
    {
        if (IsDelayMode)
            return;

        foreach (AnimationChannel channel in EnumerateCurrentChannels())
        {
            if (channel.ChannelType == AnimationChannelType.Sound)
                animationPlayer.SoundEventRequest(channel.SoundId);
        }
    }

    public void FrameChannelSprite()
    {
        // TODO: Implement
        Logger.NotImplemented("Not implemented framing channel sprites");
    }

    public override void Execute(Action<short> soundEventCallback)
    {
        Animation anim = GetAnimation();

        if (!IsDelayMode && BoxTable != null)
        {
            BoxTable.AttackBox = new Box();
            BoxTable.VulnerabilityBox = new Box();
        }

        EndOfAnimation = false;

        // --- At this point the engine loads dynamic data which we don't need to ---

        if (anim.Idx_PaletteCycleAnimation != 0 && !IsDelayMode)
            throw new NotImplementedException("Not implemented animations with palette data");

        // Enumerate every channel
        int channelIndex = 0;
        foreach (AnimationChannel channel in EnumerateCurrentChannels())
        {
            // Play the channel based on the type
            switch (channel.ChannelType)
            {
                case AnimationChannelType.Sprite:

                    if (channel.ObjectMode == OBJ_ATTR_ObjectMode.HIDE || !IsChannelVisible(channelIndex))
                        break;

                    // On GBA the size of a sprite is determined based on
                    // the shape and size values. We use these to get the
                    // actual width and height of the sprite.
                    Constants.Size shape = Constants.GetSpriteShape(channel.SpriteShape, channel.SpriteSize);

                    // Get x position
                    float xPos = channel.XPosition;

                    Vector2 pos = GetAnchoredPosition();

                    if (!FlipX)
                        xPos += pos.X;
                    else
                        xPos = pos.X - xPos - shape.Width;

                    // Get y position
                    float yPos = channel.YPosition;

                    if (!FlipY)
                        yPos += pos.Y;
                    else
                        yPos = pos.Y - yPos - shape.Height;

                    AffineMatrix? affineMatrix = null;

                    // Get the matrix if it's affine
                    if (channel.ObjectMode == OBJ_ATTR_ObjectMode.REG && AffineMatrix != null)
                    {
                        affineMatrix = AffineMatrix.Value;
                    }
                    else if (channel.ObjectMode is OBJ_ATTR_ObjectMode.AFF or OBJ_ATTR_ObjectMode.AFF_DBL)
                    {
                        AffineMatrixResource matrix = anim.AffineMatrices.Matrices[channel.AffineMatrixIndex];
                        affineMatrix = new AffineMatrix(
                            pa: matrix.Pa,
                            pb: matrix.Pb,
                            pc: matrix.Pc,
                            pd: matrix.Pd);
                    }

                    int paletteIndex = BasePaletteIndex + channel.PalIndex;

                    // Add the sprite to vram. In the original engine this part
                    // is more complicated. If the object is dynamic then it loads
                    // in the data to vram first, then for all sprites it adds an
                    // entry to the list of object attributes in OAM memory.
                    Sprite sprite = new()
                    {
                        Texture = Engine.TextureCache.GetOrCreateObject(
                            pointer: Resource.Offset, 
                            id: channel.TileIndex * Resource.PalettesCount + paletteIndex,
                            data: new SpriteDefine(Resource, channel.SpriteShape, channel.SpriteSize, paletteIndex, channel.TileIndex),
                            createObjFunc: CreateSpriteTexture),
                        Position = new Vector2(xPos, yPos),
                        FlipX = channel.FlipX ^ FlipX,
                        FlipY = channel.FlipY ^ FlipY,
                        Priority = SpritePriority,
                        Center = true,
                        AffineMatrix = affineMatrix,
                        Alpha = IsAlphaBlendEnabled ? Alpha : null,
                        Camera = Camera
                    };

                    if (IsBackSprite)
                        Gfx.AddBackSprite(sprite);
                    else
                        Gfx.AddSprite(sprite);
                    break;

                case AnimationChannelType.Sound:
                    if (!IsDelayMode && IsSoundEnabled)
                        soundEventCallback(channel.SoundId);
                    break;

                case AnimationChannelType.DisplacementVector:
                    if (!IsDelayMode)
                    {
                        // Appears mostly unused in Rayman 3. Only used for ship in final boss and Ly, but seems to always be 0 anyway.
                        Logger.NotImplemented("Not implemented displacement vectors");
                    }
                    break;

                case AnimationChannelType.AttackBox:
                    if (!IsDelayMode && BoxTable != null)
                        BoxTable.AttackBox = new Box(channel.Box);
                    break;

                case AnimationChannelType.VulnerabilityBox:
                    if (!IsDelayMode && BoxTable != null)
                        BoxTable.VulnerabilityBox = new Box(channel.Box);
                    break;
            }

            channelIndex++;
        }

        ComputeNextFrame();
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