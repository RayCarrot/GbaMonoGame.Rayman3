using BinarySerializer.Ubisoft.GbaEngine;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Rayman3;

public partial class MenuAll
{
    #region Steps

    private void Step_SelectLanguage()
    {
        if (JoyPad.IsButtonJustPressed(GbaInput.Up))
        {
            if (SelectedOption == 0)
                SelectedOption = 9;
            else
                SelectedOption--;

            Data.LanguageList.CurrentAnimation = SelectedOption;

            // TODO: Game passes in 0 as obj here, but that's probably a mistake
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
        }
        else if (JoyPad.IsButtonJustPressed(GbaInput.Down))
        {
            if (SelectedOption == 9)
                SelectedOption = 0;
            else
                SelectedOption++;

            Data.LanguageList.CurrentAnimation = SelectedOption;

            // TODO: Game passes in 0 as obj here, but that's probably a mistake
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__MenuMove);
        }
        else if (JoyPad.IsButtonJustPressed(GbaInput.A))
        {
            CurrentStepAction = Step_TransitionOutOfLanguage;

            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Valid01_Mix01);
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Switch1_Mix03);

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
                Data.GameModeList.ScreenPos = Data.GameModeList.ScreenPos with { X = 86 };
                Data.Cursor.ScreenPos = Data.Cursor.ScreenPos with { X = 46 };
                Data.Stem.ScreenPos = Data.Stem.ScreenPos with { X = 60 };
            }

            ResetStem();
        }

        AnimationPlayer.Play(Data.LanguageList);
    }

    private void Step_TransitionOutOfLanguage()
    {
        TgxCluster mainCluster = Playfield.Camera.GetMainCluster();
        mainCluster.Position += new Vector2(0, 3);

        Data.LanguageList.ScreenPos = Data.LanguageList.ScreenPos with { Y = TransitionValue + 28 };
        Data.LanguageList.FrameChannelSprite();
        AnimationPlayer.Play(Data.LanguageList);

        MoveGameLogo();

        Data.GameLogo.FrameChannelSprite(); // NOTE The game gives the bounding box a width of 255 instead of 240 here
        AnimationPlayer.Play(Data.GameLogo);

        AnimationPlayer.Play(Data.GameModeList);

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

    #endregion
}