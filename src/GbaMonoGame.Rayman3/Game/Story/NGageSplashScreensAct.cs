namespace GbaMonoGame.Rayman3;

public class NGageSplashScreensAct : Act
{
    private int Timer { get; set; }
    private int Screen { get; set; }

    private void NextScreen()
    {
        Timer = 0;
        Screen++;
        NextFrame(false);
    }

    public override void Init()
    {
        // TODO: Load settings from save file (volume and language)
        Init(Engine.Loader.Rayman3_NGageSplashScreens);
        IsAutomatic = true;
    }

    public override void Step()
    {
        bool next = false;

        switch (Screen)
        {
            case 0:
                Timer++;
                if (Timer > 120)
                    next = true;
                break;

            case 1:
                Timer++;
                if (Timer > 90)
                    next = true;
                break;

            case 2:
                Timer++;
                if (Timer > 60)
                    next = true;
                break;
        }

        if (next)
            NextScreen();

        base.Step();

        if (IsFinished)
            FrameManager.SetNextFrame(new Intro());
    }
}