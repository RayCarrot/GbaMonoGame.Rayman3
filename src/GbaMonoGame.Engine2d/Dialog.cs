﻿using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Engine2d;

public abstract class Dialog : Object
{
    protected Dialog(Scene2D scene)
    {
        Scene = scene;
    }

    protected FiniteStateMachine State { get; } = new();

    public Scene2D Scene { get; }

    public void Step() => State.Step();
    public abstract void Load();
    public virtual void Init() { }
    public abstract void Draw(AnimationPlayer animationPlayer);
}