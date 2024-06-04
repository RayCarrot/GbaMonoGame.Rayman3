namespace GbaMonoGame.Rayman3;

public class SanctuaryOfStoneAndFire_M1 : FrameSideScroller
{
    public SanctuaryOfStoneAndFire_M1(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        Scene.AddDialog(new TextBoxDialog(Scene), false, false);
    }
}