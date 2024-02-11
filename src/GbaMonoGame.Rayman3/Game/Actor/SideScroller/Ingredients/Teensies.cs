using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// TODO: ActionId enum

// PetitsEtres
public sealed partial class Teensies : ActionActor
{
    public Teensies(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        if (ActionId is 
            19 or 18 or 
            31 or 30 or 
            33 or 32 or 
            35 or 34 or 
            37 or 36)
        {
            InitialActionId = ActionId;

            if ((ActionId is 31 or 30 && GameInfo.PersistentInfo.UnlockedWorld2) ||
                (ActionId is 33 or 32 && GameInfo.PersistentInfo.UnlockedWorld3) ||
                (ActionId is 35 or 34 && GameInfo.PersistentInfo.UnlockedWorld4) ||
                (ActionId is 37 or 36 && GameInfo.PersistentInfo.UnlockedFinalBoss))
            {
                ProcessMessage(Message.Destroy);
            }
            else
            {
                ActionId = IsFacingRight ? 19 : 18;
                Fsm.ChangeAction(Fsm_WaitMaster);
            }
        }
        else if (ActionId is 1 or 0)
        {
            Fsm.ChangeAction(Fsm_VictoryDance);
        }
        else
        {
            Fsm.ChangeAction(Fsm_Idle);
        }
    }

    private int InitialActionId { get; set; }
    private byte field3_0x35 { get; set; }
    private bool HasSetTextBox { get; set; }
    private TextBoxDialog TextBox { get; set; }
}