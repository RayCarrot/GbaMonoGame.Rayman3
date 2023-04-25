using BinarySerializer.Onyx.Gba;
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

    public Page CurrentPage { get; set; }

    #endregion

    #region Public Override Methods

    public override void Init(FrameMngr frameMngr)
    {
        Engine.Instance.Vram.ClearSprites();
        Engine.Instance.Vram.ClearBackgrounds();

        AnimationPlayer.Instance.Init(false, null);

        PlayfieldResource menuPlayField = Storage.ReadResource<Playfield>(0x5b);

        TgxPlayfield.Load(menuPlayField);
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