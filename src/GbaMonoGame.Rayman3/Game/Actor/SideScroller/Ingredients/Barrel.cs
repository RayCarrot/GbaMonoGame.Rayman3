using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed partial class Barrel : MovableActor
{
    public Barrel(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        InitialHitPoints = HitPoints;
        
        // Float right
        if ((Action)actorResource.FirstActionId == Action.FloatRight)
        {
            MoveOnWater = true;
            MoveRight = true;
        }
        // Float left
        else if ((Action)actorResource.FirstActionId == Action.FloatLeft)
        {
            MoveOnWater = true;
            MoveRight = false;
        }
        // Breakable (unused in final game - was meant to be blocking your path in world 1)
        else
        {
            MoveOnWater = false;
        }

        LastHitFacingLeft = false;
        BarrelSplash = null;
        LastHitBodyPartType = null;

        Fsm.ChangeAction(Fsm_WaitForHit);
    }

    public bool MoveOnWater { get; }
    public bool MoveRight { get; set; }
    public int InitialHitPoints { get; set; }
    public BarrelSplash BarrelSplash { get; set; }
    public Vector2 InitialWaterPosition { get; set; }
    public bool LastHitFacingLeft { get; set; }
    public RaymanBody.RaymanBodyPartType? LastHitBodyPartType { get; set; }
    public byte Timer { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (base.ProcessMessageImpl(message, param))
            return false;

        switch (message)
        {
            case Message.Hit:
                RaymanBody body = (RaymanBody)param;
                RaymanBody.RaymanBodyPartType bodyPartType = body.BodyPartType;

                if (MoveOnWater && 
                    bodyPartType is RaymanBody.RaymanBodyPartType.SuperFist or RaymanBody.RaymanBodyPartType.SecondSuperFist)
                {
                    Fsm.ChangeAction(Fsm_FallIntoWater);
                }

                if (Fsm.EqualsAction(Fsm_WaitForHit))
                    LastHitBodyPartType = bodyPartType;
                else if (Fsm.EqualsAction(Fsm_Hit) && bodyPartType != LastHitBodyPartType)
                    // In the game this is 0xFE and 0xFF is null, but we use -1 since we just need a different value
                    LastHitBodyPartType = (RaymanBody.RaymanBodyPartType?)-1;

                LastHitFacingLeft = body.IsFacingLeft;
                return false;

            default:
                return false;
        }
    }
}