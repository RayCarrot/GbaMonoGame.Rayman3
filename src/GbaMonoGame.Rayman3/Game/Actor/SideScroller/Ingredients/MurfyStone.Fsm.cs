using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class MurfyStone
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                HasTriggered = false;
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(GetViewBox()) && MurfyId != null)
                {
                    if (!HasTriggered)
                    {
                        if (Timer > 180)
                            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Whistle1_Mix01);
                        
                        Timer = 0;

                        if (Scene.MainActor is Rayman { IsInDefaultState: true })
                            RaymanIdleTimer++;
                        else
                            RaymanIdleTimer = 0;

                        if (RaymanIdleTimer > 30 ||
                            (GameInfo.MapId == MapId.WoodLight_M1 &&
                             GameInfo.LastGreenLumAlive == 0 &&
                             GameInfo.PersistentInfo.LastCompletedLevel == (int)MapId.WoodLight_M1))
                        {
                            HasTriggered = true;
                            GameObject murfy = Scene.GetGameObject(MurfyId.Value);

                            // TODO: This is probably a typo in the original code and explains why Murfy takes a while to appear. Fix?
                            // Why is the horizontal resolution used for the y position??
                            murfy.Position = new Vector2(murfy.Position.X, Position.Y - Scene.Resolution.X);
                            murfy.ProcessMessage(this, Message.Murfy_Spawn);
                        }
                    }
                }
                else
                {
                    if (HasTriggered && Scene.MainActor is not Rayman { IsInDefaultState: true })
                    {
                        HasTriggered = false;
                        RaymanIdleTimer = 0;
                    }

                    Timer++;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }
}