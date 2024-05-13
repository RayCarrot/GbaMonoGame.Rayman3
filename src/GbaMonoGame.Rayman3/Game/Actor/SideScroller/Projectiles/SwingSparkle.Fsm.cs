using System;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class SwingSparkle
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                if (AnimatedObject.CurrentAnimation != 1)
                    AnimatedObject.CurrentFrame = Random.GetNumber(7);
                break;

            case FsmAction.Step:
                Rayman rayman = (Rayman)Scene.MainActor;
                if (rayman.AttachedObject != null)
                {
                    float xPos = rayman.AttachedObject.Position.X + MathHelpers.Cos256(rayman.Timer) * Value;
                    float yPos = rayman.AttachedObject.Position.Y + MathHelpers.Sin256(rayman.Timer) * Value;
                    Position = new Vector2(xPos, yPos);
                }

                bool finished = rayman.AttachedObject == null || 
                                (AnimatedObject.CurrentAnimation != 1 && 
                                 Value > rayman.PreviousXSpeed - 32 && 
                                 rayman.PreviousXSpeed >= 80);

                if (AnimatedObject.CurrentAnimation == 1)
                    Value = rayman.PreviousXSpeed - 30;

                if (finished)
                    Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                ScreenPosition = new Vector2(0, ScreenPosition.Y);
                AnimatedObject.CurrentAnimation = 0;
                ProcessMessage(Message.Destroy);
                break;
        }
    }
}