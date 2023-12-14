namespace OnyxCs.Gba.Rayman3;

public class MenhirHills_M1 : FrameSideScroller
{
    public MenhirHills_M1(MapId mapId) : base(mapId) { }

    private TextBoxDialog TextBox { get; set; }

    public override void Init()
    {
        base.Init();

        TextBox = new TextBoxDialog();
        Scene.AddDialog(TextBox, false, false);
    }
}