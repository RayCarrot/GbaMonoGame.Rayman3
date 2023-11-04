using System.Collections.Generic;

namespace OnyxCs.Gba;

/// <summary>
/// A frame is the most important object in this engine, determining how the game loop is handled. Only one frame
/// object can be active at once and is used for the current state of the game, such as a level, menu, cutscene etc.
/// </summary>
public abstract class Frame
{
    /// <summary>
    /// Gets the currently active frame.
    /// </summary>
    public static Frame CurrentFrame => FrameManager.CurrentFrame;

    /// <summary>
    /// Registers a component to the currently active frame. The original game uses global singletons, but
    /// here we store singleton data as scoped components in the current frame.
    /// </summary>
    /// <param name="component"></param>
    public static void RegisterComponent(object component) => CurrentFrame.Components.Add(component);

    /// <summary>
    /// Gets a registered component from the currently active frame. The original game uses global singletons,
    /// but here we store singleton data as scoped components in the current frame.
    /// </summary>
    /// <typeparam name="T">The component type. This can be the instance type or any type it inherits from or implements.</typeparam>
    /// <returns>The component, or null if not found</returns>
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

    private HashSet<object> Components { get; } = new();

    /// <summary>
    /// Indicates if the frame is scheduled to end.
    /// </summary>
    public bool EndOfFrame { get; set; }

    /// <summary>
    /// Initializes the frame. This is called once when the frame is made active.
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// UnInitializes the frame. This is called once when a new frame is made active.
    /// </summary>
    public virtual void UnInit() { }

    /// <summary>
    /// Steps the frame. This is called once every game frame when the frame is active.
    /// </summary>
    public abstract void Step();
}