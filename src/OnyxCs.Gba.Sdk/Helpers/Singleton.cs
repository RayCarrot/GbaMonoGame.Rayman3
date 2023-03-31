namespace OnyxCs.Gba.Sdk;

// TODO: Eventually get rid of singletons
public class Singleton<T>
    where T : new()
{
    public static T Instance { get; } = new();
}