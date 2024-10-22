using System;

namespace GbaMonoGame.Rayman3;

public partial class Caterpillar
{
    private bool Fsm_Move(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                SineWaveValue = 0;

                MechModel.Speed = Direction switch
                {
                    MoveDirection.Up => new Vector2(0, -1),
                    MoveDirection.Down => new Vector2(0, 1),
                    MoveDirection.Left => new Vector2(-1, 0),
                    MoveDirection.Right => new Vector2(1, 0),
                    _ => throw new Exception("Invalid direction")
                };
                break;

            case FsmAction.Step:
                if (Scene.IsHitMainActor(this))
                    Scene.MainActor.ReceiveDamage(AttackPoints);

                SineWaveValue += 4;
                CheckMoveBounds();

                // Sine wave movement
                Position = Direction switch
                {
                    MoveDirection.Up or MoveDirection.Down => Position with { X = LastPosition.X + MathHelpers.Sin256(SineWaveValue) * 16 },
                    MoveDirection.Left or MoveDirection.Right => Position with { Y = LastPosition.Y + MathHelpers.Sin256(SineWaveValue) * 16 },
                    _ => throw new Exception("Invalid direction")
                };

                if (HitPoints == 0)
                {
                    // TODO: Implement
                }

                if (Scene.IsDetectedMainActor(this))
                {
                    State.MoveTo(Fsm_Attack);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_Attack(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                switch (Direction)
                {
                    case MoveDirection.Up:
                        LastPosition = LastPosition with { Y = Position.Y };
                        TargetPosition = Scene.MainActor.Position.Y;
                        MechModel.Speed = MechModel.Speed with { Y = -3 };
                        break;

                    case MoveDirection.Down:
                        LastPosition = LastPosition with { Y = Position.Y };
                        TargetPosition = Scene.MainActor.Position.Y;
                        MechModel.Speed = MechModel.Speed with { Y = 3 };
                        break;

                    case MoveDirection.Left:
                        LastPosition = LastPosition with { X = Position.X };
                        TargetPosition = Scene.MainActor.Position.X;
                        MechModel.Speed = MechModel.Speed with { X = -3 };
                        break;

                    case MoveDirection.Right:
                        LastPosition = LastPosition with { X = Position.X };
                        TargetPosition = Scene.MainActor.Position.X;
                        MechModel.Speed = MechModel.Speed with { X = 3 };
                        break;

                    default:
                        throw new Exception("Invalid direction");
                }
                break;

            case FsmAction.Step:
                if (Scene.IsHitMainActor(this))
                    Scene.MainActor.ReceiveDamage(AttackPoints);

                bool reachedTarget = Direction switch
                {
                    MoveDirection.Up => Position.Y <= TargetPosition,
                    MoveDirection.Down => Position.Y >= TargetPosition,
                    MoveDirection.Left => Position.X <= TargetPosition,
                    MoveDirection.Right => Position.X >= TargetPosition,
                    _ => throw new Exception("Invalid direction")
                };

                if (HitPoints == 0)
                {
                    // TODO: Implement
                }

                if (reachedTarget)
                {
                    State.MoveTo(Fsm_AttackReturn);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }

    private bool Fsm_AttackReturn(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // Reverse the direction
                MechModel.Speed = Direction switch
                {
                    MoveDirection.Up => MechModel.Speed with { Y = 3 },
                    MoveDirection.Down => MechModel.Speed with { Y = -3 },
                    MoveDirection.Left => MechModel.Speed with { X = 3 },
                    MoveDirection.Right => MechModel.Speed with { X = -3 },
                    _ => throw new Exception("Invalid direction")
                };
                break;

            case FsmAction.Step:
                if (Scene.IsHitMainActor(this))
                    Scene.MainActor.ReceiveDamage(AttackPoints);

                bool reachedTarget = Direction switch
                {
                    MoveDirection.Up => Position.Y >= LastPosition.Y,
                    MoveDirection.Down => Position.Y <= LastPosition.Y,
                    MoveDirection.Left => Position.X >= LastPosition.X,
                    MoveDirection.Right => Position.X <= LastPosition.X,
                    _ => throw new Exception("Invalid direction")
                };

                if (HitPoints == 0)
                {
                    // TODO: Implement
                }

                if (reachedTarget)
                {
                    State.MoveTo(Fsm_Move);
                    return false;
                }
                break;

            case FsmAction.UnInit:
                // Do nothing
                break;
        }

        return true;
    }
}