using System;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public partial class MenuAll
{
    #region Private Properties

    private bool ShouldMultiplayerTextBlink { get; set; }
    private int MultiplayerMultiPakPlayersOffsetY { get; set; }
    private int MultiplayerSinglePakPlayersOffsetY { get; set; }
    private int NextMultiplayerTextId { get; set; }
    private byte MultiplayerSinglePakConnectionTimer { get; set; }
    private byte field_0x66 { get; set; } // TODO: Name
    private byte field_0xe1 { get; set; } // TODO: Name
    private bool field_0xe3 { get; set; } // TODO: Name
    private byte field_0x70 { get; set; } // TODO: Name
    private byte field_0x71 { get; set; } // TODO: Name
    private uint field_0x7c { get; set; } // TODO: Name
    private MultiplayerGameType MultiplayerGameType { get; set; }
    private MultiplayerGameType MultiplayerMapId { get; set; }

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
            Data.MultiplayerModeSelection.ScreenPos = Data.MultiplayerModeSelection.ScreenPos with { X = 86 };

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
        if (JoyPad.IsButtonJustPressed(GbaInput.Up) || JoyPad.IsButtonJustPressed(GbaInput.Down))
        {
            SelectOption(SelectedOption == 0 ? 1 : 0, true);
            Data.MultiplayerModeSelection.CurrentAnimation = Localization.LanguageUiIndex * 2 + SelectedOption;
        }
        else if (JoyPad.IsButtonJustPressed(GbaInput.B))
        {
            NextStepAction = Step_InitializeTransitionToSelectGameMode;
            CurrentStepAction = Step_TransitionOutOfMultiplayer;

            TransitionOutCursorAndStem();
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }
        else if (JoyPad.IsButtonJustPressed(GbaInput.A))
        {
            Data.Cursor.CurrentAnimation = 16;

            NextStepAction = SelectedOption switch
            {
                0 => Step_InitializeTransitionToMultiplayerMultiPak,
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

    #region Multi Pak Steps

    private void Step_InitializeTransitionToMultiplayerMultiPak()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuMultiplayerPlayersAnimations);

        Data.MultiplayerPlayerSelection = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 1,
            ObjPriority = 32,
            ScreenPos = new Vector2(145, 40 - MultiplayerMultiPakPlayersOffsetY),
            CurrentAnimation = 0
        };

        Data.MultiplayerPlayerNumberIcons = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 1,
            ObjPriority = 0,
            ScreenPos = new Vector2(102, 22 - MultiplayerMultiPakPlayersOffsetY),
            CurrentAnimation = 4
        };

        Data.MultiplayerPlayerSelectionIcons = new AnimatedObject[4];
        for (int i = 0; i < Data.MultiplayerPlayerSelectionIcons.Length; i++)
        {
            Data.MultiplayerPlayerSelectionIcons[i] = new AnimatedObject(resource, false)
            {
                IsFramed = true,
                BgPriority = 1,
                ObjPriority = 16,
                ScreenPos = new Vector2(104 + 24 * i, 49 - MultiplayerMultiPakPlayersOffsetY),
                CurrentAnimation = 8
            };
        }

        Data.MultiplayerPlayerSelectionHighlight = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 1,
            ObjPriority = 0,
            ScreenPos = new Vector2(104, 26 - MultiplayerMultiPakPlayersOffsetY),
            CurrentAnimation = 10
        };

        if (InitialPage == Page.MultiPak1)
        {
            for (int i = 0; i < 5; i++)
                Data.MultiplayerTexts[i].Text = "";

            CurrentStepAction = Step_MultiplayerMultiPak;
            InitialPage = Page.SelectLanguage;
            field_0x71 = 30;
            field_0x7c = GameTime.ElapsedFrames;
            field_0xe3 = true;
        }
        else
        {
            SetMultiplayerText(0, false);
            CurrentStepAction = Step_TransitionToMultiplayerMultiPak;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
            field_0xe3 = false;
        }

        SetBackgroundPalette(2);
        MultiplayerManager.Init();
        GameTime.Resume();

        MultiplayerGameType = MultiplayerGameType.RayTag;
        MultiplayerMapId = 0;
        field_0x66 = 0;
    }

    private void Step_TransitionToMultiplayerMultiPak()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_MultiplayerMultiPak;
        }

        if (true) // TODO: Implement connection check
        {
            if (RSMultiplayer.PlayersCount > 1)
            {
                if (RSMultiplayer.IsMaster)
                    SetMultiplayerText(2, true); // Press START
                else
                    SetMultiplayerText(3, false); // Please Wait...

                Data.MultiplayerPlayerNumberIcons.CurrentAnimation = 3 + RSMultiplayer.PlayersCount;

                Data.MultiplayerPlayerSelectionHighlight.ScreenPos = Data.MultiplayerPlayerSelectionHighlight.ScreenPos with { X = 104 + RSMultiplayer.MachineId * 24 };

                Data.MultiplayerPlayerSelection.CurrentAnimation = RSMultiplayer.MachineId;
            }
            
            MultiplayerMultiPakPlayersOffsetY -= 4;

            if (MultiplayerMultiPakPlayersOffsetY < 0)
                MultiplayerMultiPakPlayersOffsetY = 0;

            field_0x71 = 30;
            field_0x70 = 1;
            field_0x7c = GameTime.ElapsedFrames;
        }
        else
        {
            if (MultiplayerMultiPakPlayersOffsetY <= 70)
                MultiplayerMultiPakPlayersOffsetY += 4;
            else
                MultiplayerMultiPakPlayersOffsetY = 70;

            field_0x71 = 0;
            field_0x70 = 0xff;
        }

        Data.MultiplayerPlayerSelection.ScreenPos = Data.MultiplayerPlayerSelection.ScreenPos with { Y = 40 - MultiplayerMultiPakPlayersOffsetY };
        Data.MultiplayerPlayerNumberIcons.ScreenPos = Data.MultiplayerPlayerNumberIcons.ScreenPos with { Y = 22 - MultiplayerMultiPakPlayersOffsetY };
        Data.MultiplayerPlayerSelectionHighlight.ScreenPos = Data.MultiplayerPlayerSelectionHighlight.ScreenPos with { Y = 26 - MultiplayerMultiPakPlayersOffsetY };

        foreach (AnimatedObject obj in Data.MultiplayerPlayerSelectionIcons)
            obj.ScreenPos = obj.ScreenPos with { Y = 49 - MultiplayerMultiPakPlayersOffsetY };

        DrawMutliplayerText();
        AnimationPlayer.Play(Data.MultiplayerPlayerSelection);
        AnimationPlayer.Play(Data.MultiplayerPlayerNumberIcons);

        for (int i = 0; i < RSMultiplayer.PlayersCount; i++)
            AnimationPlayer.Play(Data.MultiplayerPlayerSelectionIcons[i]);

        AnimationPlayer.Play(Data.MultiplayerPlayerSelectionHighlight);
    }

    private void Step_MultiplayerMultiPak()
    {
        // TODO: Implement
    }

    #endregion

    #region Single Pak Steps

    private void Step_InitializeTransitionToMultiplayerSinglePak()
    {
        SetMultiplayerText(3, false); // Please Wait...

        MultiplayerSinglePakConnectionTimer = 125;
        NextMultiplayerTextId = -1;
        field_0xe1 = 0;

        Data.MultiplayerSinglePakPlayers.CurrentAnimation = 11;
        MultiplayerSinglePakPlayersOffsetY = 0x46;

        CurrentStepAction = Step_TransitionToMultiplayerSinglePak;
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);

        SetBackgroundPalette(2);

        field_0x66 = 0;
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
            // NOTE: Game creates the SinglePakManager class for transferring the SinglePak ROM here
            CurrentStepAction = Step_MultiplayerSinglePak;
        }

        Data.MultiplayerSinglePakPlayers.ScreenPos = Data.MultiplayerSinglePakPlayers.ScreenPos with { Y = 40 - MultiplayerSinglePakPlayersOffsetY };

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

        if (JoyPad.IsButtonJustPressed(GbaInput.B))
        {
            RSMultiplayer.Init();
            InititialGameTime = GameTime.ElapsedFrames;
            NextStepAction = Step_InitializeTransitionToMultiplayer;
            CurrentStepAction = Step_TransitionOutOfMultiplayerSinglePak;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }

        Data.MultiplayerSinglePakPlayers.ScreenPos = Data.MultiplayerSinglePakPlayers.ScreenPos with { Y = 40 - MultiplayerSinglePakPlayersOffsetY };

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
}