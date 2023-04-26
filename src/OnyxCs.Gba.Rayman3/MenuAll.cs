#nullable disable
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Sdk;
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

    #region Public Properties

    public AnimationPlayer AnimationPlayer { get; set; }
    public TgxPlayfield2D Playfield { get; set; }

    public Page CurrentPage { get; set; }

    #endregion

    #region Private Methods

    private void LoadPlayfield()
    {
        PlayfieldResource menuPlayField = Storage.ReadResource<PlayfieldResource>(0x5b);
        Playfield = TgxPlayfield.Load<TgxPlayfield2D>(menuPlayField);
    }

    #endregion

    #region Public Override Methods

    public override void Init(Engine engine)
    {
        base.Init(engine);

        Engine.Vram.ClearAll();

        AnimationPlayer = new AnimationPlayer(Engine.Vram, false, null);

        LoadPlayfield();
    }

    public override void UnInit() { }

    public override void Step()
    {

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