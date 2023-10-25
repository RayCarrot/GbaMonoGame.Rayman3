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
        SetActionId(actorResource.FirstActionId);
        ChangeAction();

        DetectionBox = actorResource.Model.DetectionBox.ToRectangle();
    }

    public Action[] Actions { get; }
    public Rectangle DetectionBox { get; }

    public int HitPoints { get; set; }
    public int AttackPoints { get; set; }

    public Rectangle ActionBox { get; set; }

    public int ActionId { get; private set; }
    public bool NewAction { get; set; }

    public override void Step()
    {
        ChangeAction();
    }

    public void SetActionId(int actionId)
    {
        ActionId = actionId;
        NewAction = true;
    }

    public bool IsActionFinished()
    {
        return AnimatedObject.EndOfAnimation;
    }

    public void ChangeAction()
    {
        if (NewAction)
        {
            Action action = Actions[ActionId];

            ActionBox = action.Box.ToRectangle();

            AnimatedObject.SetCurrentAnimation(action.AnimationIndex);
            AnimatedObject.FlipX = (action.Flags & ActionFlags.FlipX) != 0;
            AnimatedObject.FlipY = (action.Flags & ActionFlags.FlipY) != 0;

            if (action.Type != null)
            {
                // TODO: Load movement data
            }

            NewAction = false;
        }
    }
}