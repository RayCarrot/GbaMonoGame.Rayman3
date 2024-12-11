﻿using System;
using BinarySerializer;
using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.TgxEngine;
using Microsoft.Xna.Framework;
using Action = System.Action;

namespace GbaMonoGame.Rayman3;

// TODO: Add support for N-Gage menus as well as US version language selection
public partial class MenuAll : Frame, IHasPlayfield
{
    #region Constructor

    public MenuAll(Page initialPage)
    {
        // TODO: Implement all
        WheelRotation = 0;
        SelectedOption = 0;
        PrevSelectedOption = 0;
        StartEraseCursorTargetIndex = 0;
        StartEraseCursorCurrentIndex = 0;
        CurrentStepAction = null;
        NextStepAction = null;
        TransitionValue = 0;
        MultiplayerMultiPakPlayersOffsetY = 70;
        MultiplayerSinglePakPlayersOffsetY = 70;
        //field_0x1f = 0;
        GameLogoMovementXOffset = 3;
        GameLogoMovementWidth = 6;
        PrevGameTime = 0;
        GameLogoMovementXCountdown = 0;
        GameLogoYOffset = 0;
        StemMode = 0;
        IsMultiplayerMultiPakConnected = null;
        MultiplayerMultiPakConnectionTimer = 0;
        MultiplayerMultiPakLostConnectionTimer = 0;
        MultiplayerGameType = MultiplayerGameType.RayTag;
        MultiplayerMapId = 0;
        field_0x80 = 0;
        IsLoadingMultiplayerMap = false;
        ShouldMultiplayerTextBlink = false;
        FinishedLyChallenge1 = false;
        FinishedLyChallenge2 = false;
        HasAllCages = false;
        ReturningFromMultiplayerGame = false;
        Slots = new Slot[3];
        HasLoadedGameInfo = false;
        IsLoadingSlot = false;
        InitialPage = initialPage;
        PreviousMultiplayerText = 0;
    }

    #endregion

    #region Private Properties

    private AnimationPlayer AnimationPlayer { get; set; }
    private TgxPlayfield2D Playfield { get; set; }
    private TransitionsFX TransitionsFX { get; set; }

    private MenuData Data { get; set; }
    private Action CurrentStepAction { get; set; }
    private Action NextStepAction { get; set; }

    private int PrevSelectedOption { get; set; }
    private int SelectedOption { get; set; }
    private int StemMode { get; set; }

    private int TransitionValue { get; set; }
    private uint PrevGameTime { get; set; }
    private int WheelRotation { get; set; }
    private int SteamTimer { get; set; }
    
    private uint InititialGameTime { get; set; } // TODO: Multiplayer

    private Page InitialPage { get; set; }

    private bool IsLoadingMultiplayerMap { get; set; }

    private bool HasLoadedGameInfo { get; set; }
    private Slot[] Slots { get; }

    private bool FinishedLyChallenge1 { get; set; }
    private bool FinishedLyChallenge2 { get; set; }
    private bool HasAllCages { get; set; }

    #endregion

    #region Interface Properties

    TgxPlayfield IHasPlayfield.Playfield => Playfield;

    #endregion

    #region Private Methods

    private static RGB555Color[] GetBackgroundPalette(int index)
    {
        RGB555Color[] colors = index switch
        {
            0 => new RGB555Color[]
            {
                new(0x25ee), new(0x8ba), new(0x1dae), new(0x1dae), new(0x2632), new(0x2211), new(0x21cf),
                new(0x196b), new(0x154a), new(0x3695), new(0x2e54), new(0x198c), new(0x10e7), new(0x1509),
                new(0x21f0), new(0x196c), new(0x1d8d), new(0x3f21),
            },
            1 => new RGB555Color[]
            {
                new(0x2653), new(0x249d), new(0x1db2), new(0x1d91), new(0x2216), new(0x21f5), new(0x1dd3),
                new(0x196f), new(0x154d), new(0x369a), new(0x2e58), new(0x1970), new(0x10e9), new(0x150b),
                new(0x21f4), new(0x196f), new(0x1990), new(0x23a2),
            },
            2 => new RGB555Color[]
            {
                new(0x3f28), new(0x6568), new(0x3d4d), new(0x394c), new(0x4990), new(0x498f), new(0x416e),
                new(0x310b), new(0x2d0a), new(0x5a34), new(0x55d2), new(0x352b), new(0x20a7), new(0x28e8),
                new(0x456f), new(0x352b), new(0x392c), new(0x1d97),
            },
            3 => new RGB555Color[]
            {
                new(0x29b2), new(0x645f), new(0x2111), new(0x2110), new(0x2955), new(0x2534), new(0x2532),
                new(0x1cee), new(0x18cc), new(0x3df8), new(0x3197), new(0x1cef), new(0x14a9), new(0x18cb),
                new(0x2533), new(0x1cee), new(0x1cef), new(0x7ca),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };

        RGB555Color[] allColors = new RGB555Color[49];
        Array.Fill(allColors, new RGB555Color(), 0, 31);
        Array.Copy(colors, 0, allColors, 31, colors.Length);
        return allColors;
    }

    private void SetBackgroundPalette(int index)
    {
        GbaVram vram = Playfield.Vram;
        TextureScreenRenderer renderer = (TextureScreenRenderer)Playfield.TileLayers[0].Screen.Renderer;
        
        renderer.PaletteTexture = new PaletteTexture(
            Texture: Engine.TextureCache.GetOrCreateObject(
                pointer: vram.SelectedPalette.CachePointer,
                id: index + 1, // +1 since 0 is the default
                data: index,
                createObjFunc: static i => new PaletteTexture2D(GetBackgroundPalette(i))),
            PaletteIndex: 0);
    }

    private void LoadPlayfield()
    {
        PlayfieldResource menuPlayField = Storage.LoadResource<PlayfieldResource>(GameResource.MenuPlayfield);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(menuPlayField);
        Engine.GameViewPort.SetResolutionBoundsToOriginalResolution();
        Playfield.Camera.FixedResolution = true;

        SetBackgroundPalette(3);

        Gfx.ClearColor = Color.Black;

        Playfield.Camera.GetMainCluster().Position = Vector2.Zero;
        Playfield.Camera.GetCluster(1).Position = new Vector2(0, 160);
        Playfield.Camera.GetCluster(2).Position = Vector2.Zero;
    }

    private void ResetStem()
    {
        StemMode = 1;
        Data.Stem.CurrentAnimation = 12;
    }

    private void ManageCursorAndStem()
    {
        if (StemMode == 0)
        {
            if (Data.Cursor.CurrentAnimation == 16)
            {
                if (Data.Cursor.EndOfAnimation)
                {
                    Data.Cursor.CurrentAnimation = 0;

                    if (Data.Cursor.ScreenPos.Y < 68)
                    {
                        Data.Stem.CurrentAnimation = 15;
                    }
                }
            }
            else if (Data.Cursor.ScreenPos.Y >= 68)
            {
                Data.Cursor.ScreenPos -= new Vector2(0, 4);

                if (Data.Cursor.ScreenPos.Y < 68)
                {
                    Data.Cursor.ScreenPos = Data.Cursor.ScreenPos with { Y = 67 };
                    Data.Stem.CurrentAnimation = 15;
                }
            }
            else if (Data.Stem.CurrentAnimation == 15 && Data.Stem.EndOfAnimation)
            {
                Data.Stem.CurrentAnimation = 14;
                StemMode = 3;
            }
        }
        else if (StemMode == 1)
        {
            if (Data.Stem.CurrentAnimation == 12 && Data.Stem.EndOfAnimation)
            {
                Data.Stem.CurrentAnimation = 17;
            }
            else if (Data.Stem.CurrentAnimation == 17 && Data.Stem.EndOfAnimation)
            {
                Data.Stem.CurrentAnimation = 1;
                StemMode = 2;
            }
        }
        else if (StemMode == 2)
        {
            int baseY;
            int lineHeight;

            if (CurrentStepAction == Step_SinglePlayer)
            {
                baseY = 67;
                lineHeight = 18;
            }
            // TODO: Implement
            //else if (CurrentStepAction == FUN_08009450)
            //{

            //}
            else
            {
                baseY = 67;
                lineHeight = 16;
            }

            if (SelectedOption != PrevSelectedOption)
            {
                if (SelectedOption < PrevSelectedOption)
                {
                    int yPos = SelectedOption * lineHeight + baseY;

                    if (yPos < Data.Cursor.ScreenPos.Y)
                    {
                        Data.Cursor.ScreenPos -= new Vector2(0, 4);
                    }
                    else
                    {
                        Data.Cursor.ScreenPos = Data.Cursor.ScreenPos with { Y = yPos };
                        PrevSelectedOption = SelectedOption;
                    }
                }
                else
                {
                    int yPos = SelectedOption * lineHeight + baseY;

                    if (yPos > Data.Cursor.ScreenPos.Y)
                    {
                        Data.Cursor.ScreenPos += new Vector2(0, 4);
                    }
                    else
                    {
                        Data.Cursor.ScreenPos = Data.Cursor.ScreenPos with { Y = yPos };
                        PrevSelectedOption = SelectedOption;
                    }
                }
            }
        }

        AnimationPlayer.Play(Data.Stem);

        // The cursor is usually included in the stem animation, except for animation 1
        if (Data.Stem.CurrentAnimation == 1)
            AnimationPlayer.Play(Data.Cursor);
    }

    private void TransitionOutCursorAndStem()
    {
        if (StemMode is 2 or 3)
        {
            PrevSelectedOption = SelectedOption;
            SelectedOption = 0;
        }

        StemMode = 0;

        Data.Stem.CurrentAnimation = 1;

        if (Data.Cursor.ScreenPos.Y < 68 && Data.Cursor.CurrentAnimation != 16)
            Data.Stem.CurrentAnimation = 15;
    }

    private void SelectOption(int selectedOption, bool playSound)
    {
        if (StemMode is 2 or 3)
        {
            PrevSelectedOption = SelectedOption;
            SelectedOption = selectedOption;

            if (playSound)
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
        }
    }

    #endregion

    #region Public Methods

    public void LoadGameInfo()
    {
        if (HasLoadedGameInfo)
            return;

        GameInfo.Init();
        HasLoadedGameInfo = true;

        for (int i = 0; i < 3; i++)
        {
            bool loaded = GameInfo.Load(i);

            if (GameInfo.PersistentInfo.Lives != 0 && loaded)
                Slots[i] = new Slot(GameInfo.GetTotalCollectedYellowLums(), GameInfo.GetTotalCollectedCages(), GameInfo.PersistentInfo.Lives);
            else
                Slots[i] = null;

            if (GameInfo.PersistentInfo.FinishedLyChallenge1)
                FinishedLyChallenge1 = true;

            if (GameInfo.PersistentInfo.FinishedLyChallenge2)
                FinishedLyChallenge2 = true;

            if (Slots[i]?.CagesCount == 50)
                HasAllCages = true;
        }
    }

    public override void Init()
    {
        LoadGameInfo();

        AnimationPlayer = new AnimationPlayer(false, null);

        Data = new MenuData(MultiplayerMultiPakPlayersOffsetY, MultiplayerSinglePakPlayersOffsetY);
        WheelRotation = 0;

        LoadPlayfield();
        Playfield.Step();

        switch (InitialPage)
        {
            case Page.SelectLanguage:
                CurrentStepAction = Step_SelectLanguage;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Switch1_Mix03);
                break;

            case Page.SelectGameMode:
                Playfield.TileLayers[3].Screen.IsEnabled = false;
                CurrentStepAction = Step_InitializeTransitionToSelectGameMode;
                break;

            case Page.Options:
                Playfield.TileLayers[3].Screen.IsEnabled = false;
                CurrentStepAction = Step_InitializeTransitionToOptions;
                break;

            case Page.MultiPak:
                IsLoadingMultiplayerMap = true;
                Playfield.TileLayers[3].Screen.IsEnabled = false;
                CurrentStepAction = Step_InitializeTransitionToMultiplayerMultiPakPlayerSelection;
                break;

            case Page.MultiPakLostConnection:
                IsLoadingMultiplayerMap = true;
                Playfield.TileLayers[3].Screen.IsEnabled = false;
                // CurrentStepAction = FUN_08008248; // TODO: Implement
                break;
        }

        if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__raytheme) &&
            !SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__sadslide))
        {
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__raytheme);
            SoundEngineInterface.SetNbVoices(10);
        }

        RSMultiplayer.UnInit();
        RSMultiplayer.Init();
        InititialGameTime = GameTime.ElapsedFrames;
        MultiplayerInfo.Init();
        MultiplayerManager.Init();

        GameTime.Resume();

        TransitionsFX = new TransitionsFX(false);
        TransitionsFX.FadeInInit(1 / 16f);

        SteamTimer = 0;
    }

    public override void UnInit()
    {
        SoundEngineInterface.SetNbVoices(7);
        Playfield.UnInit();

        if (!IsLoadingMultiplayerMap)
        {
            RSMultiplayer.UnInit();
            GameTime.Resume();
        }

        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__raytheme);
    }

    public override void Step()
    {
        Playfield.Step();
        TransitionsFX.StepAll();
        AnimationPlayer.Execute();

        CurrentStepAction();

        if (CurrentStepAction != Step_SelectLanguage)
            ManageCursorAndStem();

        WheelRotation += 4;

        if (WheelRotation >= 2048)
            WheelRotation = 0;

        Data.Wheel1.AffineMatrix = new AffineMatrix(WheelRotation % 256, 1, 1);
        Data.Wheel2.AffineMatrix = new AffineMatrix(255 - WheelRotation / 2f % 256, 1, 1);
        Data.Wheel3.AffineMatrix = new AffineMatrix(WheelRotation / 4f % 256, 1, 1);
        Data.Wheel4.AffineMatrix = new AffineMatrix(WheelRotation / 8f % 256, 1, 1);

        AnimationPlayer.Play(Data.Wheel1);
        AnimationPlayer.Play(Data.Wheel2);
        AnimationPlayer.Play(Data.Wheel3);
        AnimationPlayer.Play(Data.Wheel4);

        if (SteamTimer == 0)
        {
            if (!Data.Steam.EndOfAnimation)
            {
                AnimationPlayer.Play(Data.Steam);
            }
            else
            {
                SteamTimer = Random.GetNumber(180) + 60; // Value between 60 and 240
                Data.Steam.CurrentAnimation = Random.GetNumber(200) < 100 ? 0 : 1;
            }
        }
        else
        {
            SteamTimer--;
        }
    }

    #endregion

    #region Data Types

    public enum Page
    {
        SelectLanguage,
        SelectGameMode,
        Options,
        MultiPak,
        MultiPakLostConnection,
        NGage, // TODO: What is this? N-Gage loads this first
    }

    private record Slot(int LumsCount, int CagesCount, int LivesCount);

    #endregion
}