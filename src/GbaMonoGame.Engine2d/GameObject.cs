﻿using GbaMonoGame.AnimEngine;
using ImGuiNET;

namespace GbaMonoGame.Engine2d;

public abstract class GameObject : Object
{
    protected GameObject(int instanceId, Scene2D scene, GameObjectResource gameObjectResource)
    {
        InstanceId = instanceId;
        Scene = scene;
        Position = gameObjectResource.Pos.ToVector2();

        IsEnabled = gameObjectResource.IsEnabled;
        IsAwake = gameObjectResource.IsAwake;
        Flag_2 = false;
        IsProjectile = gameObjectResource.IsProjectile;
        ResurrectsImmediately = gameObjectResource.ResurrectsImmediately;
        ResurrectsLater = gameObjectResource.ResurrectsLater;
        Flag_6 = gameObjectResource.Flag_6;
        Flag_7 = gameObjectResource.Flag_7;
    }

    public int InstanceId { get; }
    public Scene2D Scene { get; }
    public Vector2 Position { get; set; }

    // Flags
    public bool IsEnabled { get; set; }
    public bool IsAwake { get; set; }
    public bool Flag_2 { get; set; }
    public bool IsProjectile { get; set; }
    public bool ResurrectsImmediately { get; set; }
    public bool ResurrectsLater { get; set; }
    public bool Flag_6 { get; set; }
    public bool Flag_7 { get; set; }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        switch (message)
        {
            case Message.None:
                return true;

            case Message.WakeUp:
                IsAwake = true;
                return true;

            case Message.Sleep:
                IsAwake = false;
                return true;

            case Message.Destroy:
                IsEnabled = false;
                return true;

            case Message.Resurrect:
                IsEnabled = true;
                return true;

            case Message.ResurrectWakeUp:
                IsEnabled = true;
                IsAwake = true;
                return true;

            default:
                return false;
        }
    }

    public virtual void DrawDebugBoxes(AnimationPlayer animationPlayer)
    {
        // TODO: Display cross at origin position like in Ray1Map?
    }

    public override void DrawDebugLayout(DebugLayout debugLayout, DebugLayoutTextureManager textureManager)
    {
        ImGui.Text($"Projectile: {IsProjectile}");
        base.DrawDebugLayout(debugLayout, textureManager);
    }
}