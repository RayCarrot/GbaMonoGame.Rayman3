using BinarySerializer.Ubisoft.GbaEngine;
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
                    if ((JoyPad.IsButtonJustPressed(GbaInput.Up) || JoyPad.IsButtonJustPressed(GbaInput.A)) && 
                        JoyPad.IsButtonReleased(GbaInput.Left) &&
                        JoyPad.IsButtonReleased(GbaInput.Right) &&
                        !((World)Frame.Current).UserInfo.Hide)
                    {
                        MovableActor mainActor = Scene.MainActor;

                        if (mainActor.Speed.Y == 0)
                        {
                            if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__Tag_Mix02))
                                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Tag_Mix02);

                            mainActor.ProcessMessage(this, Message.Main_LockedLevelCurtain);
                        }
                    }
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_Unlocked(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (Scene.IsDetectedMainActor(this) && Scene.MainActor.Speed.Y == 0)
                {
                    ((World)Frame.Current).UserInfo.SetLevelInfoBar(InitialActionId);
                    Scene.MainActor.ProcessMessage(this, Message.Main_BeginInFrontOfLevelCurtain);

                    if ((JoyPad.IsButtonPressed(GbaInput.Up) || JoyPad.IsButtonPressed(GbaInput.A)) &&
                        JoyPad.IsButtonReleased(GbaInput.Left) &&
                        JoyPad.IsButtonReleased(GbaInput.Right) &&
                        !((World)Frame.Current).UserInfo.Hide)
                    {
                        Scene.MainActor.ProcessMessage(this, Message.Main_Stop);
                        State.MoveTo(Fsm_EnterCurtain);
                    }
                    else
                    {
                        if (ActionId != 33 && !((Rayman)Scene.MainActor).IsInDefaultState)
                            ActionId = 33;
                        else if (IsActionFinished)
                            ActionId = InitialActionId;
                    }
                }
                else
                {
                    // TODO: This solution won't work if camera scale is too high and multiple level curtains are on screen at once!
                    //       Perhaps we should rewrite this so it keeps track of when Rayman enters and leaves the detection zone?

                    // If set to keep all objects active we only want to do this if framed. Otherwise this will overwrite
                    // if another level curtain is on screen and Rayman is in front of that one.
                    if (!Scene.KeepAllObjectsActive || AnimatedObject.IsFramed)
                        Scene.MainActor.ProcessMessage(this, Message.Main_EndInFrontOfLevelCurtain);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_EnterCurtain(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (ActionId != 33)
                {
                    ActionId = 31;
                    Scene.MainActor.ProcessMessage(this, Message.Main_EnterLevelCurtain);
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Curtain_YoyoMove_Mix02);
                }
                break;

            case FsmAction.Step:
                if (IsActionFinished)
                {
                    if (ActionId == 33)
                    {
                        ActionId = 31;
                        Scene.MainActor.ProcessMessage(this, Message.Main_EnterLevelCurtain);
                        SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__Curtain_YoyoMove_Mix02);
                    }
                    else if (ActionId == 31)
                    {
                        AnimatedObject.YPriority = 0;
                        ActionId = 32;
                    }
                    else
                    {
                        State.MoveTo(Fsm_TransitionToLevel);
                    }
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_TransitionToLevel(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                ((World)Frame.Current).InitExiting();
                ActionId = InitialActionId;
                break;

            case FsmAction.Step:
                if (!((World)Frame.Current).IsTransitioningOut())
                    State.MoveTo(Fsm_Unlocked);
                break;

            case FsmAction.UnInit:
                Gfx.SetFullFade();
                SoundEventsManager.StopAllSongs();
                GameInfo.LoadLevel(MapId);
                break;
        }
    }
}