using BinarySerializer;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame;

/// <summary>
/// Manages loading engine resources.
/// </summary>
public static class Storage
{
    /// <summary>
    /// Loads a resource from the given index. If the context has caching enabled then the resource
    /// will be cached after loading, resulting in future loads returning the same data.
    /// </summary>
    /// <typeparam name="T">The resource type</typeparam>
    /// <param name="index">The resource index</param>
    /// <returns>The loaded resource</returns>
    public static T LoadResource<T>(int index)
        where T : Resource, new()
    {
        using Context context = Engine.Context;
        return Engine.Settings.RootTable.ReadResource<T>(Engine.Context, index, name: $"Resource_{index}");
    }

    public static T LoadResource<T>(GameResource gameResource)
        where T : Resource, new()
    {
        using Context context = Engine.Context;
        return Engine.Settings.RootTable.ReadResource<T>(Engine.Context, gameResource, name: gameResource.ToString());
    }
}