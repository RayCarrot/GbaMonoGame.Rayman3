using System;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
using Microsoft.Xna.Framework.Graphics;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;
using Action = System.Action;

namespace OnyxCs.Gba.Rayman3;

// TODO: Add support for N-Gage menus as well as US version language selection
public class MenuAll : Frame, IHasPlayfield
{
    #region Constructor

    public MenuAll(Page initialPage)
    {
        InitialPage = initialPage;
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
    private int GameLogoYOffset { get; set; }
    private int OtherGameLogoValue { get; set; }
    private int WheelRotation { get; set; }
    private int SteamTimer { get; set; }
    
    private int InitGameTime { get; set; }

    private bool HasLoadedGameInfo { get; set; }
    private Page InitialPage { get; set; }

    private Slot[] Slots { get; } = new Slot[3];
    private bool IsLoadingSlot { get; set; }

    // Unknown
    private byte StartEraseCursorTargetIndex { get; set; }
    private byte StartEraseCursorCurrentIndex { get; set; }
    private byte EraseSaveStage { get; set; }

    #endregion

    #region Interface Properties

    TgxPlayfield IHasPlayfield.Playfield => Playfield;

    #endregion

    #region Private Methods

    private Palette GetBackgroundPalette(int index)
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
        return new Palette(allColors);
    }

    private void SetBackgroundPalette(int index)
    {
        ((MultiTextureScreenRenderer)Playfield.TileLayers[0].Screen.Renderer).CurrentTextureIndex = index;
    }

    private void LoadPlayfield()
    {
        PlayfieldResource menuPlayField = Storage.LoadResource<PlayfieldResource>(GameResource.MenuPlayfield);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(menuPlayField);

        // The background layer can have multiple palettes, so we need to create a texture for each
        TgxTileLayer bgLayer = Playfield.TileLayers[0];
        TiledTextureScreenRenderer renderer = (TiledTextureScreenRenderer)bgLayer.Screen.Renderer;
        bgLayer.Screen.Renderer = new MultiTextureScreenRenderer(new Texture2D[]
        { 
            new TiledTexture2D(bgLayer.Width, bgLayer.Height, renderer.TileSet, renderer.TileMap, GetBackgroundPalette(0), bgLayer.Is8Bit),
            new TiledTexture2D(bgLayer.Width, bgLayer.Height, renderer.TileSet, renderer.TileMap, GetBackgroundPalette(1), bgLayer.Is8Bit),
            new TiledTexture2D(bgLayer.Width, bgLayer.Height, renderer.TileSet, renderer.TileMap, GetBackgroundPalette(2), bgLayer.Is8Bit),
            new TiledTexture2D(bgLayer.Width, bgLayer.Height, renderer.TileSet, renderer.TileMap, GetBackgroundPalette(3), bgLayer.Is8Bit),
        });
        SetBackgroundPalette(3);

        Playfield.Camera.GetMainCluster().Position = Vector2.Zero;
        Playfield.Camera.GetCluster(1).Position = new Vector2(0, 160);
        Playfield.Camera.GetCluster(2).Position = Vector2.Zero;
    }

    private void MoveGameLogo()
    {
        // TODO: Implement
        //if (GameLogoYOffset < 56)
        //{
        //    Data.GameLogo.ScreenPos = new Vector2(Data.GameLogo.ScreenPos.X, GameLogoYOffset * 2 - 54);
        //    GameLogoYOffset += 4;
        //}
        //else
        //{
        //    if (OtherGameLogoValue == 12)
        //    {
        //        if (Data.GameLogo.ScreenPos.Y > 16)
        //        {
        //            Data.GameLogo.ScreenPos -= new Vector2(0, 1);
        //        }
        //    }
        //    else
        //    {
                
        //    }
        //}
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
                    Data.Cursor.ScreenPos = new Vector2(Data.Cursor.ScreenPos.X, 67);
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
                        Data.Cursor.ScreenPos = new Vector2(Data.Cursor.ScreenPos.X, yPos);
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
                        Data.Cursor.ScreenPos = new Vector2(Data.Cursor.ScreenPos.X, yPos);
                        PrevSelectedOption = SelectedOption;
                    }
                }
            }
        }

        AnimationPlayer.AddSortedObject(Data.Stem);

        // The cursor is usually included in the stem animation, except for animation 1
        if (Data.Stem.CurrentAnimation == 1)
            AnimationPlayer.AddSortedObject(Data.Cursor);
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
                SoundManager.Play(Rayman3SoundEvent.Play__MenuMove);
        }
    }

    private void SetEraseCursorTargetIndex(byte targetIndex)
    {
        StartEraseCursorCurrentIndex = StartEraseCursorTargetIndex;
        StartEraseCursorTargetIndex = targetIndex;
    }

    private void MoveStartEraseCursor()
    {
        if (StartEraseCursorTargetIndex != StartEraseCursorCurrentIndex)
        {
            int targetXPos = StartEraseCursorTargetIndex * 72 + 106;

            if (StartEraseCursorTargetIndex < StartEraseCursorCurrentIndex)
            {
                if (Data.StartEraseCursor.ScreenPos.X > targetXPos)
                {
                    Data.StartEraseCursor.ScreenPos -= new Vector2(4, 0);
                }
                else
                {
                    Data.StartEraseCursor.ScreenPos = new Vector2(targetXPos, Data.StartEraseCursor.ScreenPos.Y);
                    StartEraseCursorCurrentIndex = StartEraseCursorTargetIndex;
                }
            }
            else
            {
                if (Data.StartEraseCursor.ScreenPos.X < targetXPos)
                {
                    Data.StartEraseCursor.ScreenPos += new Vector2(4, 0);
                }
                else
                {
                    Data.StartEraseCursor.ScreenPos = new Vector2(targetXPos, Data.StartEraseCursor.ScreenPos.Y);
                    StartEraseCursorCurrentIndex = StartEraseCursorTargetIndex;
                }
            }
        }
    }

    #endregion

    #region Public Methods

    public void LoadGameInfo()
    {
        if (HasLoadedGameInfo)
            return;

        HasLoadedGameInfo = true;

        for (int i = 0; i < 3; i++)
        {
            bool loaded = GameInfo.Load(i);

            if (GameInfo.PersistentInfo.Lives != 0 && loaded)
                Slots[i] = new Slot(GameInfo.GetTotalCollectedYellowLums(), GameInfo.GetTotalCollectedCages(), GameInfo.PersistentInfo.Lives);
            else
                Slots[i] = null;
        }
    }

    public override void Init()
    {
        LoadGameInfo();

        AnimationPlayer = new AnimationPlayer(false);

        Data = new MenuData();
        WheelRotation = 0;

        LoadPlayfield();

        switch (InitialPage)
        {
            case Page.SelectLanguage:
                CurrentStepAction = Step_SelectLanguage;
                SoundManager.Play(Rayman3SoundEvent.Play__Switch1_Mix03);
                break;

            case Page.SelectGameMode:
                Playfield.TileLayers[3].Screen.IsEnabled = false;
                CurrentStepAction = Step_InitializeTransitionToSelectGameMode;
                break;

            case Page.Options:
                throw new NotImplementedException();
                break;

            case Page.MultiPak:
                throw new NotImplementedException();
                break;

            case Page.SinglePak:
                throw new NotImplementedException();
                break;
        }

        if (!SoundManager.IsPlaying(Rayman3SoundEvent.Play__raytheme) &&
            !SoundManager.IsPlaying(Rayman3SoundEvent.Play__sadslide))
        {
            SoundManager.Play(Rayman3SoundEvent.Play__raytheme);
        }

        // TODO: Reset multiplayer data in FUN_080ade7c and FUN_080ade28

        InitGameTime = GameTime.ElapsedFrames;

        // TODO: MultiplayerInfo::Ctor();
        // TODO: MultiplayerManager::Ctor();

        GameTime.IsPaused = false;

        TransitionsFX = new TransitionsFX();
        TransitionsFX.FadeInInit(1 / 16f);

        SteamTimer = 0;
    }

    public override void UnInit()
    {
        SoundManager.Play(Rayman3SoundEvent.Stop__raytheme);
    }

    public override void Step()
    {
        TransitionsFX.StepAll();
        AnimationPlayer.Execute();

        CurrentStepAction();

        if (CurrentStepAction != Step_SelectLanguage)
            ManageCursorAndStem();

        WheelRotation += 4;

        if (WheelRotation > 2047)
            WheelRotation = 0;

        float sin1 = MathF.Sin(2 * MathF.PI * ((byte)WheelRotation / 256f));
        float cos1 = MathF.Cos(2 * MathF.PI * ((byte)WheelRotation / 256f));
        Data.Wheel1.AffineMatrix = new AffineMatrix(cos1, sin1, -sin1, cos1);

        float sin2 = MathF.Sin(2 * MathF.PI * ((255 - (byte)(WheelRotation >> 1)) / 256f));
        float cos2 = MathF.Cos(2 * MathF.PI * ((255 - (byte)(WheelRotation >> 1)) / 256f));
        Data.Wheel2.AffineMatrix = new AffineMatrix(cos2, sin2, -sin2, cos2);

        float sin3 = MathF.Sin(2 * MathF.PI * ((byte)(WheelRotation >> 2) / 256f));
        float cos3 = MathF.Cos(2 * MathF.PI * ((byte)(WheelRotation >> 2) / 256f));
        Data.Wheel3.AffineMatrix = new AffineMatrix(cos3, sin3, -sin3, cos3);

        float sin4 = MathF.Sin(2 * MathF.PI * ((byte)(WheelRotation >> 3) / 256f));
        float cos4 = MathF.Cos(2 * MathF.PI * ((byte)(WheelRotation >> 3) / 256f));
        Data.Wheel4.AffineMatrix = new AffineMatrix(cos4, sin4, -sin4, cos4);

        AnimationPlayer.AddSortedObject(Data.Wheel1);
        AnimationPlayer.AddSortedObject(Data.Wheel2);
        AnimationPlayer.AddSortedObject(Data.Wheel3);
        AnimationPlayer.AddSortedObject(Data.Wheel4);

        if (SteamTimer == 0)
        {
            if (!Data.Steam.EndOfAnimation)
            {
                AnimationPlayer.AddSortedObject(Data.Steam);
            }
            else
            {
                SteamTimer = Random.Shared.Next(60, 240);
                Data.Steam.CurrentAnimation = Random.Shared.Next(200) < 100 ? 0 : 1;
            }
        }
        else
        {
            SteamTimer--;
        }
    }

    #endregion

    #region Steps

    private void Step_SelectLanguage()
    {
        if (JoyPad.CheckSingle(GbaInput.Up))
        {
            if (SelectedOption == 0)
                SelectedOption = 9;
            else
                SelectedOption--;

            Data.LanguageList.CurrentAnimation = SelectedOption;

            // TODO: Game passes in 0 as obj here, but that's probably a mistake
            SoundManager.Play(Rayman3SoundEvent.Play__MenuMove);
        }
        else if (JoyPad.CheckSingle(GbaInput.Down))
        {
            if (SelectedOption == 9)
                SelectedOption = 0;
            else
                SelectedOption++;
            
            Data.LanguageList.CurrentAnimation = SelectedOption;

            // TODO: Game passes in 0 as obj here, but that's probably a mistake
            SoundManager.Play(Rayman3SoundEvent.Play__MenuMove);
        }
        else if (JoyPad.CheckSingle(GbaInput.A))
        {
            CurrentStepAction = Step_TransitionFromLanguage;

            SoundManager.Play(Rayman3SoundEvent.Play__Valid01_Mix01);
            SoundManager.Play(Rayman3SoundEvent.Play__Switch1_Mix03);

            Localization.SetLanguage(SelectedOption);

            TransitionValue = 0;
            SelectedOption = 0;
            PrevSelectedOption = 0;
            GameLogoYOffset = 56;
            OtherGameLogoValue = 12;

            Data.GameModeList.CurrentAnimation = Localization.LanguageUiIndex * 3 + SelectedOption;

            // Center sprites if English
            if (Localization.Language == 0)
            {
                Data.GameModeList.ScreenPos = new Vector2(86, Data.GameModeList.ScreenPos.Y);
                Data.Cursor.ScreenPos = new Vector2(46, Data.Cursor.ScreenPos.Y);
                Data.Stem.ScreenPos = new Vector2(60, Data.Stem.ScreenPos.Y);
            }

            ResetStem();
        }

        AnimationPlayer.AddSortedObject(Data.LanguageList);
    }

    private void Step_TransitionFromLanguage()
    {
        TgxCluster mainCluster = Playfield.Camera.GetMainCluster();
        mainCluster.Position += new Vector2(0, 3);

        Data.LanguageList.ScreenPos = new Vector2(Data.LanguageList.ScreenPos.X, TransitionValue + 28);
        AnimationPlayer.AddSortedObject(Data.LanguageList);

        MoveGameLogo();

        AnimationPlayer.AddSortedObject(Data.GameLogo);
        AnimationPlayer.AddSortedObject(Data.GameModeList);

        if (TransitionValue < -207)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_SelectGameMode;
        }
        else
        {
            TransitionValue -= 3;
        }
    }

    private void Step_SelectGameMode()
    {
        if (JoyPad.CheckSingle(GbaInput.Up))
        {
            SelectOption(SelectedOption == 0 ? 2 : SelectedOption - 1, true);

            Data.GameModeList.CurrentAnimation = Localization.LanguageUiIndex * 3 + SelectedOption;
        }
        else if (JoyPad.CheckSingle(GbaInput.Down))
        {
            SelectOption(SelectedOption == 2 ? 0 : SelectedOption + 1, true);

            Data.GameModeList.CurrentAnimation = Localization.LanguageUiIndex * 3 + SelectedOption;
        }
        else if (JoyPad.CheckSingle(GbaInput.A))
        {
            Data.Cursor.CurrentAnimation = 16;

            switch (SelectedOption)
            {
                // Single player
                case 0:
                    NextStepAction = Step_InitializeTransitionToSinglePlayer;
                    break;

                case 1:
                    // TODO: Multiplayer
                    break;

                case 2:
                    // TODO: Options
                    break;
            }

            CurrentStepAction = Step_TransitionOutOfSelectGameMode;
            SoundManager.Play(Rayman3SoundEvent.Play__Store01_Mix01);
            SelectOption(0, false);
            TransitionValue = 0;
            SoundManager.Play(Rayman3SoundEvent.Play__Valid01_Mix01);
            TransitionOutCursorAndStem();
        }

        AnimationPlayer.AddSortedObject(Data.GameModeList);
        
        MoveGameLogo();
        AnimationPlayer.AddSortedObject(Data.GameLogo);
    }

    private void Step_TransitionOutOfSelectGameMode()
    {
        TransitionValue += 4;

        if (TransitionValue <= 160)
        {
            TgxCluster cluster = Playfield.Camera.GetCluster(1);
            cluster.Position -= new Vector2(0, 4);
            Data.GameLogo.ScreenPos = new Vector2(Data.GameLogo.ScreenPos.X, 16 - (TransitionValue >> 1));
        }
        else if (TransitionValue >= 220)
        {
            TransitionValue = 0;
            CurrentStepAction = NextStepAction;
        }

        AnimationPlayer.AddSortedObject(Data.GameModeList);

        MoveGameLogo();
        AnimationPlayer.AddSortedObject(Data.GameLogo);
    }

    private void Step_TransitionOutOfSinglePlayer()
    {
        TransitionValue += 4;

        if (TransitionValue <= 160)
        {
            TgxCluster cluster = Playfield.Camera.GetCluster(1);
            cluster.Position -= new Vector2(0, 4);
            Data.StartEraseSelection.ScreenPos = new Vector2(Data.StartEraseSelection.ScreenPos.X, 30 - TransitionValue / 2f);
            Data.StartEraseCursor.ScreenPos = new Vector2(Data.StartEraseCursor.ScreenPos.X, 12 - TransitionValue / 2f);
        }
        else if (TransitionValue >= 220)
        {
            TransitionValue = 0;
            CurrentStepAction = NextStepAction;
        }

        for (int i = 0; i < 3; i++)
        {
            AnimationPlayer.AddSortedObject(Data.SlotIcons[i]);

            if (Slots[i] == null)
            {
                AnimationPlayer.AddSortedObject(Data.SlotEmptyTexts[i]);
            }
            else
            {
                AnimationPlayer.AddSortedObject(Data.SlotLumTexts[i]);
                AnimationPlayer.AddSortedObject(Data.SlotCageTexts[i]);
                AnimationPlayer.AddSortedObject(Data.SlotLumIcons[i]);
                AnimationPlayer.AddSortedObject(Data.SlotCageIcons[i]);
            }
        }

        AnimationPlayer.AddSortedObject(Data.StartEraseSelection);
        AnimationPlayer.AddSortedObject(Data.StartEraseCursor);
    }

    private void Step_InitializeTransitionToSinglePlayer()
    {
        foreach (SpriteTextObject slotLumText in Data.SlotLumTexts)
            slotLumText.Text = "1000";

        foreach (SpriteTextObject slotCageText in Data.SlotCageTexts)
            slotCageText.Text = "50";

        foreach (AnimatedObject slotEmptyText in Data.SlotEmptyTexts)
            slotEmptyText.CurrentAnimation = Localization.LanguageUiIndex;

        Data.StartEraseSelection.CurrentAnimation = Localization.LanguageUiIndex * 2 + 1;
        Data.StartEraseCursor.CurrentAnimation = 40;

        SpriteTextObject.Color = new RGB555Color(0x2fd).ToColor();

        for (int i = 0; i < 3; i++)
        {
            if (Slots[i] != null)
            {
                Data.SlotLumTexts[i].Text = Slots[i].LumsCount.ToString();
                Data.SlotCageTexts[i].Text = Slots[i].CagesCount.ToString();
            }
        }

        CurrentStepAction = Step_TransitionToSinglePlayer;
        SoundManager.Play(Rayman3SoundEvent.Play__Store02_Mix02);
        ResetStem();
        SetBackgroundPalette(1);
        StartEraseCursorCurrentIndex = 0;
        StartEraseCursorTargetIndex = 0;
        EraseSaveStage = 0;
        Data.StartEraseSelection.ScreenPos = new Vector2(80, 30);
        Data.StartEraseCursor.ScreenPos = new Vector2(106, 12);
    }

    private void Step_TransitionToSinglePlayer()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
        {
            TgxCluster cluster = Playfield.Camera.GetCluster(1);
            cluster.Position += new Vector2(0, 8);
        }

        Data.StartEraseSelection.ScreenPos = new Vector2(Data.StartEraseSelection.ScreenPos.X, (TransitionValue >> 1) - 50);
        Data.StartEraseCursor.ScreenPos = new Vector2(Data.StartEraseCursor.ScreenPos.X, (TransitionValue >> 1) - 68);

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_SinglePlayer;
        }

        for (int i = 0; i < 3; i++)
        {
            AnimationPlayer.AddSortedObject(Data.SlotIcons[i]);

            if (Slots[i] == null)
            {
                AnimationPlayer.AddSortedObject(Data.SlotEmptyTexts[i]);
            }
            else
            {
                AnimationPlayer.AddSortedObject(Data.SlotLumTexts[i]);
                AnimationPlayer.AddSortedObject(Data.SlotCageTexts[i]);
                AnimationPlayer.AddSortedObject(Data.SlotLumIcons[i]);
                AnimationPlayer.AddSortedObject(Data.SlotCageIcons[i]);
            }
        }

        AnimationPlayer.AddSortedObject(Data.StartEraseSelection);
        AnimationPlayer.AddSortedObject(Data.StartEraseCursor);
    }

    private void Step_InitializeTransitionToSelectGameMode()
    {
        Data.GameModeList.CurrentAnimation = Localization.LanguageUiIndex * 3 + SelectedOption;

        // Center sprites if English
        if (Localization.Language == 0)
        {
            Data.GameModeList.ScreenPos = new Vector2(86, Data.GameModeList.ScreenPos.Y);
            Data.Cursor.ScreenPos = new Vector2(46, Data.Cursor.ScreenPos.Y);
            Data.Stem.ScreenPos = new Vector2(60, Data.Stem.ScreenPos.Y);
        }

        // The game does a bit of a hack to skip the transition if we start at the game mode selection
        if (InitialPage == Page.SelectGameMode)
        {
            CurrentStepAction = Step_SelectGameMode;
            InitialPage = Page.SelectLanguage;
        }
        else
        {
            CurrentStepAction = Step_TransitionToSelectGameMode;
            SoundManager.Play(Rayman3SoundEvent.Play__Store02_Mix02);
        }

        Data.GameLogo.ScreenPos = new Vector2(174, Data.GameLogo.ScreenPos.Y);
        OtherGameLogoValue = 0x14;
        GameLogoYOffset = 0;

        ResetStem();
        SetBackgroundPalette(3);

        SelectedOption = 0;
    }

    private void Step_TransitionToSelectGameMode()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
        {
            TgxCluster cluster = Playfield.Camera.GetCluster(1);
            cluster.Position += new Vector2(0, 8);
        }

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_SelectGameMode;
        }

        MoveGameLogo();

        AnimationPlayer.AddSortedObject(Data.GameLogo);
        AnimationPlayer.AddSortedObject(Data.GameModeList);
    }

    private void Step_SinglePlayer()
    {
        switch (EraseSaveStage)
        {
            case 0:
                if (IsLoadingSlot)
                {
                    if (!TransitionsFX.IsChangingBrightness)
                    {
                        SoundManager.StopAll();

                        if (Slots[SelectedOption] == null)
                        {
                            // Create a new game
                            FrameManager.SetNextFrame(new Act1());
                            GameInfo.ResetPersistentInfo();
                        }
                        else
                        {
                            // Load an existing game
                            GameInfo.Load(SelectedOption);
                            GameInfo.LoadLastWorld();
                        }

                        GameInfo.CurrentSlot = SelectedOption;
                        IsLoadingSlot = false;
                    }
                }
                // Move start/erase to start
                else if ((JoyPad.CheckSingle(GbaInput.Left) || JoyPad.CheckSingle(GbaInput.L)) && Data.Cursor.CurrentAnimation != 16)
                {
                    if (StartEraseCursorTargetIndex != 0)
                    {
                        SetEraseCursorTargetIndex(0);
                        Data.StartEraseSelection.CurrentAnimation = Localization.LanguageUiIndex * 2 + 1;
                        SoundManager.Play(Rayman3SoundEvent.Play__MenuMove);
                    }
                }
                // Move start/erase to erase
                else if ((JoyPad.CheckSingle(GbaInput.Right) || JoyPad.CheckSingle(GbaInput.R)) && Data.Cursor.CurrentAnimation != 16)
                {
                    if (StartEraseCursorTargetIndex != 1)
                    {
                        SetEraseCursorTargetIndex(1);
                        Data.StartEraseSelection.CurrentAnimation = Localization.LanguageUiIndex * 2;
                        SoundManager.Play(Rayman3SoundEvent.Play__MenuMove);
                    }
                }
                // Move up
                else if (JoyPad.CheckSingle(GbaInput.Up) && Data.Cursor.CurrentAnimation != 16)
                {
                    if (SelectedOption == 0)
                        SelectOption(2, true);
                    else
                        SelectOption(SelectedOption - 1, true);
                }
                // Move down
                else if (JoyPad.CheckSingle(GbaInput.Down) && Data.Cursor.CurrentAnimation != 16)
                {
                    if (SelectedOption == 2)
                        SelectOption(0, true);
                    else
                        SelectOption(SelectedOption + 1, true);
                }
                // Select slot
                else if (JoyPad.CheckSingle(GbaInput.A) && Data.Cursor.CurrentAnimation != 16)
                {
                    Data.Cursor.CurrentAnimation = 16;

                    if (StartEraseCursorTargetIndex != 1)
                    {
                        SoundManager.Play(Rayman3SoundEvent.Play__Valid01_Mix01);
                    }
                    else if (Slots[SelectedOption] != null)
                    {
                        EraseSaveStage = 1;
                        TransitionValue = 0;
                        SoundManager.Play(Rayman3SoundEvent.Play__Valid01_Mix01);
                    }
                    else
                    {
                        SoundManager.Play(Rayman3SoundEvent.Play__Back01_Mix01);
                    }
                }
                break;

            case 1:
                TransitionValue += 4;
                Data.StartEraseSelection.ScreenPos = new Vector2(Data.StartEraseSelection.ScreenPos.X, 30 - TransitionValue);
                Data.StartEraseCursor.ScreenPos = new Vector2(Data.StartEraseCursor.ScreenPos.X, 12 - TransitionValue);

                if (TransitionValue >= 64)
                {
                    TransitionValue = 0;
                    EraseSaveStage = 2;
                    Data.StartEraseSelection.CurrentAnimation = Localization.LanguageUiIndex * 2 + 21;
                    Data.StartEraseSelection.ScreenPos = new Vector2(144, -80);
                    Data.StartEraseCursor.ScreenPos = new Vector2(Data.StartEraseCursor.ScreenPos.X, -38);
                }
                break;

            case 2:
                TransitionValue += 4;
                Data.StartEraseSelection.ScreenPos = new Vector2(Data.StartEraseSelection.ScreenPos.X, TransitionValue - 80);
                Data.StartEraseCursor.ScreenPos = new Vector2(Data.StartEraseCursor.ScreenPos.X, TransitionValue - 38);

                if (TransitionValue >= 80)
                {
                    TransitionValue = 0;
                    EraseSaveStage = 3;
                }
                break;

            case 3:
                // TODO: Implement
                break;

            case 4:
                // TODO: Implement
                break;

            case 5:
                // TODO: Implement
                break;
        }

        if (JoyPad.CheckSingle(GbaInput.B) && !TransitionsFX.IsChangingBrightness && !IsLoadingSlot)
        {
            if (EraseSaveStage == 0)
            {
                NextStepAction = Step_InitializeTransitionToSelectGameMode;
                CurrentStepAction = Step_TransitionOutOfSinglePlayer;
                SoundManager.Play(Rayman3SoundEvent.Play__Store01_Mix01);
                TransitionValue = 0;
                SelectOption(0, false);
                TransitionOutCursorAndStem();
            }
            else if (EraseSaveStage == 1)
            {
                EraseSaveStage = 5;
            }
            else if (EraseSaveStage == 2)
            {
                EraseSaveStage = 4;
                TransitionValue = 80 - TransitionValue;
            }
            else if (EraseSaveStage == 3)
            {
                EraseSaveStage = 4;
                TransitionValue = 0;
            }
        }

        MoveStartEraseCursor();

        for (int i = 0; i < 3; i++)
        {
            AnimationPlayer.AddSortedObject(Data.SlotIcons[i]);

            if (Slots[i] == null)
            {
                AnimationPlayer.AddSortedObject(Data.SlotEmptyTexts[i]);
            }
            else
            {
                AnimationPlayer.AddSortedObject(Data.SlotLumTexts[i]);
                AnimationPlayer.AddSortedObject(Data.SlotCageTexts[i]);
                AnimationPlayer.AddSortedObject(Data.SlotLumIcons[i]);
                AnimationPlayer.AddSortedObject(Data.SlotCageIcons[i]);
            }
        }

        AnimationPlayer.AddSortedObject(Data.StartEraseSelection);
        AnimationPlayer.AddSortedObject(Data.StartEraseCursor);

        if (!IsLoadingSlot && Data.Cursor.CurrentAnimation == 16 && Data.Cursor.EndOfAnimation)
        {
            Data.Cursor.CurrentAnimation = 0;

            if (StartEraseCursorTargetIndex == 0)
            {
                SoundManager.FUN_080abe44(Rayman3SoundEvent.None, 1);
                IsLoadingSlot = true;
                TransitionsFX.FadeOutInit(2 / 16f);
            }
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
        SinglePak,
        NGage, // TODO: What is this? N-Gage loads this first
    }

    private record Slot(int LumsCount, int CagesCount, int LivesCount);

    #endregion
}