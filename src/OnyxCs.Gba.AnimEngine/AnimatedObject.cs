using System;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OnyxCs.Gba.AnimEngine;

// TODO: The game has different types of AnimatedObject. They however all act the same, just with some different properties
//       depending on the class type. Doing that here would be a mess, so better we handle it using properties in this class.

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

    #region Public Properties

    public AnimatedObjectResource Resource { get; set; }

    // Flags
    public bool IsSoundEnabled { get; set; }
    public bool IsDynamic { get; set; } // TODO: If not dynamic we might want to pre-load the sprites?
    public bool EndOfAnimation { get; set; }
    public bool EnableBox { get; set; }
    public bool HasExecutedFrame { get; set; }
    public bool OnlyShowVisibleSpriteChannels { get; set; }
    public bool IsPaused { get; set; }
    public uint VisibleChannels { get; set; } // One bit per channel

    public Vector2 ScreenPos { get; set; }
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }
    public int Priority { get; set; }

    public int AnimationIndex { get; private set; }
    public int FrameIndex { get; private set; }
    public int ChannelIndex { get; set; }
    public int Timer { get; set; }

    public BoxTable BoxTable { get; set; }

    #endregion

    #region Private Methods

    private Rectangle TransformChannelBoxToEngineBox(ChannelBox box) => new(box.MinX, box.MinY, box.MaxX, box.MaxY);

    private void PlayChannelBox()
    {
        if (HasExecutedFrame)
            return;

        ResetBoxTable();

        // TODO: Implement
    }

    private void StepTimer()
    {
        Animation anim = GetAnimation();

        EndOfAnimation = false;

        if (IsPaused)
        {
            if (Timer != 0)
                HasExecutedFrame = true;

            if (FrameIndex >= anim.FramesCount)
                EndOfAnimation = true;

            return;
        }

        if (Timer == 0)
        {
            ChannelIndex += anim.ChannelsPerFrame[FrameIndex];
            FrameIndex++;
            Timer = anim.Speed;
            HasExecutedFrame = false;

            if (FrameIndex < anim.FramesCount)
                return;

            if (anim.DoNotRepeat)
            {
                FrameIndex--;
                ChannelIndex -= anim.ChannelsPerFrame[FrameIndex];
                HasExecutedFrame = true;
            }
            else
            {
                FrameIndex = 0;
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

    public Animation GetAnimation() => Resource.Animations[AnimationIndex];

    public void SetCurrentAnimation(int animation)
    {
        AnimationIndex = animation;
        FrameIndex = 0;
        ChannelIndex = 0;
        Timer = GetAnimation().Speed;
        EndOfAnimation = false;
        HasExecutedFrame = false;
    }

    public void SetCurrentFrame(int frame)
    {
        Animation anim = GetAnimation();

        if (frame != FrameIndex)
        {
            if (frame == 0)
            {
                ChannelIndex = 0;
            }
            else if (frame > FrameIndex)
            {
                int framesDiff = frame - FrameIndex;

                for (int i = 0; i < framesDiff; i++)
                    ChannelIndex += anim.ChannelsPerFrame[FrameIndex + i];
            }
            else
            {
                int framesDiff = FrameIndex - frame;

                for (int i = 0; i < framesDiff; i++)
                    ChannelIndex += anim.ChannelsPerFrame[FrameIndex - i - 1];
            }

            FrameIndex = frame;
        }

        Timer = anim.Speed;
        HasExecutedFrame = false;
        EndOfAnimation = false;
    }

    public void ResetBoxTable()
    {
        if (BoxTable == null)
            return;

        BoxTable.AttackBox = new Rectangle();
        BoxTable.VulnerabilityBox = new Rectangle();
    }

    public override void Execute(AnimationSpriteManager animationSpriteManager, Action<int> soundEventCallback)
    {
        Animation anim = GetAnimation();

        if (EnableBox && !HasExecutedFrame)
            ResetBoxTable();

        EndOfAnimation = false;

        // --- At this point the engine loads dynamic data which we don't need to ---

        if (anim.Idx_PaletteInfo != 0 && !HasExecutedFrame)
            throw new NotImplementedException("Not implemented animations with palette data");

        // Enumerate every channel
        for (int i = 0; i < anim.ChannelsPerFrame[FrameIndex]; i++)
        {
            AnimationChannel channel = anim.Channels[i + ChannelIndex];

            // Play the channel based on the type
            switch (channel.ChannelType)
            {
                case AnimationChannelType.Sprite:
                    if (!OnlyShowVisibleSpriteChannels || (VisibleChannels & (1 << i)) != 0)
                    {
                        // On GBA the size of a sprite is determined based on
                        // the shape and size values. We use these to get the
                        // actual width and height of the sprite.
                        Constants.Size shape = Constants.GetSpriteShape(channel.SpriteShape, channel.SpriteSize);

                        // Affine sprites support scaling and rotation
                        bool isAffine = channel.ObjectMode is OBJ_ATTR_ObjectMode.AFF or OBJ_ATTR_ObjectMode.AFF_DBL;

                        // Get x position
                        float xPos = channel.XPosition;

                        if (!FlipX)
                            xPos += ScreenPos.X;
                        else
                            xPos = ScreenPos.X - xPos - shape.Width;

                        if (isAffine)
                            xPos -= shape.Width / 2f;

                        // Get y position
                        float yPos = channel.YPosition;

                        if (!FlipY)
                            yPos += ScreenPos.Y;
                        else
                            yPos = ScreenPos.Y - yPos - shape.Height;

                        if (isAffine)
                            yPos -= shape.Height / 2f;

                        AffineMatrix affineMatrix = new();

                        // Get the matrix if it's affine
                        if (isAffine)
                        {
                            AffineMatrixResource matrix = anim.AffineMatrices.Matrices[channel.AffineMatrixIndex];
                            affineMatrix = new AffineMatrix(
                                pa: FlipX ? -matrix.Pa : matrix.Pa,
                                pb: FlipY ? -matrix.Pb : matrix.Pb,
                                pc: FlipX ? -matrix.Pc : matrix.Pc,
                                pd: FlipY ? -matrix.Pd : matrix.Pd);
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
                        Gfx.Sprites.Add(new Sprite(
                            texture: tex,
                            position: new Vector2(xPos, yPos),
                            flipX: channel.FlipX,
                            flipY: channel.FlipY,
                            priority: Priority,
                            mode: channel.ObjectMode,
                            affineMatrix: affineMatrix));
                    }
                    break;

                case AnimationChannelType.Sound:
                    if (!HasExecutedFrame && IsSoundEnabled)
                        soundEventCallback(channel.SoundId);
                    break;

                case AnimationChannelType.DisplacementVector:
                    if (!HasExecutedFrame)
                    {
                        // Unused in Rayman 3, so we can probably ignore this for now
                        throw new NotImplementedException("Not implemented displacement vectors");
                    }
                    break;

                case AnimationChannelType.AttackBox:
                    if (!HasExecutedFrame && BoxTable != null)
                        BoxTable.AttackBox = TransformChannelBoxToEngineBox(channel.Box);
                    break;

                case AnimationChannelType.VulnerabilityBox:
                    if (!HasExecutedFrame && BoxTable != null)
                        BoxTable.VulnerabilityBox = TransformChannelBoxToEngineBox(channel.Box);
                    break;
            }
        }

        if (EnableBox && BoxTable != null)
            PlayChannelBox();

        StepTimer();
    }

    #endregion
}