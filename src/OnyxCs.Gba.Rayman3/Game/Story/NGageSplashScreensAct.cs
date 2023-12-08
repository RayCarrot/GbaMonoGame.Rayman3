namespace OnyxCs.Gba.Rayman3;

public class NGageSplashScreensAct : Act
{
    public NGageSplashScreensAct()
    {
        IsAutomatic = true;
    }

    public override void Init()
    {
        Init(Engine.Loader.Rayman3_NGageSplashScreens);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
            FrameManager.SetNextFrame(new Intro());
    }
}