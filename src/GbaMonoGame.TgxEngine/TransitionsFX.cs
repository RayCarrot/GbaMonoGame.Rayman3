namespace GbaMonoGame.TgxEngine;

public class TransitionsFX
{
    public TransitionsFX(bool clear)
    {
        if (clear)
            Gfx.Fade = 0;
    }

    public float FadeCoefficient { get; set; }
    public float BrightnessCoefficient { get; set; } = 1;

    public bool IsChangingBrightness => BrightnessCoefficient < 1;
    public bool IsFading => FadeCoefficient > 0;

    public float StepSize { get; set; }

    public void StepAll()
    {
        // The game only runs this in 15 fps (every 4 frames), but we want to do it every frame
        float stepSize = StepSize / 4;

        if (BrightnessCoefficient < 1)
        {
            BrightnessCoefficient += stepSize;

            if (FadeCoefficient > 1)
                FadeCoefficient = 1;

            Gfx.Fade = BrightnessCoefficient;
        }
        else if (FadeCoefficient == 0)
        {
            // TODO: Implement
            Logger.NotImplemented("Not implemented transition when fade coefficient is 0");
        }
        else
        {
            FadeCoefficient -= stepSize;

            if (FadeCoefficient < 0)
                FadeCoefficient = 0;

            Gfx.Fade = FadeCoefficient;
        }
    }

    public void StepFade()
    {
        // The game only runs this in 30 fps (every 2 frames), but we want to do it every frame
        float stepSize = StepSize / 2;

        if (IsFading)
        {
            FadeCoefficient -= stepSize;

            if (FadeCoefficient <= 0)
                FadeCoefficient = 0;

            Gfx.Fade = FadeCoefficient;
        }
    }

    public void StepBrightness()
    {
        // The game only runs this in 30 fps (every 2 frames), but we want to do it every frame
        float stepSize = StepSize / 2;

        if (IsChangingBrightness)
        {
            BrightnessCoefficient += stepSize;

            if (BrightnessCoefficient >= 1)
                BrightnessCoefficient = 1;

            Gfx.Fade = BrightnessCoefficient;
        }
    }

    public void FadeInInit(float stepSize)
    {
        Gfx.Fade = 1;
        FadeCoefficient = 1;
        StepSize = stepSize;
    }

    public void FadeOutInit(float stepSize)
    {
        Gfx.Fade = 0;
        BrightnessCoefficient = 0;
        StepSize = stepSize;
    }
}