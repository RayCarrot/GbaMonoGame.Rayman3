using System;
using BinarySerializer;
using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba.Sdk;

// TODO: Handle caching
public class Storage
{
    public Storage(Context context)
    {
        Context = context;
        Settings = context.GetRequiredSettings<OnyxGbaSettings>();
    }

    private static Storage? _instance;
    public static Storage Instance => _instance ?? throw new InvalidOperationException("Rom has not been loaded");

    public Context Context { get; }
    public OnyxGbaSettings Settings { get; }

    public static void Load(Context context)
    {
        _instance = new Storage(context);
    }

    public static T ReadResource<T>(int index)
        where T : Resource, new()
    {
        using Context context = Instance.Context;
        OffsetTable rootTable = Instance.Settings.RootTable;
        return FileFactory.Read<T>(context, rootTable.GetPointer(context, index));
    }
}
