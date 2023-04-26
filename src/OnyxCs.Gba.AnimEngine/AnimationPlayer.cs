using System;
using System.Collections.Generic;
using OnyxCs.Gba.Sdk;

namespace OnyxCs.Gba.AnimEngine;

public class AnimationPlayer
{
    public AnimationPlayer(Vram vram, bool is8Bit, Action<int>? soundEventCallback)
    {
        Vram = vram;
        Is8Bit = is8Bit;
        SoundEventCallback = soundEventCallback;

        AnimatedObjects1 = new Stack<AObject>();
        AnimatedObjects2 = new Stack<AObject>();

        Vram.ClearSprites();
        Vram.ClearSpritePalettes();
    }

    private Vram Vram { get; }
    private Action<int>? SoundEventCallback { get; }

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
        Vram.ClearSprites();

        foreach (AObject obj in AnimatedObjects1)
            obj.Execute(Vram, SoundEventRequest);

        foreach (AObject obj in AnimatedObjects2)
            obj.Execute(Vram, SoundEventRequest);

        AnimatedObjects1.Clear();
        AnimatedObjects2.Clear();
    }

    public void SoundEventRequest(int soundId)
    {
        SoundEventCallback?.Invoke(soundId);
    }
}