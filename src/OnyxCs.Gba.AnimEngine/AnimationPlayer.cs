using System.Collections.Generic;

namespace OnyxCs.Gba.AnimEngine;

public class AnimationPlayer
{
    public AnimationPlayer(bool is8Bit)
    {
        Is8Bit = is8Bit;

        AnimationSpriteManager = new AnimationSpriteManager();
        AnimatedObjects1 = new Stack<AObject>();
        AnimatedObjects2 = new Stack<AObject>();

        // TODO: Might not need this if every frame clears GFX on init
        Gfx.Sprites.Clear();
    }

    private AnimationSpriteManager AnimationSpriteManager { get; }

    // TODO: What's the different between these two?
    private Stack<AObject> AnimatedObjects1 { get; }
    private Stack<AObject> AnimatedObjects2 { get; }

    public bool Is8Bit { get; }

    public void AddObject1(AObject obj)
    {
        AnimatedObjects1.Push(obj);
    }

    public void Execute()
    {
        Gfx.Sprites.Clear();

        foreach (AObject obj in AnimatedObjects1)
            obj.Execute(AnimationSpriteManager, SoundEventRequest);

        foreach (AObject obj in AnimatedObjects2)
            obj.Execute(AnimationSpriteManager, SoundEventRequest);

        AnimatedObjects1.Clear();
        AnimatedObjects2.Clear();
    }

    public void SoundEventRequest(int soundId)
    {
        SoundManager.Play(soundId);
    }
}