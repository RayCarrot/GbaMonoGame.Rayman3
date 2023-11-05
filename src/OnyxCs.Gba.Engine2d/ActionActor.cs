using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;

namespace OnyxCs.Gba.Engine2d;

public class ActionActor : BaseActor
{
    public ActionActor(int id, ActorResource actorResource) : base(id, actorResource)
    {
        HitPoints = actorResource.Model.HitPoints;
        AttackPoints = actorResource.Model.AttackPoints;
        
        Actions = actorResource.Model.Actions;
        ActionId = actorResource.FirstActionId;
        ChangeAction();

        DetectionBox = actorResource.Model.DetectionBox.ToRectangle();
    }

    private int _actionId;

    public Action[] Actions { get; }
    public Rectangle DetectionBox { get; }

    public int HitPoints { get; set; }
    public int AttackPoints { get; set; }

    public Rectangle ActionBox { get; set; }

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

    protected override bool ProcessMessage(Message message, object param)
    {
        switch (message)
        {
            case Message.Resurrect:
            case Message.ResurrectWakeUp:
                HitPoints = ActorModel.HitPoints;
                break;
        }

        return base.ProcessMessage(message, param);
    }

    public override void Step()
    {
        ChangeAction();
    }

    public void SetActionId(int actionId)
    {
        ActionId = actionId;
    }

    public void ChangeAction()
    {
        if (!NewAction) 
            return;
        
        Action action = Actions[ActionId];

        ActionBox = action.Box.ToRectangle();

        AnimatedObject.SetCurrentAnimation(action.AnimationIndex);
        AnimatedObject.FlipX = (action.Flags & ActionFlags.FlipX) != 0;
        AnimatedObject.FlipY = (action.Flags & ActionFlags.FlipY) != 0;

        if (action.MechModelType != null && this is MovableActor movableActor)
        {
            float[] mechParams = new float[action.MechModel.Params.Length];
            for (int i = 0; i < mechParams.Length; i++)
                mechParams[i] = action.MechModel.Params[i];
            movableActor.Mechanic.Init(action.MechModelType.Value, mechParams);
        }

        NewAction = false;
    }
}