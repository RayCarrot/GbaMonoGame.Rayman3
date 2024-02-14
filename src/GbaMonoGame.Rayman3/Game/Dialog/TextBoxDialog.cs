using System;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class TextBoxDialog : Dialog
{
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

    private void Fsm_MoveIn(FsmAction action)
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
                    Fsm.ChangeAction(Fsm_WaitForNextText);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_WaitForNextText(FsmAction action)
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
                    Fsm.ChangeAction(Fsm_TransitionTextOut);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_TransitionTextOut(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                TextTransitionValue = 1;
                foreach (SpriteTextObject textObj in TextObjects)
                    textObj.AffineMatrix = AffineMatrix.Identity;
                break;

            case FsmAction.Step:
                if (!IsFinished)
                {
                    TextTransitionValue++;
                    foreach (SpriteTextObject textObj in TextObjects)
                        textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);

                    if (TextTransitionValue > 8)
                    {
                        if (CurrentTextLine >= CurrentText.Length)
                        {
                            CurrentTextLine = 0;
                            ShouldPlayedLySound = true;
                            ShouldPlayRaymanSound = true;
                            ShouldPlayedMurfySound = true;
                            IsFinished = true;
                        }

                        Fsm.ChangeAction(Fsm_TransitionTextIn);
                    }
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_TransitionTextIn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Timer = 6;
                break;

            case FsmAction.Step:
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

                    bool finished = false;

                    if (TextTransitionValue < 1)
                    {
                        TextTransitionValue = 1;
                        finished = true;
                    }

                    foreach (SpriteTextObject textObj in TextObjects)
                        textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);

                    if (finished)
                        Fsm.ChangeAction(Fsm_WaitForNextText);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveOut(FsmAction action)
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
                    Fsm.ChangeAction(null);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    #endregion

    #region Protected Methods

    protected override bool ProcessMessageImpl(Message message, object param)
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
            Fsm.ChangeAction(Fsm_MoveIn);
        else
            Fsm.ChangeAction(Fsm_MoveOut);
    }

    public void MoveToNextText()
    {
        if (Fsm.EqualsAction(Fsm_WaitForNextText))
            NextText = true;
    }

    public bool IsOnScreen() => OffsetY < 45;

    public override void Load()
    {
        // NOTE: Game has it set up so Load can be called multiple times, but we only need to call it once

        AnimatedObjectResource canvasResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxCanvasAnimations);
        Canvas = new AnimatedObject(canvasResource, false)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(0, -OffsetY),
        };

        AnimatedObjectResource raymanIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxRaymanIconAnimations);
        RaymanIcon = new AnimatedObject(raymanIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
        };

        if (Engine.Settings.Platform == Platform.GBA)
        {
            TextObjects = new[]
            {
                new SpriteTextObject()
                {
                    Text = "",
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(38, 7 - OffsetY),
                    FontSize = FontSize.Font16,
                    Color = TextColor.TextBox,
                },
                new SpriteTextObject()
                {
                    Text = "",
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(38, 21 - OffsetY),
                    FontSize = FontSize.Font16,
                    Color = TextColor.TextBox,
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
                    Text = "",
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(38, 7 - OffsetY),
                    FontSize = FontSize.Font16,
                    Color = TextColor.TextBox,
                },
                new SpriteTextObject()
                {
                    Text = "",
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(38, 21 - OffsetY),
                    FontSize = FontSize.Font16,
                    Color = TextColor.TextBox,
                },
                new SpriteTextObject()
                {
                    Text = "",
                    AffineMatrix = AffineMatrix.Identity,
                    ScreenPos = new Vector2(38, 35 - OffsetY),
                    FontSize = FontSize.Font16,
                    Color = TextColor.TextBox,
                }
            };
        }
        else
        {
            throw new UnsupportedPlatformException();
        }

        AnimatedObjectResource murfyIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxMurfyIconAnimations);
        MurfyIcon = new AnimatedObject(murfyIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
        };

        // NOTE: The game only creates the two icons below if map id is not certain levels. We can ignore that as it's probably for vram allocation.
        AnimatedObjectResource lyIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxLyIconAnimations);
        LyIcon = new AnimatedObject(lyIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
        };

        AnimatedObjectResource teensiesIconResource = Storage.LoadResource<AnimatedObjectResource>(GameResource.TextBoxTeensiesIconAnimations);
        TeensiesIcon = new AnimatedObject(teensiesIconResource, true)
        {
            IsFramed = true,
            CurrentAnimation = 0,
            ScreenPos = new Vector2(8, 8 - OffsetY),
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

        Canvas.ScreenPos = new Vector2(Canvas.ScreenPos.X, -OffsetY);

        for (int i = 0; i < TextObjects.Length; i++)
            TextObjects[i].ScreenPos = new Vector2(38, 7 + 14 * i - OffsetY);

        RaymanIcon.ScreenPos = new Vector2(RaymanIcon.ScreenPos.X, 8 - OffsetY);
        MurfyIcon.ScreenPos = new Vector2(MurfyIcon.ScreenPos.X, 8 - OffsetY);
        LyIcon.ScreenPos = new Vector2(LyIcon.ScreenPos.X, 8 - OffsetY);
        TeensiesIcon.ScreenPos = new Vector2(TeensiesIcon.ScreenPos.X, 8 - OffsetY);

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
                icon.CurrentAnimation = Random.Shared.Next(100) < 50 ? 1 : 2;
        }
        else if (icon.EndOfAnimation)
        {
            IconAnimationTimer = 60 + Random.Shared.Next(60);
            icon.CurrentAnimation = 0;
        }

        animationPlayer.PlayFront(icon);
    }

    #endregion
}