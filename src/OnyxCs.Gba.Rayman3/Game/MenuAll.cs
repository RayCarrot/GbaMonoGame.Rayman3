using System;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

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

    public Page InitialPage { get; set; }

    #endregion

    #region Interface Properties
    
    TgxPlayfield IHasPlayfield.Playfield => Playfield;

    #endregion

    #region Private Methods

    private void LoadPlayfield()
    {
        PlayfieldResource menuPlayField = Storage.LoadResource<PlayfieldResource>(0x5b);
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
        Data.Stem.SetCurrentAnimation(12);
    }

    private void ManageCursorAndStem()
    {
        if (StemMode == 0)
        {
            // TODO: Implement
        }
        else if (StemMode == 1)
        {
            if (Data.Stem.AnimationIndex == 12 && Data.Stem.EndOfAnimation)
            {
                Data.Stem.SetCurrentAnimation(17);
            }
            else if (Data.Stem.AnimationIndex == 17 && Data.Stem.EndOfAnimation)
            {
                Data.Stem.SetCurrentAnimation(1);
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

        AnimationPlayer.AddSecondaryObject(Data.Stem);

        // The cursor is usually included in the stem animation, except for animation 1
        if (Data.Stem.AnimationIndex == 1)
            AnimationPlayer.AddSecondaryObject(Data.Cursor);
    }

    private void TransitionOutCursorAndStem()
    {
        if (StemMode is 2 or 3)
        {
            PrevSelectedOption = SelectedOption;
            SelectedOption = 0;
        }

        StemMode = 0;

        Data.Stem.SetCurrentAnimation(1);

        if (Data.Cursor.ScreenPos.Y < 68 && Data.Cursor.AnimationIndex != 16)
            Data.Cursor.SetCurrentAnimation(15);
    }

    private void SelectOption(int selectedOption, bool playSound)
    {
        if (StemMode is 2 or 3)
        {
            PrevSelectedOption = SelectedOption;
            SelectedOption = selectedOption;

            if (playSound)
            {
                // TODO: Play sound
            }
        }
    }

    #endregion

    #region Public Override Methods

    public override void Init()
    {
        AnimationPlayer = new AnimationPlayer(false);

        Data = new MenuData();
        WheelRotation = 0;

        LoadPlayfield();

        switch (InitialPage)
        {
            case Page.SelectLanguage:
                CurrentStepAction = Step_SelectLanguage;
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

        SteamTimer = 0;
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

        AnimationPlayer.AddSecondaryObject(Data.Wheel1);
        AnimationPlayer.AddSecondaryObject(Data.Wheel2);
        AnimationPlayer.AddSecondaryObject(Data.Wheel3);
        AnimationPlayer.AddSecondaryObject(Data.Wheel4);

        if (SteamTimer == 0)
        {
            if (!Data.Steam.EndOfAnimation)
            {
                AnimationPlayer.AddSecondaryObject(Data.Steam);
            }
            else
            {
                SteamTimer = Random.Shared.Next(60, 240);
                Data.Steam.SetCurrentAnimation(Random.Shared.Next(200) < 100 ? 0 : 1);
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

            Data.LanguageList.SetCurrentAnimation(SelectedOption);
        }
        else if (JoyPad.CheckSingle(GbaInput.Down))
        {
            if (SelectedOption == 9)
                SelectedOption = 0;
            else
                SelectedOption++;
            
            Data.LanguageList.SetCurrentAnimation(SelectedOption);
        }
        else if (JoyPad.CheckSingle(GbaInput.A))
        {
            CurrentStepAction = Step_TransitionFromLanguage;
            Localization.Language = SelectedOption;

            ScreenOutTransitionYOffset = 0;
            SelectedOption = 0;
            PrevSelectedOption = 0;
            GameLogoYOffset = 56;
            OtherGameLogoValue = 12;

            Data.GameModeList.SetCurrentAnimation(Localization.Language * 3 + SelectedOption);

            // Center sprites if English
            if (Localization.Language == 0)
            {
                Data.GameModeList.ScreenPos = new Vector2(86, Data.GameModeList.ScreenPos.Y);
                Data.Cursor.ScreenPos = new Vector2(46, Data.Cursor.ScreenPos.Y);
                Data.Stem.ScreenPos = new Vector2(60, Data.Stem.ScreenPos.Y);
            }

            ResetStem();
        }

        AnimationPlayer.AddSecondaryObject(Data.LanguageList);
    }

    private void Step_TransitionFromLanguage()
    {
        TgxCluster mainCluster = Playfield.Camera.GetMainCluster();
        mainCluster.Position += new Vector2(0, 3);

        Data.LanguageList.ScreenPos = new Vector2(Data.LanguageList.ScreenPos.X, ScreenOutTransitionYOffset + 28);
        AnimationPlayer.AddSecondaryObject(Data.LanguageList);

        MoveGameLogo();

        AnimationPlayer.AddSecondaryObject(Data.GameLogo);
        AnimationPlayer.AddSecondaryObject(Data.GameModeList);

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

            Data.GameModeList.SetCurrentAnimation(Localization.Language * 3 + SelectedOption);
        }
        else if (JoyPad.CheckSingle(GbaInput.Down))
        {
            SelectOption(SelectedOption == 2 ? 0 : SelectedOption + 1, true);

            Data.GameModeList.SetCurrentAnimation(Localization.Language * 3 + SelectedOption);
        }
        else if (JoyPad.CheckSingle(GbaInput.A))
        {
            Data.Cursor.SetCurrentAnimation(16);

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
            SoundManager.Play(403, -1);
            SelectOption(0, false);
            ScreenOutTransitionYOffset = 0;
            SoundManager.Play(309, -1);
            TransitionOutCursorAndStem();
        }

        AnimationPlayer.AddSecondaryObject(Data.GameModeList);
        
        MoveGameLogo();
        AnimationPlayer.AddSecondaryObject(Data.GameLogo);
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

        AnimationPlayer.AddSecondaryObject(Data.GameModeList);

        MoveGameLogo();
        AnimationPlayer.AddSecondaryObject(Data.GameLogo);
    }

    private void Step_InitializeTransitionToSinglePlayer()
    {
        // TODO: Implement
        CurrentStepAction = Step_TransitionToSinglePlayer;
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
            // TODO: Render slot objects
        }

        AnimationPlayer.AddSecondaryObject(Data.StartEraseSelection);
        AnimationPlayer.AddSecondaryObject(Data.StartEraseCursor);
    }

    private void Step_SinglePlayer()
    {
        // TODO: Implement

        AnimationPlayer.AddSecondaryObject(Data.StartEraseSelection);
        AnimationPlayer.AddSecondaryObject(Data.StartEraseCursor);
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
    }

    #endregion
}