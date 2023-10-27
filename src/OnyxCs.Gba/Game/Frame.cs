using System;
using System.Collections.Generic;

namespace OnyxCs.Gba;

public abstract class Frame
{
    public static Frame CurrentFrame => FrameManager.CurrentFrame;

    protected void RegisterComponent(object component) => Components.Add(component);

    public static T GetComponent<T>()
        where T : class
    {
        if (CurrentFrame == null)
            return null;

        foreach (object component in CurrentFrame.Components)
        {
            if (component is T c)
                return c;
        }

        return null;
    }

    // This is not a thing in the original engine. Rather everything is static.
    private HashSet<object> Components { get; } = new();

    public bool EndOfFrame { get; set; }

    public virtual void Init() { }
    public virtual void UnInit() { }
    public abstract void Step();
}