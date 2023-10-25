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

    public Page CurrentPage { get; set; }

    #endregion

    #region Private Methods

    private void LoadPlayfield()
    {
        PlayfieldResource menuPlayField = Storage.LoadResource<PlayfieldResource>(0x5b);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(menuPlayField);
    }

    #endregion

    #region Public Override Methods

    public override void Init()
    {
        AnimationPlayer = new AnimationPlayer(false);

        LoadPlayfield();
    }

    public override void Step()
    {
        // TODO: Implement
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