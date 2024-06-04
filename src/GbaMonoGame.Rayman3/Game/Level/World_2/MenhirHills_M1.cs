namespace GbaMonoGame.Rayman3;

public class MenhirHills_M1 : FrameSideScroller
{
    public MenhirHills_M1(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        Scene.AddDialog(new TextBoxDialog(Scene), false, false);
    }
}