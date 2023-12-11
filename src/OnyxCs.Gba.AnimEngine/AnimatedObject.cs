﻿using System;
using System.Collections.Generic;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.AnimEngine;

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
    }

    #endregion

    #region Private Fields

    private Vector2 _screenPos;
    private int _currentAnimation;
    private int _currentFrame;

    #endregion

    #region Public Properties

    public AnimatedObjectResource Resource { get; set; }

    // Flags
    public bool IsSoundEnabled { get; set; }
    public bool IsDynamic { get; set; } // TODO: If not dynamic we might want to pre-load the sprites?
    public bool EndOfAnimation { get; set; }
    public bool HasExecutedFrame { get; set; }
    public bool IsPaused { get; set; }

    public Vector2 ScreenPos
    {
        get
        {
            float x = _screenPos.X;
            float y = _screenPos.Y;

            if ((Anchor & ScreenAnchor.Right) != 0)
                x = Engine.ScreenCamera.GameResolution.X - x;
            if ((Anchor & ScreenAnchor.Bottom) != 0)
                y = Engine.ScreenCamera.GameResolution.Y - y;

            return new Vector2(x, y);
        }
        set => _screenPos = value;
    }

    public ScreenAnchor Anchor { get; set; }

    public bool FlipX { get; set; }
    public bool FlipY { get; set; }

    public int CurrentAnimation
    {
        get => _currentAnimation;
        set
        {
            _currentAnimation = value;
            _currentFrame = 0;
            ChannelIndex = 0;
            Timer = GetAnimation().Speed;
            EndOfAnimation = false;
            HasExecutedFrame = false;
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
            HasExecutedFrame = false;
            EndOfAnimation = false;
        }
    }

    public int ChannelIndex { get; set; }
    public int Timer { get; set; }

    public AffineMatrix? AffineMatrix { get; set; }

    public BoxTable BoxTable { get; set; }

    #endregion

    #region Private Methods

    private Animation GetAnimation() => Resource.Animations[CurrentAnimation];

    private IEnumerable<AnimationChannel> EnumerateCurrentChannels()
    {
        Animation anim = GetAnimation();

        for (int i = 0; i < anim.ChannelsPerFrame[CurrentFrame]; i++)
            yield return anim.Channels[i + ChannelIndex];
    }

    private void PlayChannelBox()
    {
        if (HasExecutedFrame || BoxTable == null)
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

    private void StepTimer()
    {
        Animation anim = GetAnimation();

        EndOfAnimation = false;

        if (IsPaused)
        {
            if (Timer != 0)
                HasExecutedFrame = true;

            if (CurrentFrame >= anim.FramesCount)
                EndOfAnimation = true;

            return;
        }

        if (Timer == 0)
        {
            ChannelIndex += anim.ChannelsPerFrame[CurrentFrame];
            _currentFrame++;
            Timer = anim.Speed;
            HasExecutedFrame = false;

            if (CurrentFrame < anim.FramesCount)
                return;

            if (anim.DoNotRepeat)
            {
                _currentFrame--;
                ChannelIndex -= anim.ChannelsPerFrame[CurrentFrame];
                HasExecutedFrame = true;
            }
            else
            {
                _currentFrame = 0;
                ChannelIndex = 0;
                HasExecutedFrame = false;
            }

            EndOfAnimation = true;
        }
        else
        {
            Timer--;
            HasExecutedFrame = true;
        }
    }

    #endregion

    #region Public Methods

    public void ExecuteUnframed()
    {
        EndOfAnimation = false;
        PlayChannelBox();
        StepTimer();
    }

    public override void Execute(AnimationSpriteManager animationSpriteManager, Action<ushort> soundEventCallback)
    {
        Animation anim = GetAnimation();

        if (!HasExecutedFrame && BoxTable != null)
        {
            BoxTable.AttackBox = new Box();
            BoxTable.VulnerabilityBox = new Box();
        }

        EndOfAnimation = false;

        // --- At this point the engine loads dynamic data which we don't need to ---

        if (anim.Idx_PaletteCycleAnimation != 0 && !HasExecutedFrame)
            throw new NotImplementedException("Not implemented animations with palette data");

        // Enumerate every channel
        foreach (AnimationChannel channel in EnumerateCurrentChannels())
        {
            // Play the channel based on the type
            switch (channel.ChannelType)
            {
                case AnimationChannelType.Sprite:

                    if (channel.ObjectMode == OBJ_ATTR_ObjectMode.HIDE)
                        continue;

                    // On GBA the size of a sprite is determined based on
                    // the shape and size values. We use these to get the
                    // actual width and height of the sprite.
                    Constants.Size shape = Constants.GetSpriteShape(channel.SpriteShape, channel.SpriteSize);

                    // Get x position
                    float xPos = channel.XPosition;

                    if (!FlipX)
                        xPos += ScreenPos.X;
                    else
                        xPos = ScreenPos.X - xPos - shape.Width;

                    // Get y position
                    float yPos = channel.YPosition;

                    if (!FlipY)
                        yPos += ScreenPos.Y;
                    else
                        yPos = ScreenPos.Y - yPos - shape.Height;

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

                    // Add the sprite to vram. In the original engine this part
                    // is more complicated. If the object is dynamic then it loads
                    // in the data to vram first, then for all sprites it adds an
                    // entry to the list of object attributes in OAM memory.
                    Texture2D tex = animationSpriteManager.GetSpriteTexture(
                        resource: Resource,
                        spriteShape: channel.SpriteShape,
                        spriteSize: channel.SpriteSize,
                        tileIndex: channel.TileIndex,
                        paletteIndex: channel.PalIndex);
                    Gfx.AddSprite(new Sprite(
                        texture: tex,
                        position: new Vector2(xPos, yPos),
                        flipX: channel.FlipX ^ FlipX,
                        flipY: channel.FlipY ^ FlipY,
                        priority: SpritePriority,
                        affineMatrix: affineMatrix));
                    break;

                case AnimationChannelType.Sound:
                    if (!HasExecutedFrame && IsSoundEnabled)
                        soundEventCallback(channel.SoundId);
                    break;

                case AnimationChannelType.DisplacementVector:
                    if (!HasExecutedFrame)
                    {
                        // Unused in Rayman 3, so we can probably ignore this
                        throw new NotImplementedException("Not implemented displacement vectors");
                    }
                    break;

                case AnimationChannelType.AttackBox:
                    if (!HasExecutedFrame && BoxTable != null)
                        BoxTable.AttackBox = new Box(channel.Box);
                    break;

                case AnimationChannelType.VulnerabilityBox:
                    if (!HasExecutedFrame && BoxTable != null)
                        BoxTable.VulnerabilityBox = new Box(channel.Box);
                    break;
            }
        }

        PlayChannelBox();
        StepTimer();
    }

    #endregion
}