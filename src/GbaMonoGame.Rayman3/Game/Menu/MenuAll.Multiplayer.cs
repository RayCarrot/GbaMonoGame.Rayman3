using System;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public partial class MenuAll
{
    #region Constant Fields

    private const ushort MultiPakConnectedMessage = 0xace;

    #endregion

    #region Private Properties

    private bool ShouldMultiplayerTextBlink { get; set; }
    private int MultiplayerMultiPakPlayersOffsetY { get; set; }
    private int MultiplayerSinglePakPlayersOffsetY { get; set; }
    private int NextMultiplayerTextId { get; set; }
    private byte MultiplayerSinglePakConnectionTimer { get; set; }
    private byte PreviousMultiplayerText { get; set; }
    private byte field_0xe1 { get; set; } // TODO: Name
    private bool ReturningFromMultiplayerGame { get; set; }
    private bool? IsMultiplayerMultiPakConnected { get; set; }
    private byte MultiplayerMultiPakConnectionTimer { get; set; }
    private byte MultiplayerMultiPakLostConnectionTimer { get; set; }
    private uint LastConnectionTime { get; set; }
    private byte field_0x80 { get; set; } // TODO: Name
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

    private void ReadIncomingPackets()
    {
        for (int id = 0; id < RSMultiplayer.PlayersCount; id++)
        {
            if (RSMultiplayer.IsPacketPending(id))
            {
                if (id != RSMultiplayer.MachineId)
                {
                    ushort packet = RSMultiplayer.ReadPacket(id)[0];

                    if (packet == MultiPakConnectedMessage)
                    {
                        LastConnectionTime = GameTime.ElapsedFrames;
                    }
                    else if ((packet & 0xf000) == 0xd000)
                    {
                        MultiplayerInfo.InitialGameTime = (uint)(packet & 0x1ff);
                        MultiplayerManager.UpdateFromRSMultiplayer();

                        FinishedLyChallenge1 = (packet & 0x200) != 0;
                        FinishedLyChallenge2 = (packet & 0x400) != 0;
                        HasAllCages = (packet & 0x800) != 0;

                        //NextStepAction = FUN_08008554; // TODO: Implement
                        CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakPlayerSelection;
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
                        field_0x80 = 1;
                        SelectOption(0, false);
                    }
                }

                RSMultiplayer.ReleasePacket(id);
            }
        }

        IsMultiplayerMultiPakConnected = true;
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
                0 => Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection,
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

    private void Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection()
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

        if (InitialPage == Page.MultiPak)
        {
            for (int i = 0; i < 5; i++)
                Data.MultiplayerTexts[i].Text = "";

            CurrentStepAction = Step_MultiplayerMultiPakPlayerSelection;
            InitialPage = Page.SelectLanguage;
            MultiplayerMultiPakConnectionTimer = 30;
            LastConnectionTime = GameTime.ElapsedFrames;
            ReturningFromMultiplayerGame = true;
        }
        else
        {
            SetMultiplayerText(0, false);
            CurrentStepAction = Step_TransitionToMultiplayerMultiPakPlayerSelection;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
            ReturningFromMultiplayerGame = false;
        }

        SetBackgroundPalette(2);
        MultiplayerManager.Init();
        GameTime.Resume();

        MultiplayerGameType = MultiplayerGameType.RayTag;
        MultiplayerMapId = 0;
        PreviousMultiplayerText = 0;
    }

    private void Step_TransitionToMultiplayerMultiPakPlayerSelection()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_MultiplayerMultiPakPlayerSelection;
        }

        if (RSMultiplayer.MubState == MubState.Connected)
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

            MultiplayerMultiPakConnectionTimer = 30;
            IsMultiplayerMultiPakConnected = true;
            LastConnectionTime = GameTime.ElapsedFrames;
        }
        else
        {
            if (MultiplayerMultiPakPlayersOffsetY <= 70)
                MultiplayerMultiPakPlayersOffsetY += 4;
            else
                MultiplayerMultiPakPlayersOffsetY = 70;

            MultiplayerMultiPakConnectionTimer = 0;
            IsMultiplayerMultiPakConnected = null;
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

    private void Step_MultiplayerMultiPakPlayerSelection()
    {
        RSMultiplayer.CheckForLostConnection();

        // Disconnected
        if (IsMultiplayerMultiPakConnected == null)
        {
            MultiplayerMultiPakLostConnectionTimer = 0;

            if (ReturningFromMultiplayerGame)
            {
                if (MultiplayerMultiPakConnectionTimer == 20)
                {
                    if (PreviousMultiplayerText != 1)
                        SetMultiplayerText(0, false);

                    PreviousMultiplayerText = 1;
                    MultiplayerMultiPakConnectionTimer++;
                }
                else if (MultiplayerMultiPakConnectionTimer > 20)
                {
                    if (MultiplayerMultiPakPlayersOffsetY < 70)
                        MultiplayerMultiPakPlayersOffsetY += 4;
                    else
                        MultiplayerMultiPakPlayersOffsetY = 70;
                }
                else
                {
                    MultiplayerMultiPakConnectionTimer++;
                }
            }
            else
            {
                if (MultiplayerMultiPakConnectionTimer == 10)
                {
                    if (PreviousMultiplayerText != 1)
                        SetMultiplayerText(0, false);

                    PreviousMultiplayerText = 1;
                    MultiplayerMultiPakConnectionTimer++;
                }
                else if (MultiplayerMultiPakConnectionTimer > 10)
                {
                    if (MultiplayerMultiPakPlayersOffsetY < 70)
                        MultiplayerMultiPakPlayersOffsetY += 4;
                    else
                        MultiplayerMultiPakPlayersOffsetY = 70;
                }
                else
                {
                    MultiplayerMultiPakConnectionTimer++;
                }
            }
        }
        // Lost connection
        else if (IsMultiplayerMultiPakConnected == false)
        {
            if (MultiplayerMultiPakLostConnectionTimer < 10)
            {
                MultiplayerMultiPakLostConnectionTimer++;
            }
            else
            {
                IsMultiplayerMultiPakConnected = null;
                MultiplayerMultiPakConnectionTimer = 0;
                MultiplayerMultiPakLostConnectionTimer = 0;
                RSMultiplayer.Reset();
                InititialGameTime = GameTime.ElapsedFrames;
            }
        }
        // Connected
        else if (RSMultiplayer.MubState == MubState.Connected)
        {
            MultiplayerMultiPakLostConnectionTimer = 0;

            if (RSMultiplayer.PlayersCount > 1)
            {
                if (MultiplayerMultiPakConnectionTimer < 30)
                {
                    MultiplayerMultiPakConnectionTimer++;
                }
                else
                {
                    if (RSMultiplayer.IsMaster)
                    {
                        if (PreviousMultiplayerText != 2)
                            SetMultiplayerText(2, true); // Press START

                        PreviousMultiplayerText = 2;
                    }
                    else
                    {
                        if (PreviousMultiplayerText != 3)
                            SetMultiplayerText(3, false); // Please Wait...

                        PreviousMultiplayerText = 3;
                    }

                    Data.MultiplayerPlayerNumberIcons.CurrentAnimation = 3 + RSMultiplayer.PlayersCount;

                    Data.MultiplayerPlayerSelectionHighlight.ScreenPos = Data.MultiplayerPlayerSelectionHighlight.ScreenPos with { X = 104 + RSMultiplayer.MachineId * 24 };

                    Data.MultiplayerPlayerSelection.CurrentAnimation = RSMultiplayer.MachineId;

                    MultiplayerMultiPakPlayersOffsetY -= 4;

                    if (MultiplayerMultiPakPlayersOffsetY < 0)
                        MultiplayerMultiPakPlayersOffsetY = 0;
                }
            }
        }
        else if (RSMultiplayer.MubState < MubState.Connected)
        {
            if (MultiplayerMultiPakPlayersOffsetY < 70)
                MultiplayerMultiPakPlayersOffsetY += 4;
            else
                MultiplayerMultiPakPlayersOffsetY = 70;

            MultiplayerMultiPakConnectionTimer = 30;
        }
        else if (RSMultiplayer.MubState > MubState.Connected)
        {
            if (MultiplayerMultiPakPlayersOffsetY < 70)
                MultiplayerMultiPakPlayersOffsetY += 4;
            else
                MultiplayerMultiPakPlayersOffsetY = 70;
        }

        // Master
        if (RSMultiplayer.IsMaster)
        {
            if (RSMultiplayer.MubState == MubState.Connected)
            {
                ReadIncomingPackets();

                if (JoyPad.IsButtonJustPressed(GbaInput.Start))
                {
                    uint trimmedGameTime = GameTime.ElapsedFrames & 0x1ff;
                    
                    ushort packet = (ushort)trimmedGameTime;
                    packet |= 0xd000;

                    if (FinishedLyChallenge1)
                        packet |= 0x200; 
                    
                    if (FinishedLyChallenge2)
                        packet |= 0x400;

                    if (HasAllCages)
                        packet |= 0x800;

                    RSMultiplayer.SendPacket([packet]);
                    MultiplayerInfo.InitialGameTime = trimmedGameTime;
                    MultiplayerManager.UpdateFromRSMultiplayer();

                    //NextStepAction = FUN_08008554; // TODO: Implement
                    CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakPlayerSelection;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
                    field_0x80 = 1;
                    SelectOption(0, false);
                }
                else
                {
                    RSMultiplayer.SendPacket([MultiPakConnectedMessage]);
                }
            }
            else if (RSMultiplayer.MubState == MubState.EstablishConnections && RSMultiplayer.PlayersCount > 1)
            {
                RSMultiplayer.Connect();
                InititialGameTime = GameTime.ElapsedFrames;
            }
        }
        // Slave
        else if (RSMultiplayer.MachineId is >= 1 and <= 4) // TODO: Why is id 4 valid?
        {
            if (RSMultiplayer.MubState == MubState.Connected)
            {
                ReadIncomingPackets();
                RSMultiplayer.SendPacket([MultiPakConnectedMessage]);
            }
        }

        if (IsMultiplayerMultiPakConnected == true && GameTime.ElapsedFrames - LastConnectionTime > 15)
            IsMultiplayerMultiPakConnected = false;

        if (RSMultiplayer.MubState == MubState.EstablishConnections)
        {
            if ((!RSMultiplayer.IsSlave && GameTime.ElapsedFrames - InititialGameTime > 50) ||
                // TODO: Why is id 4 valid?
                (RSMultiplayer.MachineId is >= 1 and <= 4 && GameTime.ElapsedFrames - InititialGameTime > 55))
            {
                IsMultiplayerMultiPakConnected = null;
                MultiplayerMultiPakConnectionTimer = 0;
                MultiplayerMultiPakLostConnectionTimer = 0;
                RSMultiplayer.Reset();
                InititialGameTime = GameTime.ElapsedFrames;
            }
        }
        else if (RSMultiplayer.MubState >= MubState.Error)
        {
            IsMultiplayerMultiPakConnected = null;
            MultiplayerMultiPakConnectionTimer = ReturningFromMultiplayerGame ? (byte)20 : (byte)10;
            MultiplayerMultiPakLostConnectionTimer = 0;
            RSMultiplayer.Reset();
            InititialGameTime = GameTime.ElapsedFrames;
        }

        if (JoyPad.IsButtonJustPressed(GbaInput.B))
        {
            SelectOption(0, false);
            NextStepAction = Step_InitializeTransitionToMultiplayer;
            CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakPlayerSelection;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
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

    private void Step_TransitionOutOfMultiplayerMultiPakPlayerSelection()
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

        PreviousMultiplayerText = 0;
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