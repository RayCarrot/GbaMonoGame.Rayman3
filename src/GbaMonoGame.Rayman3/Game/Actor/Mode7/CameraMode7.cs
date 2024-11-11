using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public class CameraMode7 : CameraActorMode7
{
    public CameraMode7(Scene2D scene) : base(scene)
    {
        MainActorDistance = 55;
    }

    public float MainActorDistance { get; set; }

    protected override bool ProcessMessageImpl(object sender, Message message, object param)
    {
        // TODO: Implement
        return base.ProcessMessageImpl(sender, message, param);
    }

    public override void SetFirstPosition()
    {
        // TODO: Implement
    }
}