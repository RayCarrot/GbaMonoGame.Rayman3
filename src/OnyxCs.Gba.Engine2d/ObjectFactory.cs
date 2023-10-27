using System;
using System.Collections.Generic;
using System.Linq;

namespace OnyxCs.Gba.Engine2d;

public static class ObjectFactory
{
    public delegate BaseActor CreateActor(int id, ActorResource actorResource);

    private static Dictionary<int, CreateActor> ActorCreations { get; set; }

    public static void Init<T>(Dictionary<T, CreateActor> actorCreations)
        where T : Enum
    {
        ActorCreations = actorCreations.ToDictionary(x => (int)(object)x.Key, x => x.Value);
    }

    public static void Init(Dictionary<int, CreateActor> actorCreations)
    {
        ActorCreations = actorCreations;
    }

    public static BaseActor Create(int id, ActorResource actorResource)
    {
        if (!ActorCreations.TryGetValue(actorResource.Id, out CreateActor create))
            return new DummyActor(id, actorResource);

        return create(id, actorResource);
    }
}