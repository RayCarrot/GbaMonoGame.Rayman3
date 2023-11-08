using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class Piranha : MovableActor
{
    public Piranha(int id, ActorResource actorResource) : base(id, actorResource)
    {
        InitPos = Position;
        Fsm.ChangeAction(Fsm_Wait);
    }

    private Vector2 InitPos { get; }
    private int Timer { get; set; }
    private bool ShouldDraw { get; set; }

    private void SpawnSplash()
    {
        BaseActor splash = Frame.GetComponent<Scene2D>().GameObjects.SpawnActor(ActorType.Splash);
        if (splash != null)
            splash.Position = Position;
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (ShouldDraw)
            base.Draw(animationPlayer, forceDraw);
    }
}