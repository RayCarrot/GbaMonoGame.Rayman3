using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public abstract class Act : Frame
{
    #region Constructor

    protected Act()
    {
        field19_0x74 = 0x10000;
    }

    #endregion

    #region Private Properties

    private Dictionary<ActBitmap, IScreenRenderer> TextureRenderers { get; } = new();

    private ActResource ActResource { get; set; }
    private AnimationPlayer AnimationPlayer { get; set; }
    private GfxScreen BitmapScreen { get; set; }

    private SpriteTextObject TextObj1 { get; set; }
    private SpriteTextObject TextObj2 { get; set; }
    private AnimatedObject NextTextSymbol { get; set; }

    private ushort CurrentFrameIndex { get; set; }
    private ushort CurrentStringIndex { get; set; }

    // Unknown
    private uint field5_0x16 { get; set; }
    private uint field7_0x18 { get; set; }
    private uint field19_0x74 { get; set; }

    #endregion

    #region Public Properties

    public bool IsFinished { get; private set; }

    #endregion

    #region Private Methods

    private void DrawFrame(bool freezeFrame)
    {
        if (!freezeFrame)
            CurrentFrameIndex++;

        if (CurrentFrameIndex > ActResource.FramesCount)
        {
            IsFinished = true;
            return;
        }

        ActFrame frame = ActResource.Frames.Value[CurrentFrameIndex];

        if (frame.MusicSongEvent != Rayman3SoundEvent.None)
            SoundManager.Play(frame.MusicSongEvent);

        if (!TextureRenderers.TryGetValue(frame.Bitmap.Value!, out IScreenRenderer renderer))
        {
            renderer = new TextureScreenRenderer(new BitmapTexture2D(
                width: Engine.ScreenCamera.OriginalGameResolution.X,
                height: Engine.ScreenCamera.OriginalGameResolution.Y, 
                bitmap: frame.Bitmap.Value.ImgData, 
                palette: new Palette(frame.Palette)));
            TextureRenderers[frame.Bitmap.Value] = renderer;
        }

        BitmapScreen.Renderer = renderer;

        field5_0x16 = 1;
        CurrentStringIndex = 0;

        // TODO: Implement
    }

    #endregion

    #region Protected Methods

    protected void Init(ActResource resource)
    {
        //TransitionsFX::Ctor();
        //TransitionsFX::FadeInInit(1);
        field7_0x18 = 0;

        ActResource = resource;

        if (resource.StartMusicSoundEvent != Rayman3SoundEvent.None)
            SoundManager.Play(resource.StartMusicSoundEvent);

        AnimationPlayer = new AnimationPlayer(false);
        SpriteTextObject.Color = new RGB555Color(0x8aa).ToColor();

        TextObj1 = new SpriteTextObject()
        {
            AffineMatrix = AffineMatrix.Identity,
            ScreenPos = new Vector2(4, 129),
        };
        TextObj2 = new SpriteTextObject()
        {
            AffineMatrix = AffineMatrix.Identity,
            ScreenPos = new Vector2(4, 143),
        };

        AnimatedObjectResource animatedObjectResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.StoryNextTextAnimations);
        NextTextSymbol = new AnimatedObject(animatedObjectResource, false)
        {
            CurrentAnimation = 0,
            ScreenPos = new Vector2(230, 158),
        };

        BitmapScreen = new GfxScreen(2)
        {
            IsEnabled = true,
        };
        Gfx.AddScreen(BitmapScreen);

        DrawFrame(true);
    }

    #endregion

    #region Pubic Override Methods

    public override void UnInit()
    {
        if (ActResource.StopMusicSoundEvent != Rayman3SoundEvent.None)
            SoundManager.Play(ActResource.StopMusicSoundEvent);
    }

    public override void Step()
    {

    }

    #endregion
}