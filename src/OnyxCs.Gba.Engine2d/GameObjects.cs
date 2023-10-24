﻿namespace OnyxCs.Gba.Engine2d;

public class GameObjects
{
    public GameObjects(Scene2DResource sceneResource)
    {
        ObjectsCount = sceneResource.GameObjectCount;
        AlwaysActorsCount = sceneResource.AlwaysActorsCount;
        CaptorsCount = sceneResource.CaptorsCount;
        
        Actors = new BaseActor[ObjectsCount];

        // Create always actors
        for (int i = 0; i < sceneResource.AlwaysActors.Length; i++)
            Actors[i] = ObjectFactory.Create(i, sceneResource.AlwaysActors[i]);

        // Create actors
        for (int i = 0; i < sceneResource.Actors.Length; i++)
            Actors[AlwaysActorsCount + i] = ObjectFactory.Create(AlwaysActorsCount + i, sceneResource.Actors[i]);

        // TODO: Create captors
        // TODO: Create knots

        foreach (BaseActor actor in Actors)
        {
            actor.Init();
        }
    }

    public int ObjectsCount { get; }
    public int AlwaysActorsCount { get; }
    public int CaptorsCount { get; }
    public BaseActor[] Actors { get; }
}