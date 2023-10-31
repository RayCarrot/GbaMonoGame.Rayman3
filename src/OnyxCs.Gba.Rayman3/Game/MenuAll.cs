using System;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Rayman3;

public class MenuAll : Frame
{
    #region Constructor

    public MenuAll(Page startPage)
    {
        CurrentPage = startPage;
    }

    #endregion

    #region Private Properties

    private AnimationPlayer AnimationPlayer { get; set; }
    private TgxPlayfield2D Playfield { get; set; }

    private MenuData Data { get; set; }
    private Action CurrentStepAction { get; set; }

    private int SelectedOption { get; set; }

    public Page CurrentPage { get; set; }

    #endregion

    #region Private Methods

    private void LoadPlayfield()
    {
        PlayfieldResource menuPlayField = Storage.LoadResource<PlayfieldResource>(0x5b);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(menuPlayField);
    }

    private void SwitchPage()
    {
        switch (CurrentPage)
        {
            case Page.Language:
                CurrentStepAction = Step_Language;
                break;

            case Page.GameModes:
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
    }

    #endregion

    #region Public Override Methods

    public override void Init()
    {
        AnimationPlayer = new AnimationPlayer(false);

        Data = new MenuData();

        LoadPlayfield();
        SwitchPage();
    }

    public override void Step()
    {
        AnimationPlayer.Execute();
        CurrentStepAction();
    }

    #endregion

    #region Steps

    private void Step_Language()
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
            // TODO: Select language
        }


        AnimationPlayer.AddSecondaryObject(Data.LanguageList);
    }

    #endregion

    #region Data Types

    public enum Page
    {
        Language,
        GameModes,
        Options,
        MultiPak,
        SinglePak,
    }

    #endregion
}