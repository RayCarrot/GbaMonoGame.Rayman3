using BinarySerializer.Ubisoft.GbaEngine.Rayman3;

namespace GbaMonoGame.Rayman3;

public partial class Keg
{
    private void Fsm_WaitingToFall(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                InitialPos = Position;
                ActionId = Action.WaitToFall;
                ShouldDraw = false;
                SpawnedDebrisCount = 0;
                Timer = 0;
                break;

            case FsmAction.Step:
                Timer++;

                Box actionBox = GetActionBox();
                actionBox = new Box(actionBox.MinX, actionBox.MinY, actionBox.MaxX, actionBox.MaxY + 100);

                // Spawn debris
                if (Timer >= 30 && SpawnedDebrisCount < 2 && Scene.IsDetectedMainActor(actionBox))
                {
                    SpawnedDebrisCount++;
                    SpawnDebris();

                    if (Timer > 90)
                        Timer = 30;
                }

                // Fall
                if (Timer > 90 && Scene.IsDetectedMainActor(actionBox) && SpawnedDebrisCount > 0)
                {
                    ShouldDraw = true;
                    Fsm.ChangeAction(Fsm_Falling);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Falling(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ActionId = Action.Fall;
                Position = InitialPos;
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BarlFall_Mix04);
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BarlFall_Mix04);
                break;

            case FsmAction.Step:
                if (Scene.IsHitMainActor(this) ||
                    Scene.GetPhysicalType(new Vector2(Position.X, GetDetectionBox().MaxY)).IsSolid)
                {
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BarlFall_Mix04);

                    if (Scene.IsHitMainActor(this))
                        Scene.MainActor.ReceiveDamage(AttackPoints);

                    Explosion explosion = Scene.CreateProjectile<Explosion>(ActorType.Explosion);

                    if (AnimatedObject.IsFramed)
                    {
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__BangGen1_Mix07);
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__BangGen1_Mix07);
                    }

                    if (explosion != null)
                        explosion.Position = Position - new Vector2(0, 8);

                    Position = InitialPos;
                    
                    Fsm.ChangeAction(Fsm_WaitingToFall);
                }
                break;

            case FsmAction.UnInit:
                Timer = 0;
                break;
        }
    }

    // TODO: Implement
    private void FUN_08063bd4(FsmAction action) { }
}