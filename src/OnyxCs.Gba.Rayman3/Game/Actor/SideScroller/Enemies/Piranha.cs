using Microsoft.Xna.Framework;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public class Piranha : MovableActor
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
        BaseActor splash = Frame.GetComponent<Scene2D>().Objects.SpawnActor(ActorType.Splash);
        if (splash != null)
            splash.Position = Position;
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (ShouldDraw)
            base.Draw(animationPlayer, forceDraw);
    }

    private void Fsm_Wait(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Position = InitPos;
                ActionId = IsFacingLeft ? 3 : 2;
                Timer = 0;
                ShouldDraw = false;
                break;

            case FsmAction.Step:
                Timer++;

                MovableActor mainActor = Frame.GetComponent<Scene2D>().GetMainActor();

                Rectangle actionBox = GetAbsoluteBox(ActionBox);
                Rectangle mainActorDetectionBox = mainActor.GetAbsoluteBox(mainActor.DetectionBox);

                if (actionBox.Intersects(mainActorDetectionBox) && Timer > 120)
                    Fsm.ChangeAction(Fsm_Move);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Move(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ShouldDraw = true;
                ActionId = IsFacingLeft ? 1 : 0;
                SpawnSplash();
                break;

            case FsmAction.Step:
                if (!IsActionFinished)
                {
                    Scene2D scene = Frame.GetComponent<Scene2D>();
                    if (scene.IsHitMainActor(this))
                        scene.DamageMainActor(AttackPoints);
                }
                else
                {
                    SpawnSplash();
                }

                if (HitPoints == 0)
                    Fsm.ChangeAction(Fsm_Dying);
                else if (IsActionFinished)
                    Fsm.ChangeAction(Fsm_Wait);   
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Dying(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingLeft ? 3 : 2;

                break;

            case FsmAction.Step:
                // TODO: Implement
                break;

            case FsmAction.UnInit:
                SpawnSplash();
                SendMessage(Message.Disable);
                break;
        }
    }
}