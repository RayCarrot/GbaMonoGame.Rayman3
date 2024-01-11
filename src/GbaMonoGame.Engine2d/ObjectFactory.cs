using System;
using System.Collections.Generic;
using System.Linq;

namespace GbaMonoGame.Engine2d;

public static class ObjectFactory
{
    public delegate BaseActor CreateActor(int id, Scene2D scene, ActorResource actorResource);

    private static Dictionary<int, CreateActor> ActorCreations { get; set; }
    private static Func<int, string> GetActorTypeNameFunc { get; set; }

    public static void Init<T>(Dictionary<T, CreateActor> actorCreations, Func<int, string> getActorTypeNameFunc)
        where T : Enum
    {
        ActorCreations = actorCreations.ToDictionary(x => (int)(object)x.Key, x => x.Value);
        GetActorTypeNameFunc = getActorTypeNameFunc;
    }

    public static void Init(Dictionary<int, CreateActor> actorCreations, Func<int, string> getActorTypeNameFunc)
    {
        ActorCreations = actorCreations;
        GetActorTypeNameFunc = getActorTypeNameFunc;
    }

    public static BaseActor Create(int id, Scene2D scene, ActorResource actorResource)
    {
        if (!ActorCreations.TryGetValue(actorResource.Type, out CreateActor create))
            return new DummyActor(id, scene, actorResource);

        return create(id, scene, actorResource);
    }

    public static string GetActorTypeName(int actorType) => GetActorTypeNameFunc(actorType);
}