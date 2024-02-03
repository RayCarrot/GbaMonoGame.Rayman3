using System;
using BinarySerializer;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public partial class MenuAll
{
    #region Private Properties

    private bool ShouldMultiplayerTextBlink { get; set; }
    private int MultiplayerSinglePakPlayersOffsetY { get; set; } = 70;
    private int NextMultiplayerTextId { get; set; }
    private byte MultiplayerSinglePakConnectionTimer { get; set; }

    #endregion

    #region Private Methods

    private void SetMultiplayerText(int textId, bool blink)
    {
        ShouldMultiplayerTextBlink = blink;

        string[] text = Localization.GetText(11, textId);

        int unusedLines = Data.MultiplayerTexts.Length - text.Length;
        for (int i = 0; i < Data.MultiplayerTexts.Length; i++)
        {
            if (i < unusedLines)
            {
                Data.MultiplayerTexts[i].Text = "";
            }
            else
            {
                Data.MultiplayerTexts[i].Text = text[i - unusedLines];
                Data.MultiplayerTexts[i].ScreenPos = new Vector2(140 - Data.MultiplayerTexts[i].GetStringWidth() / 2f, 32 + i * 16);
            }
        }
    }

    private void DrawMutliplayerText()
    {
        if (!ShouldMultiplayerTextBlink || (GameTime.ElapsedFrames & 0x10) != 0)
        {
            foreach (SpriteTextObject text in Data.MultiplayerTexts)
                AnimationPlayer.Play(text);
        }
    }

    #endregion

    #region Main Steps

    private void Step_InitializeTransitionToMultiplayer()
    {
        Data.MultiplayerModeSelection.CurrentAnimation = Localization.LanguageUiIndex * 2;

        // Center sprites if English
        if (Localization.Language == 0)
            Data.MultiplayerModeSelection.ScreenPos = new Vector2(86, Data.MultiplayerModeSelection.ScreenPos.Y);

        CurrentStepAction = Step_TransitionToMultiplayer;
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
        
        IsLoadingMultiplayerMap = true;

        ResetStem();
        SetBackgroundPalette(1);
    }

    private void Step_TransitionToMultiplayer()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_Multiplayer;
        }

        AnimationPlayer.Play(Data.MultiplayerModeSelection);
    }

    private void Step_Multiplayer()
    {
        if (JoyPad.CheckSingle(GbaInput.Up) || JoyPad.CheckSingle(GbaInput.Down))
        {
            SelectOption(SelectedOption == 0 ? 1 : 0, true);
            Data.MultiplayerModeSelection.CurrentAnimation = Localization.LanguageUiIndex * 2 + SelectedOption;
        }
        else if (JoyPad.CheckSingle(GbaInput.B))
        {
            NextStepAction = Step_InitializeTransitionToSelectGameMode;
            CurrentStepAction = Step_TransitionOutOfMultiplayer;

            TransitionOutCursorAndStem();
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }
        else if (JoyPad.CheckSingle(GbaInput.A))
        {
            Data.Cursor.CurrentAnimation = 16;

            NextStepAction = SelectedOption switch
            {
                // 0 => Step_InitializeTransitionToMultiplayerMultiPak, // TODO: Implement
                1 => Step_InitializeTransitionToMultiplayerSinglePak,
                _ => throw new Exception("Invalid multiplayer mode")
            };

            CurrentStepAction = Step_TransitionOutOfMultiplayer;

            TransitionOutCursorAndStem();
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }

        AnimationPlayer.Play(Data.MultiplayerModeSelection);
    }

    private void Step_TransitionOutOfMultiplayer()
    {
        TransitionValue += 4;

        if (TransitionValue <= 160)
        {
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, -4);
        }
        else if (TransitionValue >= 220)
        {
            TransitionValue = 0;
            CurrentStepAction = NextStepAction;
        }

        AnimationPlayer.Play(Data.MultiplayerModeSelection);
    }

    #endregion

    #region Single Pak Steps

    private void Step_InitializeTransitionToMultiplayerSinglePak()
    {
        SetMultiplayerText(3, false);

        MultiplayerSinglePakConnectionTimer = 125;
        NextMultiplayerTextId = -1;
        // TODO: Implement
        // field74_0xe1 = 0;

        Data.MultiplayerSinglePakPlayers.CurrentAnimation = 11;
        MultiplayerSinglePakPlayersOffsetY = 0x46;

        CurrentStepAction = Step_TransitionToMultiplayerSinglePak;
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);

        SetBackgroundPalette(2);

        // TODO: Implement
        // field45_0x66 = 0;
    }

    private void Step_TransitionToMultiplayerSinglePak()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            // NOTE: Game gets the pointer and position to the SinglePak ROM here
            RSMultiplayer.UnInit();
            // NOTE: Game created the SinglePakManager class for transferring the SinglePak ROM here
            CurrentStepAction = Step_MultiplayerSinglePak;
        }

        Data.MultiplayerSinglePakPlayers.ScreenPos = new Vector2(Data.MultiplayerSinglePakPlayers.ScreenPos.X, 40 - MultiplayerSinglePakPlayersOffsetY);

        DrawMutliplayerText();
        AnimationPlayer.Play(Data.MultiplayerSinglePakPlayers);
    }

    private void Step_MultiplayerSinglePak()
    {
        // TODO: Implement

        if (NextMultiplayerTextId != -1)
        {
            SetMultiplayerText(NextMultiplayerTextId, false);
            NextMultiplayerTextId = -1;
        }

        // TODO: Implement

        if (JoyPad.CheckSingle(GbaInput.B))
        {
            RSMultiplayer.Init();
            InititialGameTime = GameTime.ElapsedFrames;
            NextStepAction = Step_InitializeTransitionToMultiplayer;
            CurrentStepAction = Step_TransitionOutOfMultiplayerSinglePak;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }

        Data.MultiplayerSinglePakPlayers.ScreenPos = new Vector2(Data.MultiplayerSinglePakPlayers.ScreenPos.X, 40 - MultiplayerSinglePakPlayersOffsetY);

        if (NextMultiplayerTextId == -1)
            DrawMutliplayerText();
        AnimationPlayer.Play(Data.MultiplayerSinglePakPlayers);
    }

    private void Step_TransitionOutOfMultiplayerSinglePak()
    {
        TransitionValue += 4;

        if (TransitionValue <= 160)
        {
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, -4);
        }
        else if (TransitionValue >= 220)
        {
            TransitionValue = 0;
            CurrentStepAction = NextStepAction;
        }

        if (MultiplayerSinglePakPlayersOffsetY <= 70)
            MultiplayerSinglePakPlayersOffsetY += 8;
        else
            MultiplayerSinglePakPlayersOffsetY = 70;

        DrawMutliplayerText();
        AnimationPlayer.Play(Data.MultiplayerSinglePakPlayers);
    }

    #endregion

    #region Multi Pak Steps

    // TODO: Implement

    #endregion
}