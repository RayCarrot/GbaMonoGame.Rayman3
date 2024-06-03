using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class RaymanSparkle : BaseActor
{
    public RaymanSparkle(int instanceId, Scene2D scene, ActorResource actorResource) 
        : base(instanceId, scene, actorResource, new AObjectChain(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic))
    {
        AnimatedObject.Init(6, Position, 0, true);
        AnimatedObject.YPriority = 10;

        // TODO: Set static field DAT_03000dfc to 0

        MainActor1 = Scene.MainActor;
        MainActor2 = Scene.MainActor;

        if (RSMultiplayer.IsActive)
        {
            AreSparklesFacingLeft = true;
            field13_0x38 = 0xFFFF;
            Fsm.ChangeAction(FUN_08060f58);
        }
        else
        {
            AreSparklesFacingLeft = false;
            field13_0x38 = 360;
            Fsm.ChangeAction(FUN_08060930);
        }
    }

    public new AObjectChain AnimatedObject => (AObjectChain)base.AnimatedObject;

    public BaseActor MainActor1 { get; set; }
    public BaseActor MainActor2 { get; set; }
    public bool AreSparklesFacingLeft { get; set; }
    public byte SwirlValue { get; set; }
    public ushort Timer { get; set; }
    public ushort field13_0x38 { get; set; } // TODO: Name

    public void InitNewPower()
    {
        Fsm.ChangeAction(Fsm_NewPower, unInit: false);
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        AnimatedObject.Draw(this, animationPlayer, forceDraw);
    }
}