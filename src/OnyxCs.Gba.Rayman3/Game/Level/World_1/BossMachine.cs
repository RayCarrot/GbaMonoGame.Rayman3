namespace OnyxCs.Gba.Rayman3;

public class BossMachine : FrameSideScroller
{
    public BossMachine(MapId mapId) : base(mapId) { }

    private TextBoxDialog TextBox { get; set; }

    public override void Init()
    {
        base.Init();

        TextBox = new TextBoxDialog();
        Scene.AddDialog(TextBox, false, false);
        TextBox.FUN_080174b0(0);
        TextBox.FUN_08017494(13);
    }
}