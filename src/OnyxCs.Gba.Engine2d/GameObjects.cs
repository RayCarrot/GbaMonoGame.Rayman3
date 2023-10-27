﻿using System;
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

    public IEnumerable<BaseActor> EnumerateAlwaysActors() => Objects.Take(AlwaysActorsCount).Cast<BaseActor>();
    public IEnumerable<BaseActor> EnumerateActors()
    {
        // TODO: This should only enumerate actors in the current knot!
        return Objects.Skip(AlwaysActorsCount).Cast<BaseActor>();
    }

    public BaseActor SpawnActor<T>(T actorType)
        where T : Enum
    {
        return SpawnActor((int)(object)actorType);
    }

    public BaseActor SpawnActor(int actorType)
    {
        BaseActor actor = EnumerateActors().Concat(EnumerateAlwaysActors()).FirstOrDefault(x => x.Type == actorType);
        actor?.SendMessage(Message.Spawn);
        return actor;
    }
}