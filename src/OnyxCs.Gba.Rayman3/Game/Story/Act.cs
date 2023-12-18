using System;
using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public abstract class Act : Frame
{
    #region Private Properties

    private Dictionary<ActBitmap, IScreenRenderer> CachedTextureRenderers { get; } = new();

    private TransitionsFX TransitionsFX { get; set; }
    private ActResource ActResource { get; set; }
    private AnimationPlayer AnimationPlayer { get; set; }
    private GfxScreen BitmapScreen { get; set; }

    private SpriteTextObject[] TextObjects { get; set; } // One for every line of text
    private AnimatedObject NextTextSymbol { get; set; }
    private AnimatedObject SkipSymbol { get; set; } // N-Gage exclusive

    private ushort CurrentFrameIndex { get; set; }
    private ushort CurrentTextLine { get; set; }
    private Text CurrentText { get; set; }

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

        CurrentText = Localization.TextBanks[ActResource.TextBankId].Texts[textId];

        float centerX = Engine.ScreenCamera.GameResolution.X / 2f;
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

            if (CurrentTextLine < CurrentText.LinesCount)
            {
                string line = CurrentText.Lines.Value![CurrentTextLine];

                int textWidth = FontManager.GetStringWidth(textObj.FontSize, line);

                textObj.Text = line;
                textObj.ScreenPos = new Vector2(centerX - textWidth / 2f, baseY + lineHeight * i);

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
            SoundManager.Play(frame.MusicSongEvent);

        if (!CachedTextureRenderers.TryGetValue(frame.Bitmap, out IScreenRenderer renderer))
        {
            renderer = new TextureScreenRenderer(new BitmapTexture2D(
                width: (int)Engine.ScreenCamera.OriginalGameResolution.X,
                height: (int)Engine.ScreenCamera.OriginalGameResolution.Y,
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
        TransitionsFX = new TransitionsFX();
        TransitionsFX.FadeInInit(1 / 16f);
        
        IsFadingOut = false;

        ActResource = resource;

        if (resource.StartMusicSoundEvent != Rayman3SoundEvent.None)
            SoundManager.Play(resource.StartMusicSoundEvent);

        AnimationPlayer = new AnimationPlayer(false);
        SpriteTextObject.Color = new RGB555Color(0x8aa).ToColor();

        if (Engine.Settings.Platform == Platform.GBA)
        {
            TextObjects = new[]
            {
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 129),
                    FontSize = FontSize.Font16,
                },
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 143),
                    FontSize = FontSize.Font16,
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
                },
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 149),
                    FontSize = FontSize.Font16,
                },
                new SpriteTextObject()
                {
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(4, 163),
                    FontSize = FontSize.Font16,
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
                CurrentAnimation = 10 + Localization.LanguageUiIndex,
                ScreenPos = new Vector2(-1, 190)
            };
        }

        BitmapScreen = new GfxScreen(2)
        {
            IsEnabled = true,
            Priority = 1,
        };
        Gfx.AddScreen(BitmapScreen);

        NextFrame(true);
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
        if (TransitionsFX.IsChangingBrightness)
        {
            TransitionsFX.StepBrightness();
        }
        else if (TransitionsFX.IsFading)
        {
            TransitionsFX.StepFade();
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
                SoundManager.StopAll();
                SoundManager.Play(Rayman3SoundEvent.Play__Valid01_Mix01);
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
                    CurrentTextLine >= CurrentText.LinesCount)
                {
                    TransitionsFX.FadeOutInit(1 / 16f);
                    IsFadingOut = true;
                    SoundManager.Play(Rayman3SoundEvent.Play__Valid01_Mix01);
                }
                else
                {
                    InitTextTransition();
                }
            }
        }

        if (!IsAutomatic && (CurrentFrameIndex != ActResource.LastFrameIndex || CurrentTextLine < CurrentText.LinesCount))
        {
            if ((GameTime.ElapsedFrames & 0x10) != 0)
                AnimationPlayer.AddObject(NextTextSymbol);
        }

        foreach (SpriteTextObject textObj in TextObjects)
            AnimationPlayer.AddObject(textObj);

        if (!IsAutomatic && Engine.Settings.Platform == Platform.NGage)
            AnimationPlayer.AddObject(SkipSymbol);

        AnimationPlayer.Execute();
    }

    #endregion
}