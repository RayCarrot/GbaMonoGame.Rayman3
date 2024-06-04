using System;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class ChainedSparkles
{
    private void FUN_08060930(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                AnimatedObject.SetCurrentPriority(false);
                SwirlValue = 0;
                break;

            case FsmAction.Step:
                throw new NotImplementedException();
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void Fsm_NewPower(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                SwirlValue = 64;
                Timer = 0;
                break;

            case FsmAction.Step:
                if (Timer < 94)
                {
                    // Create the illusion of the sparkles flying around Rayman in 3D by changing the priority
                    AnimatedObject.SetCurrentPriority(SwirlValue < 128);

                    float x;
                    if (AreSparklesFacingLeft)
                        x = MainActor2.Position.X - MathHelpers.Sin256(SwirlValue) * 26;
                    else
                        x = MainActor2.Position.X + MathHelpers.Sin256(SwirlValue) * 26;

                    Position = new Vector2(x, MainActor2.Position.Y - Timer / 2f);

                    // This wraps at 256 because it's a byte
                    SwirlValue += 6;
                }

                Timer++;

                if (Timer == 40 && SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__OnoWin_Mix02__or__OnoWinRM_Mix02))
                    SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__OnoWin_Mix02__or__OnoWinRM_Mix02);

                if (Timer > 125)
                {
                    ProcessMessage(this, Message.Destroy);
                    Fsm.ChangeAction(Fsm_NewPower);
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }
    }

    private void FUN_08060f58(FsmAction action)
    {
        throw new NotImplementedException();

        switch (action)
        {
            case FsmAction.Init:

                break;

            case FsmAction.Step:

                break;

            case FsmAction.UnInit:

                break;
        }
    }
}