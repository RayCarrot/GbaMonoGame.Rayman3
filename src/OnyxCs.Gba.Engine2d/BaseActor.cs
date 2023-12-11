using BinarySerializer.Onyx.Gba;
using ImGuiNET;
using OnyxCs.Gba.AnimEngine;

namespace OnyxCs.Gba.Engine2d;

public class BaseActor : GameObject
{
    // NOTE: The game allows actors to pass in "user-defined" AObject classes. However the game handles this in a rather
    //       ugly way where it will by default assume it's of type AnimatedObject, so the class then has to override all
    //       of this behavior. We will however try and only have a single AnimatedObject in here.
    public BaseActor(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        ActorModel = actorResource.Model;
        ActorFlag_6 = ActorModel.Flag_06;
        IsAgainstCaptor = ActorModel.IsAgainstCaptor;
        ReceivesDamage = ActorModel.ReceivesDamage;
        Type = actorResource.Type;
        ActorFlag_C = true;

        AnimatedObject = new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic)
        {
            CurrentAnimation = 0,
            SpritePriority = ActorDrawPriority,
            YPriority = 32
        };

        _viewBox = new Box(ActorModel.ViewBox);
    }

    public static int ActorDrawPriority { get; set; }

    private readonly Box _viewBox;

    public ActorModel ActorModel { get; }
    public int Type { get; }
    public AnimatedObject AnimatedObject { get; }

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

    public Box GetViewBox() => _viewBox.Offset(Position);

    public void RewindAction()
    {
        AnimatedObject.CurrentFrame = 0;
    }

    public virtual void Init() { }

    public virtual void DoBehavior()
    {
        Fsm.Step();
    }

    public virtual void Step() { }

    public virtual void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Scene.Camera.IsActorFramed(this) || forceDraw)
        {
            animationPlayer.AddSortedObject(AnimatedObject);
        }
        else
        {
            AnimatedObject.ExecuteUnframed();
        }
    }

    public override void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        base.DrawDebugLayout(debugLayout, textureManager);

        ImGui.Text($"State: {Fsm}");
        ImGui.Text($"Direction: {(IsFacingLeft ? "Left" : "Right")}");
        ImGui.Text($"Y-prio: {AnimatedObject.YPriority}");
    }
}