using System;
using System.Collections.Generic;
using System.Linq;

namespace OnyxCs.Gba.Engine2d;

public class GameObjects
{
    public GameObjects(Scene2DResource sceneResource)
    {
        ObjectsCount = sceneResource.GameObjectCount;
        AlwaysActorsCount = sceneResource.AlwaysActorsCount;
        ActorsCount = sceneResource.ActorsCount;
        CaptorsCount = sceneResource.CaptorsCount;
        
        Objects = new GameObject[ObjectsCount];

        // Create always actors
        for (int i = 0; i < sceneResource.AlwaysActors.Length; i++)
            Objects[i] = ObjectFactory.Create(i, sceneResource.AlwaysActors[i]);

        // Create actors
        for (int i = 0; i < sceneResource.Actors.Length; i++)
            Objects[AlwaysActorsCount + i] = ObjectFactory.Create(AlwaysActorsCount + i, sceneResource.Actors[i]);

        for (int i = 0; i < sceneResource.Captors.Length; i++)
            Objects[AlwaysActorsCount + ActorsCount + i] = new Captor(AlwaysActorsCount + ActorsCount + i, sceneResource.Captors[i]);

        // TODO: Create knots

        // Initialize actors
        foreach (BaseActor actor in Objects.Take(AlwaysActorsCount + ActorsCount).Cast<BaseActor>())
            actor.Init();
    }

    public int ObjectsCount { get; }
    public int AlwaysActorsCount { get; }
    public int ActorsCount { get; }
    public int CaptorsCount { get; }

    public GameObject[] Objects { get; }

    private IEnumerable<BaseActor> EnumerateAlwaysActors() => Objects.Take(AlwaysActorsCount).Cast<BaseActor>();
    public IEnumerable<BaseActor> EnumerateEnabledAlwaysActors() => EnumerateAlwaysActors().Where(x => x.IsEnabled);
    public IEnumerable<BaseActor> EnumerateDisabledAlwaysActors() => EnumerateAlwaysActors().Where(x => !x.IsEnabled);

    private IEnumerable<BaseActor> EnumerateActors()
    {
        // TODO: This should only enumerate actors in the current knot!
        return Objects.Skip(AlwaysActorsCount).Take(ActorsCount).Cast<BaseActor>();
    }
    public IEnumerable<BaseActor> EnumerateEnabledActors() => EnumerateActors().Where(x => x.IsEnabled);
    public IEnumerable<BaseActor> EnumerateDisabledActors() => EnumerateActors().Where(x => !x.IsEnabled);

    private IEnumerable<Captor> EnumerateCaptors() => Objects.Skip(AlwaysActorsCount + ActorsCount).Take(CaptorsCount).Cast<Captor>();
    public IEnumerable<Captor> EnumerateEnabledCaptors() => EnumerateCaptors().Where(x => x.IsEnabled);

    public BaseActor SpawnActor<T>(T actorType)
        where T : Enum
    {
        return SpawnActor((int)(object)actorType);
    }

    public BaseActor SpawnActor(int actorType)
    {
        BaseActor actor = EnumerateDisabledActors().Concat(EnumerateDisabledAlwaysActors()).FirstOrDefault(x => x.Type == actorType);
        actor?.SendMessage(Message.ResetWakeUp);
        return actor;
    }
}