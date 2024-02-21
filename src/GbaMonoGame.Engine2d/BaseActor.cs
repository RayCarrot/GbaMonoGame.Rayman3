﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;
using ImGuiNET;

namespace GbaMonoGame.Engine2d;

public class BaseActor : GameObject
{
    // NOTE: The game allows actors to pass in "user-defined" AObject classes. However the game handles this in a rather
    //       ugly way where it will by default assume it's of type AnimatedObject, so the class then has to override all
    //       of this behavior. We however only use a single AnimatedObject in the engine, thus making this cleaner.
    public BaseActor(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        ActorModel = actorResource.Model;
        IsSolid = ActorModel.IsSolid;
        IsAgainstCaptor = ActorModel.IsAgainstCaptor;
        ReceivesDamage = ActorModel.ReceivesDamage;
        Type = actorResource.Type;
        HasMoved = true;

        AnimatedObject = new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic)
        {
            CurrentAnimation = 0,
            SpritePriority = ActorDrawPriority,
            YPriority = 32,
            Camera = scene.Playfield.Camera,
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
    public Vector2 ScreenPosition
    {
        get => AnimatedObject.ScreenPos;
        set => AnimatedObject.ScreenPos = value;
    }

    // Flags
    public bool IsSolid { get; set; }
    public bool IsAgainstCaptor { get; set; }
    public bool ReceivesDamage { get; set; }
    public bool IsInvulnerable { get; set; }
    public bool IsTouchingActor { get; set; }
    public bool IsTouchingMap { get; set; }
    public bool HasMoved { get; set; }
    public bool ActorFlag_E { get; set; }

    protected void DrawDefault(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Scene.Camera.IsActorFramed(this) || forceDraw)
        {
            AnimatedObject.IsFramed = true;
            animationPlayer.Play(AnimatedObject);
        }
        else
        {
            AnimatedObject.IsFramed = false;
            AnimatedObject.ComputeNextFrame();
        }
    }

    protected void DrawLarge(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (Scene.Camera.IsActorFramed(this) || forceDraw)
        {
            AnimatedObject.IsFramed = true;
            AnimatedObject.FrameChannelSprite();
            animationPlayer.Play(AnimatedObject);
        }
        else
        {
            AnimatedObject.IsFramed = false;
            AnimatedObject.ComputeNextFrame();
        }
    }

    public Box GetViewBox() => _viewBox.Offset(Position);

    public void RewindAction()
    {
        AnimatedObject.Rewind();
    }

    public virtual void Init() { }

    public virtual void DoBehavior()
    {
        Fsm.Step();
    }

    public virtual void Step() { }

    public virtual void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        DrawDefault(animationPlayer, forceDraw);
    }

    public override void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        base.DrawDebugLayout(debugLayout, textureManager);

        ImGui.Text($"State: {Fsm}");
        ImGui.Text($"Direction: {(IsFacingLeft ? "Left" : "Right")}");
        ImGui.Text($"Y-prio: {AnimatedObject.YPriority}");
        ImGui.Text($"Animation: {AnimatedObject.CurrentAnimation}");
    }
}