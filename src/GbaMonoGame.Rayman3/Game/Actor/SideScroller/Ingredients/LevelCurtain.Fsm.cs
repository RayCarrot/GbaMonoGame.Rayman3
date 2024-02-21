using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class LevelCurtain
{
    private void Fsm_Locked(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this))
                {
                    if ((JoyPad.CheckSingle(GbaInput.Up) || JoyPad.CheckSingle(GbaInput.A)) &&
                        !JoyPad.Check(GbaInput.Left) &&
                        !JoyPad.Check(GbaInput.Right) &&
                        !((World)Frame.Current).UserInfo.Hide)
                    {
                        MovableActor mainActor = Scene.MainActor;

                        if (mainActor.Speed.Y == 0)
                        {
                            if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__Tag_Mix02))
                                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Tag_Mix02);

                            mainActor.ProcessMessage(Message.Main_LockedLevelCurtain);
                        }
                    }
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Open(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
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