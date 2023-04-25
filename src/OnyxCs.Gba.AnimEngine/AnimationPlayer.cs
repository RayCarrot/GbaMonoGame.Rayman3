using System;
using System.Collections.Generic;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.AnimEngine;

public class AnimationPlayer : Singleton<AnimationPlayer>
{
    public AnimationPlayer()
    {
        AnimatedObjects1 = new Stack<AObject>();
        AnimatedObjects2 = new Stack<AObject>();
    }

    public Action<int>? SoundEventCallback { get; set; }

    // TODO: What's the different between these two?
    public Stack<AObject> AnimatedObjects1 { get; }
    public Stack<AObject> AnimatedObjects2 { get; }

    public bool Is8Bit { get; set; }

    public void Init(bool is8Bit, Action<int> soundEventCallback)
    {
        SoundEventCallback = soundEventCallback;
        AnimatedObjects1.Clear();
        AnimatedObjects2.Clear();
        Is8Bit = is8Bit;
        Engine.Instance.Vram.ClearSprites();
        Engine.Instance.Vram.ClearSpritePalettes();
    }

    public void AddObject1(AObject obj)
    {
        AnimatedObjects1.Push(obj);
    }

    public void Execute(Vram vram)
    {
        vram.ClearSprites();

        foreach (AObject obj in AnimatedObjects1)
            obj.Execute(vram);

        foreach (AObject obj in AnimatedObjects2)
            obj.Execute(vram);

        AnimatedObjects1.Clear();
        AnimatedObjects2.Clear();
    }

    public void SoundEventRequest(int soundId)
    {
        SoundEventCallback?.Invoke(soundId);
    }
}