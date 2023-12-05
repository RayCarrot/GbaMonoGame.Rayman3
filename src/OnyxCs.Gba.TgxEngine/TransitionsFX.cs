namespace OnyxCs.Gba.TgxEngine;

public class TransitionsFX
{
    public float FadeCoefficient { get; set; }
    public float BrightnessCoefficient { get; set; } = 1;
    public float StepSize { get; set; }

    public void Step()
    {
        // The game only runs this in 15 fps (every 4 frames), but we want to do it every frame
        float stepSize = StepSize / 4;

        if (BrightnessCoefficient < 1)
        {
            // TODO: Implement
        }
        else if (FadeCoefficient == 0)
        {
            // TODO: Implement
        }
        else
        {
            FadeCoefficient -= stepSize;

            if (FadeCoefficient <= 0)
                FadeCoefficient = 0;

            Gfx.Fade = FadeCoefficient;
        }
    }

    public void FadeInInit(float stepSize)
    {
        Gfx.Fade = 1;
        FadeCoefficient = 1;
        StepSize = stepSize;
    }
}