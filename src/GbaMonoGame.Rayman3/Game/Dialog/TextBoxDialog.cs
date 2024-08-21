using System;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class TextBoxDialog : Dialog
{
    #region Constructor

    public TextBoxDialog(Scene2D scene) : base(scene) { }

    #endregion

    #region Private Properties

    private AnimatedObject Canvas { get; set; }
    private AnimatedObject RaymanIcon { get; set; }
    private SpriteTextObject[] TextObjects { get; set; } // One for every line of text
    private AnimatedObject MurfyIcon { get; set; }
    private AnimatedObject LyIcon { get; set; }
    private AnimatedObject TeensiesIcon { get; set; }

    private bool ShouldPlayedLySound { get; set; } = true;
    private bool ShouldPlayRaymanSound { get; set; } = true;
    private bool ShouldPlayedMurfySound { get; set; } = true;
    private int OffsetY { get; set; } = 45;
    private int IconAnimationTimer { get; set; } = 60;
    private byte Timer { get; set; }
    private Rayman3SoundEvent NextSoundEvent { get; set; } = Rayman3SoundEvent.None;
    private float TextTransitionValue { get; set; } = 1;
    private bool IsShowingCutsceneCharacter { get; set; }
    private bool NextText { get; set; }

    private int TextBankId { get; set; }
    private int TextId { get; set; }
    private TextBoxCutsceneCharacter CutsceneCharacter { get; set; }
    private int CurrentTextLine { get; set; }
    private string[] CurrentText { get; set; }

    #endregion

    #region Public Methods

    public bool IsFinished { get; private set; }

    #endregion

    #region Private Methods

    public void UpdateText(int textObjectIndex)
    {
        CurrentText = Localization.GetText(TextBankId, TextId);

        if (textObjectIndex != 0)
            IsFinished = false;

        if (CurrentTextLine + textObjectIndex >= CurrentText.Length)
        {
            TextObjects[textObjectIndex].Text = "";
            return;
        }

        if (textObjectIndex == 0)
        {
            // Rayman speaking
            if (CurrentText[CurrentTextLine][0] == '1')
            {
                IsShowingCutsceneCharacter = false;
                if (ShouldPlayRaymanSound && CutsceneCharacter == TextBoxCutsceneCharacter.Ly)
                {
                    ShouldPlayRaymanSound = false;
                    if (OffsetY > 44)
                        NextSoundEvent = Rayman3SoundEvent.Play__RyVOLy_Mix01;
                    else
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__RyVOLy_Mix01);
                }
                else
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__RyVO1_Mix01__or__RyVO2_Mix01__or__RyVO3_Mix01);
                }
            }
            // Character speaking
            else
            {
                IsShowingCutsceneCharacter = true;

                switch (CutsceneCharacter)
                {
                    case TextBoxCutsceneCharacter.Murfy:
                        if (ShouldPlayedMurfySound)
                        {
                            ShouldPlayedMurfySound = false;
                            if (OffsetY > 44)
                                NextSoundEvent = Rayman3SoundEvent.Play__MurfyVO4A_Mix01;
                            else
                                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MurfyVO4A_Mix01);
                        }
                        else
                        {
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MurfyVO1A_Mix01__or__MurfyVO1B_Mix01);
                        }

                        break;

                    case TextBoxCutsceneCharacter.Ly:
                        if (ShouldPlayedLySound)
                        {
                            ShouldPlayedLySound = false;
                            if (OffsetY > 44)
                                NextSoundEvent = Rayman3SoundEvent.Play__LyVO1_Mix01;
                            else
                                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LyVO1_Mix01);
                        }
                        break;

                    case TextBoxCutsceneCharacter.Teensies:
                        if (CurrentTextLine == 0)
                        {
                            if (OffsetY > 44)
                                NextSoundEvent = Rayman3SoundEvent.Play__TiztrVO1_Mix01;
                            else
                                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__TiztrVO1_Mix01);
                        }
                        else
                        {
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__TiztrVO2_Mix01__or__TiztrVO3_Mix01__or__TiztrVO4_Mix01);
                        }
                        break;
                }
            }

            // Skip first 2 characters because it shows who's speaking
            TextObjects[0].Text = CurrentText[CurrentTextLine][2..];
        }
        else
        {
            TextObjects[textObjectIndex].Text = CurrentText[CurrentTextLine + textObjectIndex];
        }
    }

    #endregion

    #region FSM

    private bool Fsm_MoveIn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                TextTransitionValue = 1;
                Timer = 0;
                foreach (SpriteTextObject textObj in TextObjects)
                    textObj.AffineMatrix = AffineMatrix.Identity;
                break;

            case FsmAction.Step:
                OffsetY -= 3;
                if (OffsetY <= 0)
                {
                    OffsetY = 0;
                    State.MoveTo(Fsm_WaitForNextText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_WaitForNextText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (NextText)
                {
                    NextText = false;
                    CurrentTextLine += TextObjects.Length;
                    State.MoveTo(Fsm_TransitionTextOut);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_TransitionTextOut(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                TextTransitionValue = 1;
                foreach (SpriteTextObject textObj in TextObjects)
                    textObj.AffineMatrix = AffineMatrix.Identity;
                break;

            case FsmAction.Step:
                bool transitionIn = false;

                if (!IsFinished)
                {
                    TextTransitionValue++;
                    foreach (SpriteTextObject textObj in TextObjects)
                        textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);

                    if (TextTransitionValue > 8)
                    {
                        transitionIn = true;

                        if (CurrentTextLine >= CurrentText.Length)
                        {
                            CurrentTextLine = 0;
                            ShouldPlayedLySound = true;
                            ShouldPlayRaymanSound = true;
                            ShouldPlayedMurfySound = true;
                            IsFinished = true;
                        }
                    }
                }

                if (transitionIn)
                {
                    State.MoveTo(Fsm_TransitionTextIn);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_TransitionTextIn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Timer = 6;
                break;

            case FsmAction.Step:
                bool finished = false;

                if (Timer != 0)
                {
                    if (TextObjects.Length == 3)
                    {
                        if (Timer == 6)
                            UpdateText(0);
                        if (Timer == 4)
                            UpdateText(1);
                        if (Timer == 2)
                            UpdateText(2);
                    }
                    else
                    {
                        if (Timer == 4)
                            UpdateText(0);
                        if (Timer == 2)
                            UpdateText(1);
                    }

                    Timer--;
                }
                else
                {
                    TextTransitionValue--;

                    if (TextTransitionValue < 1)
                    {
                        TextTransitionValue = 1;
                        finished = true;
                    }

                    foreach (SpriteTextObject textObj in TextObjects)
                        textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);
                }

                if (finished)
                {
                    State.MoveTo(Fsm_WaitForNextText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_MoveOut(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                CurrentTextLine = 0;
                break;

            case FsmAction.Step:
                OffsetY += 3;
                if (OffsetY >= 45)
                {
                    OffsetY = 45;
                    State.MoveTo(null);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    #endregion

    #region Protected Methods

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        return false;
    }

    #endregion

    #region Public Methods

    public void SetCutsceneCharacter(TextBoxCutsceneCharacter cutsceneCharacter)
    {
        CutsceneCharacter = cutsceneCharacter;
        TextBankId = cutsceneCharacter switch
        {
            TextBoxCutsceneCharacter.Murfy => 0,
            TextBoxCutsceneCharacter.Ly => 1,
            TextBoxCutsceneCharacter.Teensies => 9,
            _ => throw new ArgumentOutOfRangeException(nameof(cutsceneCharacter), cutsceneCharacter, null)
        };
    }

    public void SetText(int textId)
    {
        TextId = textId;

        for (int i = 0; i < TextObjects.Length; i++)
            UpdateText(i);
    }

    public void MoveInOurOut(bool moveIn)
    {
        if (moveIn)
            State.MoveTo(Fsm_MoveIn);
        else
            State.MoveTo(Fsm_MoveOut);
    }

    public void MoveToNextText()
    {
        if (State == Fsm_WaitForNextText)
            NextText = true;
    }

    public bool CanSkip() => State == Fsm_WaitForNextText;

    public void Skip()
    {
        if (State != Fsm_WaitForNextText)
            return;

        CurrentTextLine = 0;
        ShouldPlayedLySound = true;
        ShouldPlayRaymanSound = true;
        ShouldPlayedMurfySound = true;
        IsFinished = true;
        State.MoveTo(Fsm_TransitionTextIn);
    }

    public bool IsOnScreen() => OffsetY < 45;

    public override void Load()
    {
        // NOTE: Game has it set up so Load can be called multiple times. Dynamic objects don't get recreated after the first time, but instead
        //       reloaded into VRAM. We don't need to do that though due to how the graphics system works here, so just always create everything.

        AnimatedObjectResource canvasResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxCanvasAnimations);
        Canvas = new AnimatedObject(canvasResource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(0, -OffsetY),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            Camera = Scene.HudCamera,
        };

        AnimatedObjectResource raymanIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxRaymanIconAnimations);
        RaymanIcon = new AnimatedObject(raymanIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            Camera = Scene.HudCamera,
        };

        int textsCount = Engine.Settings.Platform switch
        {
            Platform.GBA => 2,
            Platform.NGage => 3,
            _ => throw new UnsupportedPlatformException()
        };
        TextObjects = new SpriteTextObject[textsCount];
        for (int i = 0; i < textsCount; i++)
        {
            TextObjects[i] = new SpriteTextObject()
            {
                Text = i < CurrentText?.Length ? (i == 0 ? CurrentText[i][2..] : CurrentText[i]) : "",
                AffineMatrix = AffineMatrix.Identity,
                ScreenPos = new Vector2(38, 7 + 14 * i - OffsetY),
                HorizontalAnchor = HorizontalAnchorMode.Scale,
                FontSize = FontSize.Font16,
                Color = TextColor.TextBox,
                Camera = Scene.HudCamera,
            };
        }

        AnimatedObjectResource murfyIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxMurfyIconAnimations);
        MurfyIcon = new AnimatedObject(murfyIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            Camera = Scene.HudCamera,
        };

        // NOTE: The game only creates the two icons below if map id is not certain levels. We can ignore that as it's probably for vram allocation.
        AnimatedObjectResource lyIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxLyIconAnimations);
        LyIcon = new AnimatedObject(lyIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            Camera = Scene.HudCamera,
        };

        AnimatedObjectResource teensiesIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxTeensiesIconAnimations);
        TeensiesIcon = new AnimatedObject(teensiesIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
            HorizontalAnchor = HorizontalAnchorMode.Scale,
            Camera = Scene.HudCamera,
        };
    }

    public override void Draw(AnimationPlayer animationPlayer)
    {
        if (OffsetY > 44)
            return;

        if (NextSoundEvent != Rayman3SoundEvent.None)
        {
            SoundEventsManager.ProcessEvent(NextSoundEvent);
            NextSoundEvent = Rayman3SoundEvent.None;
        }

        Canvas.ScreenPos = Canvas.ScreenPos with { Y = -OffsetY };

        for (int i = 0; i < TextObjects.Length; i++)
            TextObjects[i].ScreenPos = TextObjects[i].ScreenPos with { Y = 7 + 14 * i - OffsetY };

        RaymanIcon.ScreenPos = RaymanIcon.ScreenPos with { Y = 8 - OffsetY };
        MurfyIcon.ScreenPos = MurfyIcon.ScreenPos with { Y = 8 - OffsetY };
        LyIcon.ScreenPos = LyIcon.ScreenPos with { Y = 8 - OffsetY };
        TeensiesIcon.ScreenPos = TeensiesIcon.ScreenPos with { Y = 8 - OffsetY };

        // Blink next text symbol
        if (CurrentTextLine + 2 < CurrentText.Length && TextTransitionValue == 1 && (GameTime.ElapsedFrames & 0x10) != 0)
            Canvas.SetChannelVisible(0);
        else
            Canvas.SetChannelInvisible(0);

        animationPlayer.PlayFront(Canvas);

        if (Timer == 0)
        {
            foreach (SpriteTextObject textObj in TextObjects)
                animationPlayer.PlayFront(textObj);
        }

        AnimatedObject icon;
        if (IsShowingCutsceneCharacter)
            icon = CutsceneCharacter switch
            {
                TextBoxCutsceneCharacter.Murfy => MurfyIcon,
                TextBoxCutsceneCharacter.Ly => LyIcon,
                TextBoxCutsceneCharacter.Teensies => TeensiesIcon,
                _ => throw new ArgumentOutOfRangeException()
            };
        else
            icon = RaymanIcon;

        if (icon.CurrentAnimation == 0)
        {
            IconAnimationTimer--;

            if (IconAnimationTimer == 0)
                icon.CurrentAnimation = Random.GetNumber(100) < 50 ? 1 : 2;
        }
        else if (icon.EndOfAnimation)
        {
            IconAnimationTimer = 60 + Random.GetNumber(60);
            icon.CurrentAnimation = 0;
        }

        animationPlayer.PlayFront(icon);
    }

    #endregion
}