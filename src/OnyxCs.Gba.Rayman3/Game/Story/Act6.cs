namespace OnyxCs.Gba.Rayman3;

public class Act6 : Act
{
    public override void Init()
    {
        Init(Engine.Loader.Rayman3_Act6);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
        {
            // TODO: Load credits frame
        }
    }
}