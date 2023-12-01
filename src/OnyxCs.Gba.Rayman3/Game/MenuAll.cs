using System;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;
using BinarySerializer.Onyx.Gba.Rayman3;
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

    private MenuData Data { get; set; }
    private Action CurrentStepAction { get; set; }
    private Action NextStepAction { get; set; }

    private int PrevSelectedOption { get; set; }
    private int SelectedOption { get; set; }
    private int StemMode { get; set; }

    private int ScreenOutTransitionYOffset { get; set; }
    private int GameLogoYOffset { get; set; }
    private int OtherGameLogoValue { get; set; }
    private int WheelRotation { get; set; }
    private int SteamTimer { get; set; }
    
    private int InitGameTime { get; set; }

    private bool HasLoadedGameInfo { get; set; }
    private Page InitialPage { get; set; }

    private Slot[] Slots { get; } = new Slot[3];

    #endregion

    #region Interface Properties
    
    TgxPlayfield IHasPlayfield.Playfield => Playfield;

    #endregion

    #region Private Methods

    private void LoadPlayfield()
    {
        PlayfieldResource menuPlayField = Storage.LoadResource<PlayfieldResource>(GameResource.MenuPlayfield);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(menuPlayField);

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
            // TODO: Implement
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
            // TODO: Check for step actions we haven't implemented yet

            if (SelectedOption != PrevSelectedOption)
            {
                if (SelectedOption < PrevSelectedOption)
                {
                    int yPos = SelectedOption * 16 + 67;

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
                    int yPos = SelectedOption * 16 + 67;

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
            Data.Cursor.CurrentAnimation = 15;
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
                Slots[i] = new Slot(GameInfo.GetTotalCollectedYellowLums(), GameInfo.GetTotalCollectedYellowCages(), GameInfo.PersistentInfo.Lives);
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
                throw new NotImplementedException();
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

        // TODO: TransitionsFX::Ctor();
        // TODO: TransitionsFX::FadeInInit(1);

        SteamTimer = 0;
    }

    public override void UnInit()
    {
        SoundManager.Play(Rayman3SoundEvent.Stop__raytheme);
    }

    public override void Step()
    {
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

            Localization.Language = SelectedOption;

            ScreenOutTransitionYOffset = 0;
            SelectedOption = 0;
            PrevSelectedOption = 0;
            GameLogoYOffset = 56;
            OtherGameLogoValue = 12;

            Data.GameModeList.CurrentAnimation = Localization.Language * 3 + SelectedOption;

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

        Data.LanguageList.ScreenPos = new Vector2(Data.LanguageList.ScreenPos.X, ScreenOutTransitionYOffset + 28);
        AnimationPlayer.AddSortedObject(Data.LanguageList);

        MoveGameLogo();

        AnimationPlayer.AddSortedObject(Data.GameLogo);
        AnimationPlayer.AddSortedObject(Data.GameModeList);

        if (ScreenOutTransitionYOffset < -207)
        {
            ScreenOutTransitionYOffset = 0;
            CurrentStepAction = Step_SelectGameMode;
        }
        else
        {
            ScreenOutTransitionYOffset -= 3;
        }
    }

    private void Step_SelectGameMode()
    {
        if (JoyPad.CheckSingle(GbaInput.Up))
        {
            SelectOption(SelectedOption == 0 ? 2 : SelectedOption - 1, true);

            Data.GameModeList.CurrentAnimation = Localization.Language * 3 + SelectedOption;
        }
        else if (JoyPad.CheckSingle(GbaInput.Down))
        {
            SelectOption(SelectedOption == 2 ? 0 : SelectedOption + 1, true);

            Data.GameModeList.CurrentAnimation = Localization.Language * 3 + SelectedOption;
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
            ScreenOutTransitionYOffset = 0;
            SoundManager.Play(Rayman3SoundEvent.Play__Valid01_Mix01);
            TransitionOutCursorAndStem();
        }

        AnimationPlayer.AddSortedObject(Data.GameModeList);
        
        MoveGameLogo();
        AnimationPlayer.AddSortedObject(Data.GameLogo);
    }

    private void Step_TransitionOutOfSelectGameMode()
    {
        ScreenOutTransitionYOffset += 4;

        if (ScreenOutTransitionYOffset <= 160)
        {
            TgxCluster cluster = Playfield.Camera.GetCluster(1);
            cluster.Position -= new Vector2(0, 4);
            Data.GameLogo.ScreenPos = new Vector2(Data.GameLogo.ScreenPos.X, 16 - (ScreenOutTransitionYOffset >> 1));
        }
        else if (ScreenOutTransitionYOffset >= 220)
        {
            ScreenOutTransitionYOffset = 0;
            CurrentStepAction = NextStepAction;
        }

        AnimationPlayer.AddSortedObject(Data.GameModeList);

        MoveGameLogo();
        AnimationPlayer.AddSortedObject(Data.GameLogo);
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
        // TODO: Set palette to mode 1 for the background
        Data.StartEraseSelection.ScreenPos = new Vector2(80, 30);
        Data.StartEraseCursor.ScreenPos = new Vector2(106, 12);
    }

    private void Step_TransitionToSinglePlayer()
    {
        ScreenOutTransitionYOffset += 4;

        if (ScreenOutTransitionYOffset <= 80)
        {
            TgxCluster cluster = Playfield.Camera.GetCluster(1);
            cluster.Position += new Vector2(0, 8);
        }

        Data.StartEraseSelection.ScreenPos = new Vector2(Data.StartEraseSelection.ScreenPos.X, (ScreenOutTransitionYOffset >> 1) - 50);
        Data.StartEraseCursor.ScreenPos = new Vector2(Data.StartEraseCursor.ScreenPos.X, (ScreenOutTransitionYOffset >> 1) - 68);

        if (ScreenOutTransitionYOffset >= 160)
        {
            ScreenOutTransitionYOffset = 0;
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

    private void Step_SinglePlayer()
    {
        // TODO: Implement

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

        // TODO: Implement
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