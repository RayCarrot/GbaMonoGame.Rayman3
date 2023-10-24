using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public abstract class FrameSideScroller : Frame
{
    protected FrameSideScroller(MapId mapId)
    {
        GameInfo.SetNextMapId(mapId);
    }

    public Scene2D Scene { get; set; }

    private void ProcessDialogs()
    {
        foreach (Dialog dialog in Scene.Dialogs)
        {
            dialog.Fsm();
            dialog.Draw(Scene.AnimationPlayer);
        }
    }

    public override void Init()
    {
        Gfx.Clear();
        GameInfo.LoadedYellowLums = 0;
        GameInfo.GreenLums = 0;
        GameInfo.MapId = GameInfo.NextMapId;
        // TODO: More setup...
        Scene = new Scene2D((int)GameInfo.MapId, 4);
        // TODO: More setup...
        Scene.AddDialog(new UserInfoSideScroller(GameInfo.Level.HasBlueLum));
        // TODO: More setup...
    }

    public override void Step()
    {
        // TODO: Implement
        Scene.AnimationPlayer.Execute();

        foreach (BaseActor actor in Scene.Objects.Actors)
            actor.Draw(Scene.AnimationPlayer);
        ProcessDialogs();
    }
}