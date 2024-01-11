using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.Engine2d;

public class ActionActor : BaseActor
{
    public ActionActor(int id, Scene2D scene, ActorResource actorResource) : base(id, scene, actorResource)
    {
        HitPoints = actorResource.Model.HitPoints;
        AttackPoints = actorResource.Model.AttackPoints;
        
        Actions = actorResource.Model.Actions;
        ActionId = actorResource.FirstActionId;
        ChangeAction();

        _detectionBox = new Box(actorResource.Model.DetectionBox);
    }

    private int _actionId;
    private Box _detectionBox;
    private Box _actionBox;

    public Action[] Actions { get; }

    public int HitPoints { get; set; }
    public int AttackPoints { get; set; }

    public sealed override int ActionId
    {
        get => _actionId;
        set
        {
            _actionId = value;
            NewAction = true;
        }
    }

    public bool NewAction { get; set; }

    public virtual Box GetDetectionBox() => _detectionBox.Offset(Position);
    public virtual Box SetDetectionBox(Box detectionBox) => _detectionBox = detectionBox;
    public virtual Box GetActionBox() => _actionBox.Offset(Position);

    protected override bool ProcessMessageImpl(Message message, object param)
    {
        switch (message)
        {
            case Message.Resurrect:
            case Message.ResurrectWakeUp:
                HitPoints = ActorModel.HitPoints;
                break;
        }

        return base.ProcessMessageImpl(message, param);
    }

    public override void Step()
    {
        ChangeAction();
    }

    public void ChangeAction()
    {
        if (!NewAction) 
            return;
        
        Action action = Actions[ActionId];

        _actionBox = new Box(action.Box);

        AnimatedObject.CurrentAnimation = action.AnimationIndex;
        AnimatedObject.FlipX = (action.Flags & ActionFlags.FlipX) != 0;
        AnimatedObject.FlipY = (action.Flags & ActionFlags.FlipY) != 0;

        if (action.MechModelType != null && this is MovableActor movableActor)
        {
            float[] mechParams = new float[action.MechModel.Params.Length];
            for (int i = 0; i < mechParams.Length; i++)
                mechParams[i] = action.MechModel.Params[i];
            movableActor.MechModel.Init(action.MechModelType.Value, mechParams);
        }

        NewAction = false;
    }

    public void ReceiveDamage(int damage)
    {
        if (damage < HitPoints)
            HitPoints -= damage;
        else
            HitPoints = 0;
    }
}