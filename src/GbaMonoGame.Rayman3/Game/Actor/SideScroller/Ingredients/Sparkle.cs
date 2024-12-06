using GbaMonoGame.AnimEngine;
using GbaMonoGame.Engine2d;

namespace GbaMonoGame.Rayman3;

public sealed class Sparkle : BaseActor
{
    public Sparkle(int instanceId, Scene2D scene, ActorResource actorResource) : base(instanceId, scene, actorResource)
    {
        // NOTE: The game doesn't initialize this, so it'll get set to the default value of 0xCD
        Countdown = 0xCD;

        AnimatedObject.CurrentAnimation = actorResource.FirstActionId;
        State.SetTo(null);
    }

    public int Countdown { get; set; }

    public override void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        bool isFramed = Scene.Camera.IsActorFramed(this) || forceDraw;
        bool show;

        Countdown--;

        if (Countdown > 0)
        {
            show = false;
        }
        else
        {
            show = true;

            if (Countdown == 0)
                AnimatedObject.Rewind();
            else if (AnimatedObject.EndOfAnimation)
                Countdown = Random.GetNumber(120) + 240;
        }

        AnimatedObject.IsFramed = isFramed;
        
        if (isFramed && show)
            animationPlayer.Play(AnimatedObject);
        else
            AnimatedObject.ComputeNextFrame();
    }
}