using System;
using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Lums : BaseActor
{
    public Lums(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        // NOTE: In the game it creates a special version of the AnimatedObject for this class called AObjectLum.
        //       That allows a palette to be defined, and doesn't handle things like sound events, boxes etc. We
        //       can however keep using the default AnimatedObject class here.

        LumId = 0;
        ActionId = (Action)actorResource.FirstActionId;

        switch (ActionId)
        {
            case Action.YellowLum:
            case Action.RedLum:
            case Action.GreenLum:
                AnimatedObject.CurrentAnimation = (byte)ActionId * 3;

                if (ActionId == Action.YellowLum && !IsProjectile && !MultiplayerManager.IsInMultiplayer)
                {
                    LumId = GameInfo.LoadedYellowLums;
                    GameInfo.LoadedYellowLums++;

                    if (GameInfo.HasCollectedYellowLum(LumId, GameInfo.MapId))
                        ProcessMessage(Message.Destroy);
                }
                break;

            case Action.BlueLum:
                if (!MultiplayerManager.IsInMultiplayer)
                    AnimatedObject.PaletteIndex = 1;

                AnimatedObject.CurrentAnimation = 0;
                break;

            case Action.WhiteLum:
                if (!MultiplayerManager.IsInMultiplayer)
                {
                    AnimatedObject.PaletteIndex = 1;

                    if (GameInfo.field22_0x1b)
                        ProcessMessage(Message.Destroy);
                }

                AnimatedObject.CurrentAnimation = 3;
                break;

            case Action.BigYellowLum:
                AnimatedObject.CurrentAnimation = 10;
                break;

            case Action.BigBlueLum:
                if (!MultiplayerManager.IsInMultiplayer)
                    AnimatedObject.PaletteIndex = 1;

                AnimatedObject.CurrentAnimation = 10;
                break;

            default:
                throw new Exception($"Unknown lum state {ActionId}");
        }

        if (!MultiplayerManager.IsInMultiplayer)
        {
            Fsm.ChangeAction(Fsm_Idle);

            if (ActionId == Action.GreenLum)
            {
                LumId = GameInfo.GreenLums;
                GameInfo.GreenLums++;

                if (LumId < GameInfo.LastGreenLumAlive)
                    ProcessMessage(Message.Destroy);
            }
        }
        else
        {
            // TODO: Implement
        }
    }

    private int LumId { get; }
    private int BossDespawnTimer { get; set; }

    private Box GetCollisionBox()
    {
        Box viewBox = GetViewBox();
        return new Box(viewBox.MinX + 16, viewBox.MinY + 16, viewBox.MaxX - 16, viewBox.MaxY - 16);
    }

    private bool CheckCollision()
    {
        return Scene.IsDetectedMainActor(GetCollisionBox());
    }

    private bool CheckCollisionAndAttract()
    {
        bool collided = Scene.IsDetectedMainActor(GetCollisionBox());

        // Move the lum towards the main actor
        if (!collided)
        {
            Box detectionBox = Scene.MainActor.GetDetectionBox();
            Vector2 detectionCenter = detectionBox.Center;

            if (Position.X < detectionCenter.X)
                Position += new Vector2(3.5f, 0);
            else
                Position -= new Vector2(3.5f, 0);

            if (Position.Y < detectionCenter.Y)
                Position += new Vector2(0, 3.5f);
            else
                Position -= new Vector2(0, 3.5f);
        }

        return collided;
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        // Intercept messages
        switch (message)
        {
            // When is this ever used?
            case Message.Resurrect:
                if (ActionId == Action.YellowLum && !GameInfo.HasCollectedYellowLum(LumId, GameInfo.MapId))
                    return false;
                break;
        }

        if (base.ProcessMessageImpl(message, param))
            return false;

        // Handle messages
        switch (message)
        {
            // TODO: Implement
            //case 1063:
            //    return false;

            // TODO: Implement
            //case 1087:
            //    return false;

            default:
                return false;
        }
    }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        if (!Fsm.EqualsAction(FUN_0805ed40) && !Fsm.EqualsAction(FUN_0805e6b8) && !Fsm.EqualsAction(FUN_0805e83c))
        {
            if (Scene.Camera.IsActorFramed(this))
            {
                AnimatedObject.IsFramed = true;
                animationPlayer.Play(AnimatedObject);
            }
            else
            {
                AnimatedObject.IsFramed = false;

                if (MultiplayerManager.IsInMultiplayer)
                    AnimatedObject.ComputeNextFrame();
            }
        }
    }
}