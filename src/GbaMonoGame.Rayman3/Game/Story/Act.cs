using System;
using System.Collections.Generic;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public abstract class Act : Frame
{
    #region Private Properties

    private Dictionary<Bitmap, IScreenRenderer> CachedTextureRenderers { get; } = new();

    private TransitionsFX TransitionsFX { get; set; }
    private ActResource ActResource { get; set; }
    private AnimationPlayer AnimationPlayer { get; set; }
    private GfxScreen BitmapScreen { get; set; }

    private SpriteTextObject[] TextObjects { get; set; } // One for every line of text
    private AnimatedObject NextTextSymbol { get; set; }
    private AnimatedObject SkipSymbol { get; set; } // N-Gage exclusive

    private ushort CurrentFrameIndex { get; set; }
    private ushort CurrentTextLine { get; set; }
    private string[] CurrentText { get; set; }

    private bool IsFadingOut { get; set; }

    private bool IsTransitioningTextOut { get; set; }
    private bool IsTransitioningTextIn { get; set; }
    private byte TransitionTextOutDelay { get; set; }
    private float TextTransitionValue { get; set; } = 1;

    #endregion

    #region Protected Properties

    protected bool IsAutomatic { get; set; } // N-Gage exclusive

    #endregion

    #region Public Properties

    public bool IsFinished { get; private set; }

    #endregion

    #region Private Methods

    private void NextText()
    {
        ActFrame frame = ActResource.Frames.Value[CurrentFrameIndex];
        int textId = frame.TextId;

        if (textId == -1)
            return;

        CurrentText = Localization.GetText(ActResource.TextBankId, textId);

        float centerX = Engine.GameViewPort.GameResolution.X / 2f;
        float baseY = Engine.Settings.Platform switch
        {
            Platform.GBA => 129,
            Platform.NGage => 135,
            _ => throw new UnsupportedPlatformException()
        };
        const int lineHeight = 14;

        for (int i = 0; i < TextObjects.Length; i++)
        {
            SpriteTextObject textObj = TextObjects[i];

            if (CurrentTextLine < CurrentText.Length)
            {
                string line = CurrentText[CurrentTextLine];

                textObj.Text = line;
                textObj.ScreenPos = new Vector2(centerX - textObj.GetStringWidth() / 2f, baseY + lineHeight * i);

                CurrentTextLine++;
            }
            else
            {
                textObj.Text = String.Empty;
                textObj.ScreenPos = new Vector2(centerX, baseY + lineHeight * i);
            }
        }
    }

    private void InitTextTransition()
    {
        IsTransitioningTextOut = true;
        IsTransitioningTextIn = true;
        TextTransitionValue = 1;

        foreach (SpriteTextObject textObj in TextObjects)
            textObj.AffineMatrix = AffineMatrix.Identity;
    }

    private void TransitionTextOut()
    {
        TextTransitionValue++;

        foreach (SpriteTextObject textObj in TextObjects)
            textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);

        if (TextTransitionValue > 8)
        {
            IsTransitioningTextOut = false;
            IsTransitioningTextIn = true;
            TransitionTextOutDelay = 4;
        }
    }

    private void TransitionTextIn()
    {
        if (TransitionTextOutDelay == 0)
        {
            TextTransitionValue--;

            if (TextTransitionValue < 1)
            {
                TextTransitionValue = 1;
                IsTransitioningTextOut = false;
                IsTransitioningTextIn = false;
            }

            foreach (SpriteTextObject textObj in TextObjects)
                textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);
        }
        else
        {
            if (TransitionTextOutDelay == 2)
                NextText();

            TransitionTextOutDelay--;
        }
    }

    #endregion

    #region Protected Methods

    protected void NextFrame(bool freezeFrame)
    {
        if (!freezeFrame)
            CurrentFrameIndex++;

        if (CurrentFrameIndex > ActResource.LastFrameIndex)
        {
            IsFinished = true;
            return;
        }

        ActFrame frame = ActResource.Frames.Value[CurrentFrameIndex];

        if (frame.MusicSongEvent != Rayman3SoundEvent.None)
            SoundEventsManager.ProcessEvent(frame.MusicSongEvent);

        if (!CachedTextureRenderers.TryGetValue(frame.Bitmap, out IScreenRenderer renderer))
        {
            renderer = new TextureScreenRenderer(new BitmapTexture2D(
                width: (int)Engine.GameViewPort.OriginalGameResolution.X,
                height: (int)Engine.GameViewPort.OriginalGameResolution.Y,
                bitmap: frame.Bitmap.ImgData,
                palette: new Palette(frame.Palette)));
            CachedTextureRenderers[frame.Bitmap] = renderer;
        }

        BitmapScreen.Renderer = renderer;

        CurrentTextLine = 0;
        CurrentText = null;

        foreach (SpriteTextObject textObj in TextObjects)
            textObj.Text = String.Empty;

        NextText();
    }

    protected void Init(ActResource resource)
    {
        Engine.GameViewPort.SetResolutionBoundsToOriginalResolution();

        TransitionsFX = new TransitionsFX(false);
        TransitionsFX.FadeInInit(1 / 16f);
        
        IsFadingOut = false;

        ActResource = resource;

        if (resource.StartMusicSoundEvent != Rayman3SoundEvent.None)
            SoundEventsManager.ProcessEvent(resource.StartMusicSoundEvent);

        AnimationPlayer = new AnimationPlayer(false, null);

        if (Engine.Settings.Platform == Platform.GBA)
        {
            TextObjects = new[]
            {
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 129),
                    FontSize = FontSize.Font16,
                    Color = TextColor.Story,
                },
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 143),
                    FontSize = FontSize.Font16,
                    Color = TextColor.Story,
                }
            };
        }
        else if (Engine.Settings.Platform == Platform.NGage)
        {
            // N-Gage has 3 lines of text
            TextObjects = new[]
            {
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 135),
                    FontSize = FontSize.Font16,
                    Color = TextColor.Story,
                },
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 149),
                    FontSize = FontSize.Font16,
                    Color = TextColor.Story,
                },
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 163),
                    FontSize = FontSize.Font16,
                    Color = TextColor.Story,
                }
            };
        }
        else
        {
            throw new UnsupportedPlatformException();
        }

        AnimatedObjectResource nextSymbolResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.StoryNextTextAnimations);
        NextTextSymbol = new AnimatedObject(nextSymbolResource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = Engine.Settings.Platform switch
            {
                Platform.GBA => new Vector2(230, 158),
                Platform.NGage => new Vector2(166, 200),
                _ => throw new UnsupportedPlatformException()
            }
        };

        if (Engine.Settings.Platform == Platform.NGage)
        {
            AnimatedObjectResource skipSymbolResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.NGageButtons);
            SkipSymbol = new AnimatedObject(skipSymbolResource, false)
            {
                IsFramed = true,
                CurrentAnimation = 10 + Localization.LanguageUiIndex,
                ScreenPos = new Vector2(-1, 190)
            };
        }

        BitmapScreen = new GfxScreen(2)
        {
            IsEnabled = true,
            Priority = 1,
            // Center the screen
            Offset = -(Engine.GameViewPort.GameResolution - Engine.GameViewPort.OriginalGameResolution) / 2,
        };
        Gfx.AddScreen(BitmapScreen);

        NextFrame(true);
    }

    #endregion

    #region Pubic Override Methods

    public override void UnInit()
    {
        foreach (IScreenRenderer renderer in CachedTextureRenderers.Values)
            renderer.Dispose();

        AnimationPlayer.UnInit();

        if (ActResource.StopMusicSoundEvent != Rayman3SoundEvent.None)
            SoundEventsManager.ProcessEvent(ActResource.StopMusicSoundEvent);
    }

    public override void Step()
    {
        if (!TransitionsFX.IsFadeOutFinished)
        {
            TransitionsFX.StepFadeOut();
        }
        else if (!TransitionsFX.IsFadeInFinished)
        {
            TransitionsFX.StepFadeIn();
        }
        else if (IsFadingOut)
        {
            IsFadingOut = false;
            TransitionsFX.FadeInInit(1 / 16f);
            NextFrame(false);
        }
        else
        {
            // Skip cutscene
            if (!IsAutomatic && JoyPad.CheckSingle(GbaInput.Start)) // TODO: N-Gage checks other input?
            {
                CurrentFrameIndex = ActResource.LastFrameIndex;
                TransitionsFX.FadeOutInit(1 / 16f);
                IsFadingOut = true;
                SoundEventsManager.StopAllSongs();
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Valid01_Mix01);
            }
            else if (IsTransitioningTextOut)
            {
                TransitionTextOut();
            }
            else if (IsTransitioningTextIn)
            {
                TransitionTextIn();
            }
            else if (!IsAutomatic && JoyPad.CheckSingle(GbaInput.A)) // TODO: N-Gage allows a lot more buttons here
            {
                if (ActResource.Frames.Value[CurrentFrameIndex].TextId == -1 ||
                    CurrentTextLine >= CurrentText.Length)
                {
                    TransitionsFX.FadeOutInit(1 / 16f);
                    IsFadingOut = true;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Valid01_Mix01);
                }
                else
                {
                    InitTextTransition();
                }
            }
        }

        if (!IsAutomatic && (CurrentFrameIndex != ActResource.LastFrameIndex || CurrentTextLine < CurrentText.Length))
        {
            if ((GameTime.ElapsedFrames & 0x10) != 0)
                AnimationPlayer.PlayFront(NextTextSymbol);
        }

        foreach (SpriteTextObject textObj in TextObjects)
            AnimationPlayer.PlayFront(textObj);

        if (!IsAutomatic && Engine.Settings.Platform == Platform.NGage)
            AnimationPlayer.PlayFront(SkipSymbol);

        AnimationPlayer.Execute();
    }

    #endregion
}