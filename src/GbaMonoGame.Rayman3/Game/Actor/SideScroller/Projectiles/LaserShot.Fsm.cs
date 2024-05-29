using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public partial class LaserShot
{
    private void Fsm_Default(FsmAction action)
    {
        switch (action)
        {
            case FsmAction.Init:
                // ???
                AnimatedObject.ScreenPos = new Vector2(0, AnimatedObject.ScreenPos.Y);
                Speed = new Vector2(1.525879E-05f, Speed.Y);
                break;

            case FsmAction.Step:
                bool finished = (AnimatedObject.ScreenPos.X >= Engine.GameViewPort.GameResolution.X && Speed.X > 0) || 
                                (AnimatedObject.ScreenPos.X < 0 && Speed.X < 0) ||
                                Speed.X == 0 ||
                                // TODO: Have these match resolution? In the original game they're the same on both
                                //       GBA and N-Gage. Though they seem to be GBA res with a margin of 10.
                                AnimatedObject.ScreenPos.Y <= -10 || 
                                AnimatedObject.ScreenPos.Y >= 170;

                if (ActionId is Action.Shot1Enemy_Right or Action.Shot1Enemy_Left)
                {
                    InteractableActor hitActor = Scene.IsHitActor(this);
                    if (hitActor != null && hitActor != Scene.MainActor)
                    {
                        hitActor.ReceiveDamage(AttackPoints);
                        hitActor.ProcessMessage(this, Message.Main_Damaged1);
                        Explosion explosion = Scene.CreateProjectile<Explosion>(ActorType.Explosion);
                        hitActor.ProcessMessage(this, Message.Hit);
                        
                        if (explosion != null)
                            explosion.Position = Position;
                    }
                }
                else
                {
                    if (Scene.IsHitMainActor(this) && !Scene.MainActor.IsInvulnerable)
                    {
                        Scene.MainActor.ReceiveDamage(AttackPoints);
                        Explosion explosion = Scene.CreateProjectile<Explosion>(ActorType.Explosion);
                        Scene.MainActor.ProcessMessage(this, Message.Main_Damaged1);
                        
                        if (explosion != null)
                            explosion.Position = Position;
                    }
                }

                if (finished)
                    Fsm.ChangeAction(Fsm_Default);
                break;

            case FsmAction.UnInit:
                // ???
                AnimatedObject.ScreenPos = new Vector2(0, AnimatedObject.ScreenPos.Y);
                Speed = new Vector2(1.525879E-05f, Speed.Y);
                
                ProcessMessage(this, Message.Destroy);
                break;
        }
    }
}