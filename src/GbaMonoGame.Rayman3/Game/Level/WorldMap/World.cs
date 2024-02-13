namespace GbaMonoGame.Rayman3;

// TODO: A lot more to implement here. Also look into how N-Gage does the fade transition.
public class World : FrameWorldSideScroller
{
    public World(MapId mapId) : base(mapId) { }

    public UserInfoWorld UserInfo { get; set; }

    public override void Init()
    {
        base.Init();

        Scene.AddDialog(UserInfo = new UserInfoWorld(Scene, GameInfo.Level.HasBlueLum), false, false);
        Scene.AddDialog(new TextBoxDialog(), false, false);
    }

    public override void Step()
    {
        base.Step();

        // TODO: This is temporary code
        if (UserInfo.CurtainsLeft.EndOfAnimation && UserInfo.CurtainsLeft.CurrentAnimation != 0)
            UserInfo.CurtainsLeft.CurrentAnimation = 0;
        if (UserInfo.CurtainsRight.EndOfAnimation && UserInfo.CurtainsRight.CurrentAnimation != 0)
            UserInfo.CurtainsRight.CurrentAnimation = 0;
    }
}