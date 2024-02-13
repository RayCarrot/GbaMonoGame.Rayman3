using System;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

// PetitsEtres
public sealed partial class Teensies : ActionActor
{
    public Teensies(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        if (ActionId is
            Action.Init_Master_Right or Action.Init_Master_Left or
            Action.Init_World1_Right or Action.Init_World1_Left or
            Action.Init_World2_Right or Action.Init_World2_Left or
            Action.Init_World3_Right or Action.Init_World3_Left or
            Action.Init_World4_Right or Action.Init_World4_Left)
        {
            InitialActionId = ActionId;

            if ((ActionId is Action.Init_World1_Right or Action.Init_World1_Left && GameInfo.PersistentInfo.UnlockedWorld2) ||
                (ActionId is Action.Init_World2_Right or Action.Init_World2_Left && GameInfo.PersistentInfo.UnlockedWorld3) ||
                (ActionId is Action.Init_World3_Right or Action.Init_World3_Left && GameInfo.PersistentInfo.UnlockedWorld4) ||
                (ActionId is Action.Init_World4_Right or Action.Init_World4_Left && GameInfo.PersistentInfo.UnlockedFinalBoss))
            {
                ProcessMessage(Message.Destroy);
            }
            else
            {
                ActionId = IsFacingRight ? Action.Init_Master_Right : Action.Init_Master_Left;
                Fsm.ChangeAction(Fsm_WaitMaster);
            }
        }
        else if (ActionId is Action.Init_Victory_Right or Action.Init_Victory_Left)
        {
            Fsm.ChangeAction(Fsm_VictoryDance);
        }
        else
        {
            Fsm.ChangeAction(Fsm_Idle);
        }
    }

    private Action InitialActionId { get; set; }
    private byte field3_0x35 { get; set; }
    private bool HasSetTextBox { get; set; }
    private TextBoxDialog TextBox { get; set; }

    private bool IsWorldFinished()
    {
        if (InitialActionId is Action.Init_World1_Right or Action.Init_World1_Left)
            return GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.SanctuaryOfBigTree_M2;
        else if (InitialActionId is Action.Init_World2_Right or Action.Init_World2_Left)
            return GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.MarshAwakening2;
        else if (InitialActionId is Action.Init_World3_Right or Action.Init_World3_Left)
            return GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.SanctuaryOfRockAndLava_M3;
        else if (InitialActionId is Action.Init_World4_Right or Action.Init_World4_Left)
            return GameInfo.PersistentInfo.LastCompletedLevel >= (int)MapId.PirateShip_M2;
        else
            throw new Exception("Invalid initial action id for teensies");
    }

    private bool IsEnoughCagesTaken()
    {
        if (InitialActionId is Action.Init_World1_Right or Action.Init_World1_Left)
        {
            if (GameInfo.GetTotalCollectedCages() >= 5)
            {
                GameInfo.PersistentInfo.UnlockedWorld2 = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (InitialActionId is Action.Init_World2_Right or Action.Init_World2_Left)
        {
            if (GameInfo.GetTotalCollectedCages() >= 10)
            {
                GameInfo.PersistentInfo.UnlockedWorld3 = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (InitialActionId is Action.Init_World3_Right or Action.Init_World3_Left)
        {
            if (GameInfo.GetTotalCollectedCages() >= 15)
            {
                GameInfo.PersistentInfo.UnlockedWorld4 = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (InitialActionId is Action.Init_World4_Right or Action.Init_World4_Left)
        {
            if (GameInfo.GetTotalCollectedCages() >= 20)
            {
                GameInfo.PersistentInfo.UnlockedFinalBoss = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            throw new Exception("Invalid initial action id for teensies");
        }
    }
}