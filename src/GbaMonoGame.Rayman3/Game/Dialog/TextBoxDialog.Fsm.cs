using GbaMonoGame.AnimEngine;

namespace GbaMonoGame.Rayman3;

public partial class TextBoxDialog
{
    private bool Fsm_MoveIn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                TextTransitionValue = 1;
                Timer = 0;
                foreach (SpriteTextObject textObj in TextObjects)
                    textObj.AffineMatrix = AffineMatrix.Identity;
                break;

            case FsmAction.Step:
                OffsetY -= 3;
                if (OffsetY <= 0)
                {
                    OffsetY = 0;
                    State.MoveTo(Fsm_WaitForNextText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_WaitForNextText(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Do nothing
                break;

            case FsmAction.Step:
                if (NextText)
                {
                    NextText = false;
                    CurrentTextLine += TextObjects.Length;
                    State.MoveTo(Fsm_TransitionTextOut);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_TransitionTextOut(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                TextTransitionValue = 1;
                foreach (SpriteTextObject textObj in TextObjects)
                    textObj.AffineMatrix = AffineMatrix.Identity;
                break;

            case FsmAction.Step:
                bool transitionIn = false;

                if (!IsFinished)
                {
                    TextTransitionValue++;
                    foreach (SpriteTextObject textObj in TextObjects)
                        textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);

                    if (TextTransitionValue > 8)
                    {
                        transitionIn = true;

                        if (CurrentTextLine >= CurrentText.Length)
                        {
                            CurrentTextLine = 0;
                            ShouldPlayedLySound = true;
                            ShouldPlayRaymanSound = true;
                            ShouldPlayedMurfySound = true;
                            IsFinished = true;
                        }
                    }
                }

                if (transitionIn)
                {
                    State.MoveTo(Fsm_TransitionTextIn);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_TransitionTextIn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                Timer = 6;
                break;

            case FsmAction.Step:
                bool finished = false;

                if (Timer != 0)
                {
                    if (TextObjects.Length == 3)
                    {
                        if (Timer == 6)
                            UpdateText(0);
                        if (Timer == 4)
                            UpdateText(1);
                        if (Timer == 2)
                            UpdateText(2);
                    }
                    else
                    {
                        if (Timer == 4)
                            UpdateText(0);
                        if (Timer == 2)
                            UpdateText(1);
                    }

                    Timer--;
                }
                else
                {
                    TextTransitionValue--;

                    if (TextTransitionValue < 1)
                    {
                        TextTransitionValue = 1;
                        finished = true;
                    }

                    foreach (SpriteTextObject textObj in TextObjects)
                        textObj.AffineMatrix = new AffineMatrix(1, 0, 0, TextTransitionValue);
                }

                if (finished)
                {
                    State.MoveTo(Fsm_WaitForNextText);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_MoveOut(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                CurrentTextLine = 0;
                break;

            case FsmAction.Step:
                OffsetY += 3;
                if (OffsetY >= 45)
                {
                    OffsetY = 45;
                    State.MoveTo(null);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}