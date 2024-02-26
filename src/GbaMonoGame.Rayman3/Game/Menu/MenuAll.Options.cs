using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;

namespace GbaMonoGame.Rayman3;

public partial class MenuAll
{
    #region Private Properties

    private bool IsLoadingCredits { get; set; }

    #endregion

    #region Private Methods

    private void UpdateMusicVolumeAnimations()
    {
        if (IsMusicOn())
        {
            switch (Localization.Language)
            {
                case 0:
                case 1:
                case 4:
                case 7:
                case 9:
                    Data.MusicOnOff.CurrentAnimation = 5;
                    break;

                case 2:
                    Data.MusicOnOff.CurrentAnimation = 25;
                    break;

                case 3:
                    Data.MusicOnOff.CurrentAnimation = 19;
                    break;

                case 5:
                    Data.MusicOnOff.CurrentAnimation = 27;
                    break;

                case 6:
                    Data.MusicOnOff.CurrentAnimation = 23;
                    break;

                case 8:
                    Data.MusicOnOff.CurrentAnimation = 21;
                    break;
            }
        }
        else
        {
            switch (Localization.Language)
            {
                case 0:
                case 1:
                case 4:
                case 7:
                case 9:
                    Data.MusicOnOff.CurrentAnimation = 6;
                    break;

                case 2:
                    Data.MusicOnOff.CurrentAnimation = 24;
                    break;

                case 3:
                    Data.MusicOnOff.CurrentAnimation = 18;
                    break;

                case 5:
                    Data.MusicOnOff.CurrentAnimation = 26;
                    break;

                case 6:
                    Data.MusicOnOff.CurrentAnimation = 22;
                    break;

                case 8:
                    Data.MusicOnOff.CurrentAnimation = 20;
                    break;
            }
        }
    }

    private void UpdateSfxVolumeAnimations()
    {
        if (IsSfxOn())
        {
            switch (Localization.Language)
            {
                case 0:
                case 1:
                case 4:
                case 7:
                case 9:
                    Data.SfxOnOff.CurrentAnimation = 5;
                    break;

                case 2:
                    Data.SfxOnOff.CurrentAnimation = 25;
                    break;

                case 3:
                    Data.SfxOnOff.CurrentAnimation = 19;
                    break;

                case 5:
                    Data.SfxOnOff.CurrentAnimation = 27;
                    break;

                case 6:
                    Data.SfxOnOff.CurrentAnimation = 23;
                    break;

                case 8:
                    Data.SfxOnOff.CurrentAnimation = 21;
                    break;
            }
        }
        else
        {
            switch (Localization.Language)
            {
                case 0:
                case 1:
                case 4:
                case 7:
                case 9:
                    Data.SfxOnOff.CurrentAnimation = 6;
                    break;

                case 2:
                    Data.SfxOnOff.CurrentAnimation = 24;
                    break;

                case 3:
                    Data.SfxOnOff.CurrentAnimation = 18;
                    break;

                case 5:
                    Data.SfxOnOff.CurrentAnimation = 26;
                    break;

                case 6:
                    Data.SfxOnOff.CurrentAnimation = 22;
                    break;

                case 8:
                    Data.SfxOnOff.CurrentAnimation = 20;
                    break;
            }
        }
    }

    private void ToggleMusicOnOff()
    {
        if (SoundEventsManager.GetVolumeForType(SoundType.Music) == 0)
            SoundEventsManager.SetVolumeForType(SoundType.Music, SoundEngineInterface.MaxVolume);
        else
            SoundEventsManager.SetVolumeForType(SoundType.Music, 0);
    }

    private void ToggleSfxOnOff()
    {
        if (SoundEventsManager.GetVolumeForType(SoundType.Sfx) == 0)
            SoundEventsManager.SetVolumeForType(SoundType.Sfx, SoundEngineInterface.MaxVolume);
        else
            SoundEventsManager.SetVolumeForType(SoundType.Sfx, 0);
    }

    private bool IsMusicOn()
    {
        return SoundEventsManager.GetVolumeForType(SoundType.Music) == SoundEngineInterface.MaxVolume;
    }

    private bool IsSfxOn()
    {
        return SoundEventsManager.GetVolumeForType(SoundType.Sfx) == SoundEngineInterface.MaxVolume;
    }

    #endregion

    #region Steps

    private void Step_InitializeTransitionToOptions()
    {
        Data.OptionsSelection.CurrentAnimation = Localization.LanguageUiIndex * 3 + SelectedOption;
        UpdateMusicVolumeAnimations();
        UpdateSfxVolumeAnimations();

        // Center sprites if English
        if (Localization.Language == 0)
            Data.OptionsSelection.ScreenPos = new Vector2(86, Data.OptionsSelection.ScreenPos.Y);

        if (InitialPage == Page.Options)
        {
            CurrentStepAction = Step_Options;
            InitialPage = Page.SelectLanguage;

            // Center sprites if English
            if (Localization.Language == 0)
            {
                Data.SoundsOnOffBase.ScreenPos = new Vector2(180, Data.SoundsOnOffBase.ScreenPos.Y);
                Data.MusicOnOff.ScreenPos = new Vector2(180, Data.MusicOnOff.ScreenPos.Y);
                Data.SfxOnOff.ScreenPos = new Vector2(180, Data.SfxOnOff.ScreenPos.Y);
            }
            else
            {
                Data.SoundsOnOffBase.ScreenPos = new Vector2(210, Data.SoundsOnOffBase.ScreenPos.Y);
                Data.MusicOnOff.ScreenPos = new Vector2(210, Data.MusicOnOff.ScreenPos.Y);
                Data.SfxOnOff.ScreenPos = new Vector2(210, Data.SfxOnOff.ScreenPos.Y);
            }
        }
        else
        {
            CurrentStepAction = Step_TransitionToOptions;
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store02_Mix02);
        }

        ResetStem();
        SetBackgroundPalette(1);
        IsLoadingCredits = false;
    }

    private void Step_TransitionToOptions()
    {
        TransitionValue += 4;

        if (TransitionValue <= 80)
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, 8);

        // Center sprites if English
        if (Localization.Language == 0)
        {
            Data.SoundsOnOffBase.ScreenPos = new Vector2(260 - TransitionValue / 2f, Data.SoundsOnOffBase.ScreenPos.Y);
            Data.MusicOnOff.ScreenPos = new Vector2(260 - TransitionValue / 2f, Data.MusicOnOff.ScreenPos.Y);
            Data.SfxOnOff.ScreenPos = new Vector2(260 - TransitionValue / 2f, Data.SfxOnOff.ScreenPos.Y);
        }
        else
        {
            Data.SoundsOnOffBase.ScreenPos = new Vector2(290 - TransitionValue / 2f, Data.SoundsOnOffBase.ScreenPos.Y);
            Data.MusicOnOff.ScreenPos = new Vector2(290 - TransitionValue / 2f, Data.MusicOnOff.ScreenPos.Y);
            Data.SfxOnOff.ScreenPos = new Vector2(290 - TransitionValue / 2f, Data.SfxOnOff.ScreenPos.Y);
        }

        if (TransitionValue >= 160)
        {
            TransitionValue = 0;
            CurrentStepAction = Step_Options;
        }

        AnimationPlayer.Play(Data.OptionsSelection);
        AnimationPlayer.Play(Data.SoundsOnOffBase);
        AnimationPlayer.PlayFront(Data.MusicOnOff);
        AnimationPlayer.PlayFront(Data.SfxOnOff);
    }

    private void Step_Options()
    {
        if (IsLoadingCredits)
        {
            if (TransitionsFX.IsFadeOutFinished)
                FrameManager.SetNextFrame(new Credits());
        }
        else
        {
            if (JoyPad.CheckSingle(GbaInput.Up) && Data.Cursor.CurrentAnimation == 0)
            {
                if (SelectedOption == 0)
                    SelectOption(2, true);
                else
                    SelectOption(SelectedOption - 1, true);

                Data.OptionsSelection.CurrentAnimation = Localization.LanguageUiIndex * 3 + SelectedOption;
            }
            else if (JoyPad.CheckSingle(GbaInput.Down) && Data.Cursor.CurrentAnimation == 0)
            {
                if (SelectedOption == 2)
                    SelectOption(0, true);
                else
                    SelectOption(SelectedOption + 1, true);

                Data.OptionsSelection.CurrentAnimation = Localization.LanguageUiIndex * 3 + SelectedOption;
            }
            else if (JoyPad.CheckSingle(GbaInput.B) && Data.Cursor.CurrentAnimation == 0)
            {
                NextStepAction = Step_InitializeTransitionToSelectGameMode;
                CurrentStepAction = Step_TransitionOutOfOptions;
                TransitionOutCursorAndStem();
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Store01_Mix01);
            }
            else if (JoyPad.CheckSingle(GbaInput.A) && Data.Cursor.CurrentAnimation == 0)
            {
                Data.Cursor.CurrentAnimation = 16;

                if (SelectedOption == 0)
                {
                    ToggleMusicOnOff();
                    UpdateMusicVolumeAnimations();
                }
                else if (SelectedOption == 1)
                {
                    ToggleSfxOnOff();
                    UpdateSfxVolumeAnimations();
                }

                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Valid01_Mix01);
            }

            if (Data.Cursor.CurrentAnimation == 16 && Data.Cursor.EndOfAnimation)
            {
                Data.Cursor.CurrentAnimation = 0;

                if (SelectedOption == 2)
                {
                    TransitionsFX.FadeOutInit(4 / 16f);
                    IsLoadingCredits = true;
                }
            }
        }

        AnimationPlayer.Play(Data.OptionsSelection);
        AnimationPlayer.Play(Data.SoundsOnOffBase);
        AnimationPlayer.PlayFront(Data.MusicOnOff);
        AnimationPlayer.PlayFront(Data.SfxOnOff);
    }

    private void Step_TransitionOutOfOptions()
    {
        TransitionValue += 4;

        if (TransitionValue <= 160)
        {
            Playfield.Camera.GetCluster(1).Position += new Vector2(0, -4);

            // Center sprites if English
            if (Localization.Language == 0)
            {
                Data.SoundsOnOffBase.ScreenPos = new Vector2(180 + TransitionValue / 2f, Data.SoundsOnOffBase.ScreenPos.Y);
                Data.MusicOnOff.ScreenPos = new Vector2(180 + TransitionValue / 2f, Data.MusicOnOff.ScreenPos.Y);
                Data.SfxOnOff.ScreenPos = new Vector2(180 + TransitionValue / 2f, Data.SfxOnOff.ScreenPos.Y);
            }
            else
            {
                Data.SoundsOnOffBase.ScreenPos = new Vector2(210 + TransitionValue / 2f, Data.SoundsOnOffBase.ScreenPos.Y);
                Data.MusicOnOff.ScreenPos = new Vector2(210 + TransitionValue / 2f, Data.MusicOnOff.ScreenPos.Y);
                Data.SfxOnOff.ScreenPos = new Vector2(210 + TransitionValue / 2f, Data.SfxOnOff.ScreenPos.Y);
            }
        }
        else if (TransitionValue >= 220)
        {
            TransitionValue = 0;
            CurrentStepAction = NextStepAction;
        }

        AnimationPlayer.Play(Data.OptionsSelection);
        AnimationPlayer.Play(Data.SoundsOnOffBase);
        AnimationPlayer.PlayFront(Data.MusicOnOff);
        AnimationPlayer.PlayFront(Data.SfxOnOff);
    }

    #endregion
}