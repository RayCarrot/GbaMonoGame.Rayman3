using System.Collections.Generic;

namespace OnyxCs.Gba.AnimEngine;

public class AnimationPlayer
{
    public AnimationPlayer(bool is8Bit)
    {
        Is8Bit = is8Bit;

        AnimationSpriteManager = new AnimationSpriteManager();
        UnsortedObjects = new List<AObject>();
        SortedObjects = new List<AObject>();
    }

    private AnimationSpriteManager AnimationSpriteManager { get; }

    private List<AObject> UnsortedObjects { get; }
    private List<AObject> SortedObjects { get; }

    public bool Is8Bit { get; }

    private void SoundEventRequest(ushort soundId)
    {
        SoundManager.Play(soundId);
    }

    public void AddObject(AObject obj)
    {
        UnsortedObjects.Add(obj);
    }

    public void AddSortedObject(AObject obj)
    {
        for (int i = 0; i < SortedObjects.Count; i++)
        {
            if (SortedObjects[i].Priority >= obj.Priority)
            {
                SortedObjects.Insert(i, obj);
                return;
            }
        }

        SortedObjects.Add(obj);
    }

    public void Execute()
    {
        foreach (AObject obj in UnsortedObjects)
            obj.Execute(AnimationSpriteManager, SoundEventRequest);

        foreach (AObject obj in SortedObjects)
            obj.Execute(AnimationSpriteManager, SoundEventRequest);

        UnsortedObjects.Clear();
        SortedObjects.Clear();
    }
}