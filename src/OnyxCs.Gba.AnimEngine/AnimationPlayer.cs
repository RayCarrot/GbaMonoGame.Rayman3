using System.Collections.Generic;

namespace OnyxCs.Gba.AnimEngine;

public class AnimationPlayer
{
    public AnimationPlayer(bool is8Bit)
    {
        Is8Bit = is8Bit;

        AnimationSpriteManager = new AnimationSpriteManager();
        PrimaryAnimationStack = new Stack<AObject>();
        SecondaryAnimationStack = new Stack<AObject>();
    }

    private AnimationSpriteManager AnimationSpriteManager { get; }

    private Stack<AObject> PrimaryAnimationStack { get; }
    private Stack<AObject> SecondaryAnimationStack { get; }

    public bool Is8Bit { get; }

    private void SoundEventRequest(ushort soundId)
    {
        SoundManager.Play(soundId);
    }

    public void AddPrimaryObject(AObject obj)
    {
        PrimaryAnimationStack.Push(obj);
    }

    public void AddSecondaryObject(AObject obj)
    {
        SecondaryAnimationStack.Push(obj);
    }

    public void Execute()
    {
        foreach (AObject obj in PrimaryAnimationStack)
            obj.Execute(AnimationSpriteManager, SoundEventRequest);

        foreach (AObject obj in SecondaryAnimationStack)
            obj.Execute(AnimationSpriteManager, SoundEventRequest);

        PrimaryAnimationStack.Clear();
        SecondaryAnimationStack.Clear();
    }
}