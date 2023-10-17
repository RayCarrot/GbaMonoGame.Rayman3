using BinarySerializer;
using BinarySerializer.Onyx.Gba;

namespace OnyxCs.Gba;

// TODO: Handle caching
public static class Storage
{
    private static Context Context { get; set; }
    private static OnyxGbaSettings Settings { get; set; }

    public static void SetContext(Context context)
    {
        Context = context;
        Settings = context.GetRequiredSettings<OnyxGbaSettings>();
    }

    public static T LoadResource<T>(int index)
        where T : Resource, new()
    {
        using Context context = Context;
        OffsetTable rootTable = Settings.RootTable;
        return FileFactory.Read<T>(context, rootTable.GetPointer(context, index));
    }
}