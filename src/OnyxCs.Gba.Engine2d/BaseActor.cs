using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework;
using OnyxCs.Gba.AnimEngine;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Engine2d;

public class BaseActor : GameObject
{
    // NOTE: The game allows actors to pass in "user-defined" AObject classes. However the game handles this in a rather
    //       ugly way where it will by default assume it's of type AnimatedObject, so the class then has to override all
    //       of this behavior. We will however try and only have a single AnimatedObject in here.
    public BaseActor(int id, ActorResource actorResource) : base(id, actorResource)
    {
        ActorModel = actorResource.Model;
        ActorType = actorResource.Id;

        AnimatedObject = new AnimatedObject(actorResource.Model.AnimatedObject, actorResource.IsAnimatedObjectDynamic);
        AnimatedObject.SetCurrentAnimation(0);

        ViewBox = ActorModel.ViewBox.ToRectangle();
    }

    public ActorModel ActorModel { get; }
    public int ActorType { get; }
    public AnimatedObject AnimatedObject { get; }
    public Rectangle ViewBox { get; }

    public Fsm Fsm { get; set; }

    public virtual void Init() { }

    public virtual void DoBehavior()
    {
        //Fsm();
    }

    public virtual void Step() { }

    public virtual void Draw(AnimationPlayer animationPlayer, bool forceDraw)
    {
        // TODO: Properly implement this
        // 1. Get screen position
        // 2. Check with camera actor if it is framed
        // 3. Set IsFramed in AnimatedObject based on that

        AnimatedObject.ScreenPos = Position - Frame.GetComponent<TgxPlayfield2D>().Camera.Position;

        bool isFramed = true;

        if (isFramed)
        {
            animationPlayer.AddSecondaryObject(AnimatedObject);
        }
        else
        {
            // Execute animation without drawing?
        }
    }
}