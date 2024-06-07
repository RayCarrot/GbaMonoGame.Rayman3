﻿using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.AnimEngine;
using ImGuiNET;

namespace GbaMonoGame.Engine2d;

public abstract class BaseActor : GameObject
{
    protected BaseActor(int instanceId, Scene2D scene, ActorResource actorResource) 
        : this(instanceId, scene, actorResource, new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic)) { }

    protected BaseActor(int instanceId, Scene2D scene, ActorResource actorResource, AnimatedObject animatedObject) 
        : base(instanceId, scene, actorResource)
    {
        ActorModel = actorResource.Model;
        IsSolid = ActorModel.IsSolid;
        IsAgainstCaptor = ActorModel.IsAgainstCaptor;
        ReceivesDamage = ActorModel.ReceivesDamage;
        Type = actorResource.Type;
        HasMoved = true;
        AnimatedObject = animatedObject;

        // Initialize the animated object. In the original game this is optional since the
        // animated object can be user defined and set to another AObject type.
        animatedObject.CurrentAnimation = 0;
        animatedObject.SpritePriority = ActorDrawPriority;
        animatedObject.YPriority = 32;
        animatedObject.Camera = scene.Playfield.Camera;

        _viewBox = new Box(ActorModel.ViewBox);
    }

    public static int ActorDrawPriority { get; set; }

    private readonly Box _viewBox;

    public ActorModel ActorModel { get; }
    public int Type { get; }
    public AnimatedObject AnimatedObject { get; }

    public FiniteStateMachine State { get; } = new();

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
    public bool IsObjectCollisionXOnly { get; set; }

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
        State.Step();
    }

    public virtual void Step() { }

    public virtual void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        DrawDefault(animationPlayer, forceDraw);
    }

    public override void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        base.DrawDebugLayout(debugLayout, textureManager);

        ImGui.Text($"State: {State}");
        ImGui.Text($"Direction: {(IsFacingLeft ? "Left" : "Right")}");
        ImGui.Text($"Y-prio: {AnimatedObject.YPriority}");
        ImGui.Text($"Animation: {AnimatedObject.CurrentAnimation}");
    }
}