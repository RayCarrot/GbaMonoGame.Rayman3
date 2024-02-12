namespace GbaMonoGame.Rayman3;

// TODO: A lot more to implement here. Also look into how N-Gage does the fade transition.
public class World : FrameWorldSideScroller
{
    public World(MapId mapId) : base(mapId) { }

    public override void Init()
    {
        base.Init();

        // Only implement this for now so the teensies work
        Scene.AddDialog(new TextBoxDialog(), false, false);
    }
}