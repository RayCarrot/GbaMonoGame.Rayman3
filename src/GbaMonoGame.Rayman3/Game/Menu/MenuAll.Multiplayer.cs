﻿using System;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public partial class MenuAll
{
    #region Constant Fields

    private const ushort MultiPakConnectedMessage = 0xace;

    #endregion

    #region Properties

    public bool ShouldMultiplayerTextBlink { get; set; }
    public int PreviousMultiplayerText { get; set; }
    public int NextMultiplayerTextId { get; set; }

    public uint MultiplayerInititialGameTime { get; set; }
    public int MultiplayerPlayersOffsetY { get; set; }
    public bool ReturningFromMultiplayerGame { get; set; }
    public bool? IsMultiplayerConnected { get; set; }
    public byte MultiplayerConnectionTimer { get; set; }
    public byte MultiplayerLostConnectionTimer { get; set; }
    public uint LastConnectionTime { get; set; }
    public byte MultiplayerMapHighlightValue { get; set; }
    public bool field_0x80 { get; set; } // TODO: Name
    public MultiplayerGameType MultiplayerGameType { get; set; }
    public int MultiplayerMapId { get; set; }

    public int SinglePakPlayersOffsetY { get; set; }
    public byte MultiplayerSinglePakConnectionTimer { get; set; }
    public byte field_0xe1 { get; set; } // TODO: Name

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

                        NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakTypeSelection;
                        CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakPlayerSelection;
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
                        field_0x80 = true;
                        SelectOption(0, false);
                    }
                }

                RSMultiplayer.ReleasePacket(id);
            }
        }

        IsMultiplayerConnected = true;
    }

    private void AnimateSelectedMultiplayerMapPalette()
    {
        // TODO: Implement
    }

    private void StartMultiplayerGame()
    {
        MultiplayerInfo.MapId = MultiplayerMapId;
        Random.SetSeed(MultiplayerInfo.InitialGameTime);

        switch (MultiplayerGameType)
        {
            case MultiplayerGameType.RayTag:
                MultiplayerInfo.SetGameType(MultiplayerGameType.RayTag);

                if (MultiplayerMapId == 0)
                    FrameManager.SetNextFrame(new FrameMultiTag(MapId.GbaMulti_RayTag1));
                else if (MultiplayerMapId == 1)
                    FrameManager.SetNextFrame(new FrameMultiTag(MapId.GbaMulti_RayTag2));
                break;

            case MultiplayerGameType.CatAndMouse:
                MultiplayerInfo.SetGameType(MultiplayerGameType.CatAndMouse);

                if (MultiplayerMapId == 0)
                    FrameManager.SetNextFrame(new FrameMultiCatAndMouse(MapId.GbaMulti_CatAndMouse1));
                else if (MultiplayerMapId == 1)
                    FrameManager.SetNextFrame(new FrameMultiCatAndMouse(MapId.GbaMulti_CatAndMouse2));
                break;

            case MultiplayerGameType.Missile:
                MultiplayerInfo.SetGameType(MultiplayerGameType.Missile);

                throw new NotImplementedException("Not implemented loading multiplayer missile maps");
                break;
        }
    }

    #endregion

    #region Mode Selection Steps

    private void Step_InitializeTransitionToMultiplayerModeSelection()
    {
        Data.MultiplayerModeSelection.CurrentAnimation = Localization.LanguageUiIndex * 2;

        // Center sprites if English
        if (Localization.Language == 0)
            Data.MultiplayerModeSelection.ScreenPos = Data.MultiplayerModeSelection.ScreenPos with { X = 86 };

        CurrentStepAction = Step_TransitionToMultiplayerModeSelection;
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
        
        IsLoadingMultiplayerMap = true;

        ResetStem();
        SetBackgroundPalette(1);
    }

    private void Step_TransitionToMultiplayerModeSelection()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_MultiplayerModeSelection;
        }

        AnimationPlayer.Play(Data.MultiplayerModeSelection);
    }

    private void Step_MultiplayerModeSelection()
    {
        if (JoyPad.IsButtonJustPressed(GbaInput.Up) || JoyPad.IsButtonJustPressed(GbaInput.Down))
        {
            SelectOption(SelectedOption == 0 ? 1 : 0, true);
            Data.MultiplayerModeSelection.CurrentAnimation = Localization.LanguageUiIndex * 2 + SelectedOption;
        }
        else if (JoyPad.IsButtonJustPressed(GbaInput.B))
        {
            NextStepAction = Step_InitializeTransitionToSelectGameMode;
            CurrentStepAction = Step_TransitionOutOfMultiplayerModeSelection;

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

            CurrentStepAction = Step_TransitionOutOfMultiplayerModeSelection;

            TransitionOutCursorAndStem();
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }

        AnimationPlayer.Play(Data.MultiplayerModeSelection);
    }

    private void Step_TransitionOutOfMultiplayerModeSelection()
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

    #region Multi Pak Player Selection Steps

    private void Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection()
    {
        AnimatedObjectResource resource = Storage.LoadResource<AnimatedObjectResource>(GameResource.MenuMultiplayerPlayersAnimations);

        Data.MultiplayerPlayerSelection = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 1,
            ObjPriority = 32,
            ScreenPos = new Vector2(145, 40 - MultiplayerPlayersOffsetY),
            CurrentAnimation = 0
        };

        Data.MultiplayerPlayerNumberIcons = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 1,
            ObjPriority = 0,
            ScreenPos = new Vector2(102, 22 - MultiplayerPlayersOffsetY),
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
                ScreenPos = new Vector2(104 + 24 * i, 49 - MultiplayerPlayersOffsetY),
                CurrentAnimation = 8
            };
        }

        Data.MultiplayerPlayerSelectionHighlight = new AnimatedObject(resource, false)
        {
            IsFramed = true,
            BgPriority = 1,
            ObjPriority = 0,
            ScreenPos = new Vector2(104, 26 - MultiplayerPlayersOffsetY),
            CurrentAnimation = 10
        };

        if (InitialPage == Page.Multiplayer)
        {
            for (int i = 0; i < 5; i++)
                Data.MultiplayerTexts[i].Text = "";

            CurrentStepAction = Step_MultiplayerMultiPakPlayerSelection;
            InitialPage = Page.SelectLanguage;
            MultiplayerConnectionTimer = 30;
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
            
            MultiplayerPlayersOffsetY -= 4;

            if (MultiplayerPlayersOffsetY < 0)
                MultiplayerPlayersOffsetY = 0;

            MultiplayerConnectionTimer = 30;
            IsMultiplayerConnected = true;
            LastConnectionTime = GameTime.ElapsedFrames;
        }
        else
        {
            if (MultiplayerPlayersOffsetY <= 70)
                MultiplayerPlayersOffsetY += 4;
            else
                MultiplayerPlayersOffsetY = 70;

            MultiplayerConnectionTimer = 0;
            IsMultiplayerConnected = null;
        }

        Data.MultiplayerPlayerSelection.ScreenPos = Data.MultiplayerPlayerSelection.ScreenPos with { Y = 40 - MultiplayerPlayersOffsetY };
        Data.MultiplayerPlayerNumberIcons.ScreenPos = Data.MultiplayerPlayerNumberIcons.ScreenPos with { Y = 22 - MultiplayerPlayersOffsetY };
        Data.MultiplayerPlayerSelectionHighlight.ScreenPos = Data.MultiplayerPlayerSelectionHighlight.ScreenPos with { Y = 26 - MultiplayerPlayersOffsetY };

        foreach (AnimatedObject obj in Data.MultiplayerPlayerSelectionIcons)
            obj.ScreenPos = obj.ScreenPos with { Y = 49 - MultiplayerPlayersOffsetY };

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
        if (IsMultiplayerConnected == null)
        {
            MultiplayerLostConnectionTimer = 0;

            if (ReturningFromMultiplayerGame)
            {
                if (MultiplayerConnectionTimer == 20)
                {
                    if (PreviousMultiplayerText != 1)
                        SetMultiplayerText(0, false);

                    PreviousMultiplayerText = 1;
                    MultiplayerConnectionTimer++;
                }
                else if (MultiplayerConnectionTimer > 20)
                {
                    if (MultiplayerPlayersOffsetY < 70)
                        MultiplayerPlayersOffsetY += 4;
                    else
                        MultiplayerPlayersOffsetY = 70;
                }
                else
                {
                    MultiplayerConnectionTimer++;
                }
            }
            else
            {
                if (MultiplayerConnectionTimer == 10)
                {
                    if (PreviousMultiplayerText != 1)
                        SetMultiplayerText(0, false);

                    PreviousMultiplayerText = 1;
                    MultiplayerConnectionTimer++;
                }
                else if (MultiplayerConnectionTimer > 10)
                {
                    if (MultiplayerPlayersOffsetY < 70)
                        MultiplayerPlayersOffsetY += 4;
                    else
                        MultiplayerPlayersOffsetY = 70;
                }
                else
                {
                    MultiplayerConnectionTimer++;
                }
            }
        }
        // Lost connection
        else if (IsMultiplayerConnected == false)
        {
            if (MultiplayerLostConnectionTimer < 10)
            {
                MultiplayerLostConnectionTimer++;
            }
            else
            {
                IsMultiplayerConnected = null;
                MultiplayerConnectionTimer = 0;
                MultiplayerLostConnectionTimer = 0;
                RSMultiplayer.Reset();
                MultiplayerInititialGameTime = GameTime.ElapsedFrames;
            }
        }
        // Connected
        else if (RSMultiplayer.MubState == MubState.Connected)
        {
            MultiplayerLostConnectionTimer = 0;

            if (RSMultiplayer.PlayersCount > 1)
            {
                if (MultiplayerConnectionTimer < 30)
                {
                    MultiplayerConnectionTimer++;
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

                    MultiplayerPlayersOffsetY -= 4;

                    if (MultiplayerPlayersOffsetY < 0)
                        MultiplayerPlayersOffsetY = 0;
                }
            }
        }
        else if (RSMultiplayer.MubState < MubState.Connected)
        {
            if (MultiplayerPlayersOffsetY < 70)
                MultiplayerPlayersOffsetY += 4;
            else
                MultiplayerPlayersOffsetY = 70;

            MultiplayerConnectionTimer = 30;
        }
        else if (RSMultiplayer.MubState > MubState.Connected)
        {
            if (MultiplayerPlayersOffsetY < 70)
                MultiplayerPlayersOffsetY += 4;
            else
                MultiplayerPlayersOffsetY = 70;
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

                    NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakTypeSelection;
                    CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakPlayerSelection;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
                    field_0x80 = true;
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
                MultiplayerInititialGameTime = GameTime.ElapsedFrames;
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

        if (IsMultiplayerConnected == true && GameTime.ElapsedFrames - LastConnectionTime > 15)
            IsMultiplayerConnected = false;

        if (RSMultiplayer.MubState == MubState.EstablishConnections)
        {
            if ((!RSMultiplayer.IsSlave && GameTime.ElapsedFrames - MultiplayerInititialGameTime > 50) ||
                // TODO: Why is id 4 valid?
                (RSMultiplayer.MachineId is >= 1 and <= 4 && GameTime.ElapsedFrames - MultiplayerInititialGameTime > 55))
            {
                IsMultiplayerConnected = null;
                MultiplayerConnectionTimer = 0;
                MultiplayerLostConnectionTimer = 0;
                RSMultiplayer.Reset();
                MultiplayerInititialGameTime = GameTime.ElapsedFrames;
            }
        }
        else if (RSMultiplayer.MubState >= MubState.Error)
        {
            IsMultiplayerConnected = null;
            MultiplayerConnectionTimer = ReturningFromMultiplayerGame ? (byte)20 : (byte)10;
            MultiplayerLostConnectionTimer = 0;
            RSMultiplayer.Reset();
            MultiplayerInititialGameTime = GameTime.ElapsedFrames;
        }

        if (JoyPad.IsButtonJustPressed(GbaInput.B))
        {
            SelectOption(0, false);
            NextStepAction = Step_InitializeTransitionToMultiplayerModeSelection;
            CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakPlayerSelection;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }

        Data.MultiplayerPlayerSelection.ScreenPos = Data.MultiplayerPlayerSelection.ScreenPos with { Y = 40 - MultiplayerPlayersOffsetY };
        Data.MultiplayerPlayerNumberIcons.ScreenPos = Data.MultiplayerPlayerNumberIcons.ScreenPos with { Y = 22 - MultiplayerPlayersOffsetY };
        Data.MultiplayerPlayerSelectionHighlight.ScreenPos = Data.MultiplayerPlayerSelectionHighlight.ScreenPos with { Y = 26 - MultiplayerPlayersOffsetY };

        foreach (AnimatedObject obj in Data.MultiplayerPlayerSelectionIcons)
            obj.ScreenPos = obj.ScreenPos with { Y = 49 - MultiplayerPlayersOffsetY };

        DrawMutliplayerText();
        AnimationPlayer.Play(Data.MultiplayerPlayerSelection);
        AnimationPlayer.Play(Data.MultiplayerPlayerNumberIcons);

        for (int i = 0; i < RSMultiplayer.PlayersCount; i++)
            AnimationPlayer.Play(Data.MultiplayerPlayerSelectionIcons[i]);

        AnimationPlayer.Play(Data.MultiplayerPlayerSelectionHighlight);
    }

    private void Step_TransitionOutOfMultiplayerMultiPakPlayerSelection()
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

        if (MultiplayerPlayersOffsetY <= 70)
            MultiplayerPlayersOffsetY += 4;
        else
            MultiplayerPlayersOffsetY = 70;

        Data.MultiplayerPlayerSelection.ScreenPos = Data.MultiplayerPlayerSelection.ScreenPos with { Y = 40 - MultiplayerPlayersOffsetY };
        Data.MultiplayerPlayerNumberIcons.ScreenPos = Data.MultiplayerPlayerNumberIcons.ScreenPos with { Y = 22 - MultiplayerPlayersOffsetY };

        foreach (AnimatedObject obj in Data.MultiplayerPlayerSelectionIcons)
            obj.ScreenPos = obj.ScreenPos with { Y = 49 - MultiplayerPlayersOffsetY };

        AnimationPlayer.Play(Data.MultiplayerPlayerSelection);
        AnimationPlayer.Play(Data.MultiplayerPlayerNumberIcons);

        for (int i = 0; i < RSMultiplayer.PlayersCount; i++)
            AnimationPlayer.Play(Data.MultiplayerPlayerSelectionIcons[i]);
    }

    #endregion

    #region Multi Pak Type Selection Steps

    private void Step_InitializeTransitionToMultiplayerMultiPakTypeSelection()
    {
        Data.MultiplayerTypeName.CurrentAnimation = (int)MultiplayerGameType + Localization.LanguageUiIndex * 3;
        Data.MultiplayerTypeFrame.CurrentAnimation = 2;
        Data.MultiplayerTypeLeftArrow.CurrentAnimation = 1;
        Data.MultiplayerTypeRightArrow.CurrentAnimation = 0;
        Data.MultiplayerTypeIcon.CurrentAnimation = (int)MultiplayerGameType;

        CurrentStepAction = Step_TransitionToMultiplayerMultiPakTypeSelection;
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
        MultiplayerPlayersOffsetY = 112;
        SetBackgroundPalette(0);
    }

    private void Step_TransitionToMultiplayerMultiPakTypeSelection()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_MultiplayerMultiPakTypeSelection;
            GameTime.Resume();
        }

        MultiplayerPlayersOffsetY -= 4;

        if (MultiplayerPlayersOffsetY < 0)
            MultiplayerPlayersOffsetY = 0;

        Data.MultiplayerTypeFrame.ScreenPos = Data.MultiplayerTypeFrame.ScreenPos with { Y = 35 - MultiplayerPlayersOffsetY };
        Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { Y = 50 - MultiplayerPlayersOffsetY };
        Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { Y = 50 - MultiplayerPlayersOffsetY };
        Data.MultiplayerTypeIcon.ScreenPos = Data.MultiplayerTypeIcon.ScreenPos with { Y = 24 - MultiplayerPlayersOffsetY };

        if (TransitionValue == 152 && field_0x80)
        {
            MultiplayerManager.DiscardPendingPackets();
            field_0x80 = false;
        }

        AnimationPlayer.Play(Data.MultiplayerTypeName);
        
        Data.MultiplayerTypeFrame.FrameChannelSprite();
        AnimationPlayer.Play(Data.MultiplayerTypeFrame);

        if (MultiplayerGameType == MultiplayerGameType.RayTag)
            Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { X = 300 };
        else
            Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { X = 100 };

        if (MultiplayerGameType == MultiplayerGameType.Missile)
            Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { X = 300 };
        else
            Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { X = 184 };

        AnimationPlayer.Play(Data.MultiplayerTypeLeftArrow);
        AnimationPlayer.Play(Data.MultiplayerTypeRightArrow);
        AnimationPlayer.Play(Data.MultiplayerTypeIcon);
    }

    private void Step_MultiplayerMultiPakTypeSelection()
    {
        MubState state = MultiplayerManager.Step();

        if (state == MubState.Connected)
        {
            if (MultiplayerManager.HasReadJoyPads())
            {
                GameTime.Resume();

                if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.Left) && MultiplayerGameType != MultiplayerGameType.RayTag)
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
                    MultiplayerGameType--;
                    Data.MultiplayerTypeName.CurrentAnimation = (int)MultiplayerGameType + Localization.LanguageUiIndex * 3;
                    Data.MultiplayerTypeIcon.CurrentAnimation = (int)MultiplayerGameType;
                }

                if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.Right) && MultiplayerGameType != MultiplayerGameType.Missile)
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
                    MultiplayerGameType++;
                    Data.MultiplayerTypeName.CurrentAnimation = (int)MultiplayerGameType + Localization.LanguageUiIndex * 3;
                    Data.MultiplayerTypeIcon.CurrentAnimation = (int)MultiplayerGameType;
                }
                else if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.A))
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Valid01_Mix01);
                    NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakMapSelection;
                    CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakTypeSelection;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
                }
                else if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.B))
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Back01_Mix01);
                    NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection;
                    CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakTypeSelection;
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
                }

                MultiplayerManager.InvalidateCurrentFrameInputs();
            }
            else
            {
                GameTime.Pause();
            }

            AnimationPlayer.Play(Data.MultiplayerTypeName);

            Data.MultiplayerTypeFrame.FrameChannelSprite();
            AnimationPlayer.Play(Data.MultiplayerTypeFrame);

            if (MultiplayerGameType == MultiplayerGameType.RayTag)
                Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { X = 300 };
            else
                Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { X = 100 };

            if (MultiplayerGameType == MultiplayerGameType.Missile)
                Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { X = 300 };
            else
                Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { X = 184 };

            AnimationPlayer.Play(Data.MultiplayerTypeLeftArrow);
            AnimationPlayer.Play(Data.MultiplayerTypeRightArrow);
            AnimationPlayer.Play(Data.MultiplayerTypeIcon);
        }
        else
        {
            NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection;
            CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakTypeSelection;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }
    }

    private void Step_TransitionOutOfMultiplayerMultiPakTypeSelection()
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

        MultiplayerPlayersOffsetY += 4;

        if (MultiplayerPlayersOffsetY > 112)
            MultiplayerPlayersOffsetY = 112;

        Data.MultiplayerTypeFrame.ScreenPos = Data.MultiplayerTypeFrame.ScreenPos with { Y = 35 - MultiplayerPlayersOffsetY };
        Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { Y = 50 - MultiplayerPlayersOffsetY };
        Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { Y = 50 - MultiplayerPlayersOffsetY };
        Data.MultiplayerTypeIcon.ScreenPos = Data.MultiplayerTypeIcon.ScreenPos with { Y = 24 - MultiplayerPlayersOffsetY };

        AnimationPlayer.Play(Data.MultiplayerTypeName);

        Data.MultiplayerTypeFrame.FrameChannelSprite();
        AnimationPlayer.Play(Data.MultiplayerTypeFrame);

        if (MultiplayerGameType == MultiplayerGameType.RayTag)
            Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { X = 300 };
        else
            Data.MultiplayerTypeLeftArrow.ScreenPos = Data.MultiplayerTypeLeftArrow.ScreenPos with { X = 100 };

        if (MultiplayerGameType == MultiplayerGameType.Missile)
            Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { X = 300 };
        else
            Data.MultiplayerTypeRightArrow.ScreenPos = Data.MultiplayerTypeRightArrow.ScreenPos with { X = 184 };

        AnimationPlayer.Play(Data.MultiplayerTypeLeftArrow);
        AnimationPlayer.Play(Data.MultiplayerTypeRightArrow);
        AnimationPlayer.Play(Data.MultiplayerTypeIcon);
    }

    #endregion

    #region Multi Pak Map Selection Steps

    private void Step_InitializeTransitionToMultiplayerMultiPakMapSelection()
    {
        MultiplayerMapId = 0;

        Data.MultiplayerMapSelection.CurrentAnimation = (int)MultiplayerGameType;

        Data.MultiplayerMapName1.Text = Localization.GetText(11, 9 + (int)MultiplayerGameType * 2)[0];
        Data.MultiplayerMapName1.ScreenPos = new Vector2(
            x: Engine.ScreenCamera.Resolution.X / 2 - Data.MultiplayerMapName1.GetStringWidth() / 2f,
            y: 56);

        Data.MultiplayerMapName2.Text = Localization.GetText(11, 10 + (int)MultiplayerGameType * 2)[0];
        Data.MultiplayerMapName2.ScreenPos = new Vector2(
            x: Engine.ScreenCamera.Resolution.X / 2 - Data.MultiplayerMapName1.GetStringWidth() / 2f,
            y: 96);

        CurrentStepAction = Step_TransitionToMultiplayerMultiPakMapSelection;
        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
        SetBackgroundPalette(2);
        MultiplayerPlayersOffsetY = 112;
        ResetStem();
        MultiplayerMapHighlightValue = 0;

        if ((MultiplayerGameType == MultiplayerGameType.RayTag && !FinishedLyChallenge1) ||
            (MultiplayerGameType == MultiplayerGameType.CatAndMouse && !FinishedLyChallenge2) ||
            (MultiplayerGameType == MultiplayerGameType.Missile && !HasAllCages))
        {
            // TODO: Remove second option
        }
    }

    private void Step_TransitionToMultiplayerMultiPakMapSelection()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_MultiplayerMultiPakMapSelection;
            GameTime.Resume();
        }

        MultiplayerPlayersOffsetY -= 4;

        if (MultiplayerPlayersOffsetY < 0)
            MultiplayerPlayersOffsetY = 0;

        AnimateSelectedMultiplayerMapPalette();

        Data.MultiplayerMapSelection.FrameChannelSprite();

        AnimationPlayer.Play(Data.MultiplayerMapSelection);
        AnimationPlayer.Play(Data.MultiplayerMapName1);
        AnimationPlayer.Play(Data.MultiplayerMapName2);
    }

    private void Step_MultiplayerMultiPakMapSelection()
    {
        MubState state = MultiplayerManager.Step();

        if (state == MubState.Connected)
        {
            if (MultiplayerManager.HasReadJoyPads())
            {
                GameTime.Resume();

                if (IsStartingGame)
                {
                    if (TransitionsFX.IsFadeOutFinished)
                    {
                        StartMultiplayerGame();
                        IsStartingGame = false;
                    }
                }
                else
                {
                    if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.Up) && StemMode == 2)
                    {
                        if (MultiplayerMapId == 1)
                        {
                            MultiplayerMapId = 0;
                            Data.MultiplayerMapSelection.CurrentAnimation = (int)MultiplayerGameType;
                            SelectOption(0, true);
                        }
                    }
                    else if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.Down) && StemMode == 2)
                    {
                        if ((MultiplayerGameType == MultiplayerGameType.RayTag && FinishedLyChallenge1) ||
                            (MultiplayerGameType == MultiplayerGameType.CatAndMouse && FinishedLyChallenge2) ||
                            (MultiplayerGameType == MultiplayerGameType.Missile && HasAllCages))
                        {
                            if (MultiplayerMapId == 0)
                            {
                                MultiplayerMapId = 1;
                                Data.MultiplayerMapSelection.CurrentAnimation = 3 + (int)MultiplayerGameType;
                                SelectOption(2, true);
                            }
                        }
                    }
                    else if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.A))
                    {
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Valid01_Mix01);
                        SoundEventsManager.ReplaceAllSongs(-1, 1);
                        IsStartingGame = true;
                        MultiplayerManager.FUN_080ae49c();
                        Gfx.FadeControl = new FadeControl(FadeMode.None);
                        TransitionsFX.FadeOutInit(4 / 16f);
                    }
                    else if (MultiJoyPad.IsButtonJustPressed(0, GbaInput.B))
                    {
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Back01_Mix01);
                        NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakTypeSelection;
                        CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakMapSelection;
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
                        TransitionOutCursorAndStem();
                    }
                }

                MultiplayerManager.InvalidateCurrentFrameInputs();
            }
            else
            {
                GameTime.Pause();
            }

            AnimateSelectedMultiplayerMapPalette();

            Data.MultiplayerMapSelection.FrameChannelSprite();

            AnimationPlayer.Play(Data.MultiplayerMapSelection);
            AnimationPlayer.Play(Data.MultiplayerMapName1);
            AnimationPlayer.Play(Data.MultiplayerMapName2);
        }
        else
        {
            NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection;
            CurrentStepAction = Step_TransitionOutOfMultiplayerMultiPakMapSelection;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
            TransitionOutCursorAndStem();
        }
    }

    private void Step_TransitionOutOfMultiplayerMultiPakMapSelection()
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
            Gfx.FadeControl = new FadeControl(FadeMode.None);
        }

        MultiplayerPlayersOffsetY += 4;

        if (MultiplayerPlayersOffsetY > 112)
            MultiplayerPlayersOffsetY = 112;

        AnimateSelectedMultiplayerMapPalette();

        Data.MultiplayerMapSelection.FrameChannelSprite();

        AnimationPlayer.Play(Data.MultiplayerMapSelection);
        AnimationPlayer.Play(Data.MultiplayerMapName1);
        AnimationPlayer.Play(Data.MultiplayerMapName2);
    }

    #endregion

    #region Multi Pak Lost Connection Steps

    private void Step_InitializeMultiplayerLostConnection()
    {
        if (InitialPage == Page.MultiplayerLostConnection)
        {
            InitialPage = Page.SelectLanguage;
            CurrentStepAction = Step_MultiplayerLostConnection;
            SetMultiplayerText(1, true);
        }
        else
        {
            CurrentStepAction = Step_TransitionToMultiplayerMultiPakPlayerSelection;
            SetMultiplayerText(0, false);
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
        }

        SetBackgroundPalette(2);
    }

    private void Step_MultiplayerLostConnection()
    {
        if (JoyPad.IsButtonJustPressed(GbaInput.Start))
            CurrentStepAction = Step_TransitionOutOfMultiplayerLostConnection;

        DrawMutliplayerText();
    }

    private void Step_TransitionOutOfMultiplayerLostConnection()
    {
        TransitionValue += 4;

        if (TransitionValue <= 160)
        {
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, -4);
        }
        else if (TransitionValue >= 220)
        {
            TransitionValue = 0;
            NextStepAction = Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection;
            CurrentStepAction = Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection;
        }

        DrawMutliplayerText();
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
        SinglePakPlayersOffsetY = 0x46;

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

        Data.MultiplayerSinglePakPlayers.ScreenPos = Data.MultiplayerSinglePakPlayers.ScreenPos with { Y = 40 - SinglePakPlayersOffsetY };

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
            MultiplayerInititialGameTime = GameTime.ElapsedFrames;
            NextStepAction = Step_InitializeTransitionToMultiplayerModeSelection;
            CurrentStepAction = Step_TransitionOutOfMultiplayerSinglePak;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
        }

        Data.MultiplayerSinglePakPlayers.ScreenPos = Data.MultiplayerSinglePakPlayers.ScreenPos with { Y = 40 - SinglePakPlayersOffsetY };

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

        if (SinglePakPlayersOffsetY <= 70)
            SinglePakPlayersOffsetY += 8;
        else
            SinglePakPlayersOffsetY = 70;

        DrawMutliplayerText();
        AnimationPlayer.Play(Data.MultiplayerSinglePakPlayers);
    }

    #endregion
}