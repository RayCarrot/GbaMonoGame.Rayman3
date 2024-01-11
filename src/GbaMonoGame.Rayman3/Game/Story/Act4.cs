namespace GbaMonoGame.Rayman3;

public class Act4 : Act
{
    public override void Init()
    {
        Init(Engine.Loader.Rayman3_Act4);
    }

    public override void Step()
    {
        base.Step();

        if (IsFinished)
        {
            // TODO: Load worldmap frame
        }
    }
}