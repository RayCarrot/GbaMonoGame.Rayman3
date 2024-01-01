using OnyxCs.Gba.Engine2d;

namespace OnyxCs.Gba.Rayman3;

public sealed partial class RaymanBody : MovableActor
{
    public RaymanBody(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        Rayman = Scene.MainActor;
        AnimatedObject.YPriority = 18;
        Fsm.ChangeAction(Fsm_Wait);
    }

    public MovableActor Rayman { get; set; }
    public RaymanBodyPartType BodyPartType { get; set; }
    public uint ChargePower { get; set; }
    public bool HasCharged { get; set; }
    public byte field4_0x66 { get; set; }
    public int field7_0x6c { get; set; }

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        if (base.ProcessMessageImpl(message, param))
            return false;

        switch (message)
        {
            case (Message)1002: // TODO: Name message
                // TODO: Implement
                return false;

            default:
                return false;
        }
    }

    public enum RaymanBodyPartType
    {
        Fist = 0,
        SecondFist = 1,
        Foot = 2,
        Torso = 3,
        HitEffect = 4,
        SuperFist = 5,
        SecondSuperFist = 6,
    }
}