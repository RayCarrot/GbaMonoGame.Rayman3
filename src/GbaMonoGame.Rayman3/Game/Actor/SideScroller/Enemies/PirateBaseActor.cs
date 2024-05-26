using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public abstract class PirateBaseActor : MovableActor
{
    protected PirateBaseActor(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        IsObjectCollisionXOnly = true;
        Resource = actorResource;
        SpawnsRedLum = false;
        Reset();
    }

    protected ActorResource Resource { get; }

    protected bool SpawnsRedLum { get; set; }
    protected bool PirateFlag_1 { get; set; } // TODO: Name

    protected uint InvulnerabilityTimer { get; set; }
    protected uint IdleDetectionTimer { get; set; }
    protected bool HitFromFront { get; set; }
    protected int PrevHitPoints { get; set; }

    private void Reset()
    {
        InvulnerabilityTimer = 0;
        IdleDetectionTimer = 0;
        HitFromFront = false;
        PrevHitPoints = HitPoints;
        PirateFlag_1 = false;
    }

    protected void SpawnRedLum(float offsetX)
    {
        if (!SpawnsRedLum)
            return;

        Lums lum = Scene.KnotManager.CreateProjectile<Lums>(ActorType.Lums);

        float xPos;
        if ((IsFacingLeft && HitFromFront) ||
            (!IsFacingLeft && !HitFromFront))
            xPos = Position.X - offsetX;
        else
            xPos = Position.X + offsetX;

        lum?.InitializeRedLumProjectile(new Vector2(xPos, Position.Y - 25));
    }

    protected void StartInvulnerability()
    {
        InvulnerabilityTimer = GameTime.ElapsedFrames;
        IsInvulnerable = true;
    }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        // Intercept messages
        switch (message)
        {
            case Message.Resurrect:
                Position = Resource.Pos.ToVector2();
                HitPoints = ActorModel.HitPoints;
                Reset();
                ReInit();
                break;
        }

        if (base.ProcessMessageImpl(message, param))
            return false;

        // Handle messages
        switch (message)
        {
            case Message.Hit:
                BaseActor hitActor = (BaseActor)param;

                if ((hitActor.IsFacingLeft && IsFacingRight) ||
                    (hitActor.IsFacingRight && IsFacingLeft))
                {
                    HitFromFront = false;
                }
                else
                {
                    HitFromFront = true;
                }
                return false;

            default:
                return false;
        }
    }

    protected abstract void ReInit();

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        DrawWithInvulnerability(animationPlayer, forceDraw);
    }
}