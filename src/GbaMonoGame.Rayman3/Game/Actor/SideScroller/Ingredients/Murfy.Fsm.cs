using System;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class Murfy
{
    private void Fsm_PreInit(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                InitialPosition = Position;
                TextBox = Scene.GetRequiredDialog<TextBoxDialog>();
                Fsm.ChangeAction(Fsm_Init);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Init(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ShouldSpawn = false;
                Position = InitialPosition;
                break;

            case FsmAction.Step:
                MechModel.Speed = Vector2.Zero;
                if (ShouldSpawn)
                    Fsm.ChangeAction(Fsm_WaitToSpawn);
                break;

            case FsmAction.UnInit:
                SetText();
                break;
        }
    }

    private void Fsm_WaitToSpawn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                MechModel.Speed = Vector2.Zero;
                if (ShouldSpawn)
                {
                    MoveTextBoxIn = false;
                    Fsm.ChangeAction(Fsm_MoveIn);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_MoveIn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (MainActor == Scene.MainActor) // Should never be false, so why is this here?
                {
                    FUN_08071fb0();
                    Scene.MainActor.ProcessMessage(Message.Main_EnterCutscene);
                    if (MainActor.Position.X < 120)
                    {
                        MainActor.ChangeAction();
                        MainActor.AnimatedObject.FlipX = false;
                        MainActor.ChangeAction();
                    }
                }

                if (MainActor.Position.X < Position.X)
                {
                    ActionId = 17;
                    TargetPosition = new Vector2(MainActor.Position.X - 45, MainActor.Position.Y);
                }
                else
                {
                    ActionId = 16;
                    TargetPosition = new Vector2(MainActor.Position.X + 45, MainActor.Position.Y);
                }

                SavedSpeed = Vector2.Zero;

                TextBox.MoveInOurOut(MoveTextBoxIn);

                if (GameInfo.MapId is not (MapId.World1 or MapId.World2 or MapId.World3 or MapId.World4))
                    ((FrameSideScroller)Frame.Current).UserInfo.MoveOutBars();
                break;

            case FsmAction.Step:
                if (MainActor.IsFacingRight)
                    TargetPosition = new Vector2(MainActor.Position.X + 70, MainActor.Position.Y - 15);
                else
                    TargetPosition = new Vector2(MainActor.Position.X - 70, MainActor.Position.Y - 15);

                // Set horizontal speed
                if (Position.X > TargetPosition.X + 20)
                {
                    if (ActionId != 17)
                        ActionId = 17;

                    if (SavedSpeed.X > 2.5)
                        SavedSpeed -= new Vector2(0.5f, 0);
                    else
                        SavedSpeed = new Vector2(-2.5f, SavedSpeed.Y);
                }
                else if (Position.X < TargetPosition.X - 20)
                {
                    if (ActionId != 16)
                        ActionId = 16;

                    if (SavedSpeed.X < 2.5)
                        SavedSpeed += new Vector2(0.5f, 0);
                    else
                        SavedSpeed = new Vector2(2.5f, SavedSpeed.Y);
                }
                else
                {
                    if (SavedSpeed.X > 0.5)
                    {
                        SavedSpeed -= new Vector2(0.5f, 0);
                    }
                    else if (SavedSpeed.X < -0.5)
                    {
                        SavedSpeed += new Vector2(0.5f, 0);
                    }
                    else
                    {
                        SavedSpeed = new Vector2(0, SavedSpeed.Y);

                        if (ActionId is 16 or 17)
                        {
                            if (Position.X > MainActor.Position.X && ActionId != 17)
                                ActionId = 17;
                            else if (Position.X < MainActor.Position.X && ActionId != 16)
                                ActionId = 16;
                        }
                    }
                }

                // Set vertical speed
                if (Position.Y < TargetPosition.Y - 5)
                {
                    if (SavedSpeed.Y < 2)
                        SavedSpeed += new Vector2(0, 0.05859375f);
                    else
                        SavedSpeed = new Vector2(SavedSpeed.X, 2);
                }
                else if (Position.Y > TargetPosition.Y + 5)
                {
                    if (SavedSpeed.Y <= -2)
                        SavedSpeed = new Vector2(SavedSpeed.X, -2);
                    else
                        SavedSpeed -= new Vector2(0, 0.05859375f);
                }
                else
                {
                    SavedSpeed = new Vector2(SavedSpeed.X, 0);
                }

                // Set speed
                MechModel.Speed = SavedSpeed;

                // If stopped moving
                if (SavedSpeed == Vector2.Zero && ActionId is not (0 or 1))
                    Fsm.ChangeAction(Fsm_Talk);
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Talk(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = IsFacingRight ? 1 : 0;
                Byte_8B = ActionId;
                TextBox.MoveInOurOut(true);
                Timer = 0;
                break;

            case FsmAction.Step:
                // TODO: Implement
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}