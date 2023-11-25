using System;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba;

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
        OffsetTable rootTable = Engine.Settings.RootTable;
        return FileFactory.Read<T>(context, rootTable.GetPointer(context, index));
    }

    public static T LoadResource<T>(Enum value)
        where T : Resource, new()
    {
        GameResourceDefineAttribute define = value.
            GetAttributes<GameResourceDefineAttribute>().
            FirstOrDefault(x => x.Game == Engine.Settings.Game && x.Platform == Engine.Settings.Platform);

        if (define == null)
            throw new ArgumentException("Enum value has no game resource define for the current game settings", nameof(value));

        using Context context = Engine.Context;
        OffsetTable rootTable = Engine.Settings.RootTable;
        return FileFactory.Read<T>(context, rootTable.GetPointer(context, define.ResourceId));
    }
}