namespace OnyxCs.Gba.Rayman3;

public class Act3 : Act
{
    public override void Init()
    {
        Init(Engine.Loader.Act3);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
            GameInfo.LoadLevel(GameInfo.GetNextLevelId());
    }
}