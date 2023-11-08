using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Engine2d;

public class BaseActor : GameObject
{
    // NOTE: The game allows actors to pass in "user-defined" AObject classes. However the game handles this in a rather
    //       ugly way where it will by default assume it's of type AnimatedObject, so the class then has to override all
    //       of this behavior. We will however try and only have a single AnimatedObject in here.
    public BaseActor(int id, ActorResource actorResource) : base(id, actorResource)
    {
        ActorModel = actorResource.Model;
        ActorFlag_6 = ActorModel.Flag_06;
        IsAgainstCaptor = ActorModel.IsAgainstCaptor;
        ReceivesDamage = ActorModel.ReceivesDamage;
        Type = actorResource.Type;
        ActorFlag_C = true;

        AnimatedObject = new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic);
        AnimatedObject.SetCurrentAnimation(0);
        AnimatedObject.Priority = ActorDrawPriority;

        ViewBox = ActorModel.ViewBox.ToRectangle();
    }

    public static int ActorDrawPriority { get; set; }

    public ActorModel ActorModel { get; }
    public int Type { get; }
    public AnimatedObject AnimatedObject { get; }
    public Rectangle ViewBox { get; }

    public FiniteStateMachine Fsm { get; } = new();

    public virtual int ActionId { get; set; }
    public bool IsActionFinished => AnimatedObject.EndOfAnimation;
    public bool IsFacingLeft => AnimatedObject.FlipX;
    public bool IsFacingRight => !IsFacingLeft;
    public Vector2 ScreenPosition => AnimatedObject.ScreenPos;

    // Flags
    public bool ActorFlag_6 { get; set; }
    public bool IsAgainstCaptor { get; set; }
    public bool ReceivesDamage { get; set; }
    public bool IsInvulnerable { get; set; }
    public bool IsTouchingActor { get; set; }
    public bool IsTouchingMap { get; set; }
    public bool ActorFlag_C { get; set; }
    public bool ActorFlag_E { get; set; }

    public void RewindAction()
    {
        AnimatedObject.SetCurrentFrame(0);
    }

    public virtual void Init() { }

    public virtual void DoBehavior()
    {
        Fsm.Step();
    }

    public virtual void Step() { }

    public virtual void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        CameraActor camera = Frame.GetComponent<Scene2D>().Camera;

        if (camera.IsActorFramed(this) || forceDraw)
        {
            animationPlayer.AddSecondaryObject(AnimatedObject);
        }
        else
        {
            AnimatedObject.ExecuteUnframed();
        }
    }
}