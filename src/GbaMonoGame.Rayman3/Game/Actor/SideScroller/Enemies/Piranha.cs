using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Piranha : MovableActor
{
    public Piranha(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        InitPos = Position;
        Fsm.ChangeAction(Fsm_Wait);
    }

    private Vector2 InitPos { get; }
    private int Timer { get; set; }
    private bool ShouldDraw { get; set; }

    private void SpawnSplash()
    {
        Splash splash = Scene.KnotManager.CreateProjectile<Splash>(ActorType.Splash);
        if (splash != null)
            splash.Position = Position;
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (ShouldDraw)
            base.Draw(animationPlayer, forceDraw);
    }
}