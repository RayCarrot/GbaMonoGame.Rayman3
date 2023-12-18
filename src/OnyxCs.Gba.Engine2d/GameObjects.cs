using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Onyx.Gba;
using OnyxCs.Gba.TgxEngine;

namespace OnyxCs.Gba.Engine2d;

public class GameObjects
{
    public GameObjects(Scene2DResource sceneResource)
    {
        ObjectsCount = sceneResource.GameObjectCount;
        AlwaysActorsCount = sceneResource.AlwaysActorsCount;
        ActorsCount = sceneResource.ActorsCount;
        CaptorsCount = sceneResource.CaptorsCount;
        KnotsWidth = sceneResource.KnotsWidth;
        Objects = new GameObject[ObjectsCount];
        Knots = sceneResource.Knots;
    }

    public int ObjectsCount { get; }
    public int AlwaysActorsCount { get; }
    public int ActorsCount { get; }
    public int CaptorsCount { get; }

    public GameObject[] Objects { get; }
    public Knot[] Knots { get; }
    public byte KnotsWidth { get; }
    public Knot CurrentKnot { get; set; }
    public Knot PreviousKnot { get; set; }

    // The game does this in the constructor, but we need the object instance to be created before doing this in case an
    // object has to access the main actor of the scene
    internal void Load(Scene2D scene, Scene2DResource sceneResource)
    {
        // Create always actors
        for (int i = 0; i < sceneResource.AlwaysActors.Length; i++)
            Objects[i] = ObjectFactory.Create(i, scene, sceneResource.AlwaysActors[i]);

        // Create actors
        for (int i = 0; i < sceneResource.Actors.Length; i++)
            Objects[AlwaysActorsCount + i] = ObjectFactory.Create(AlwaysActorsCount + i, scene, sceneResource.Actors[i]);

        for (int i = 0; i < sceneResource.Captors.Length; i++)
            Objects[AlwaysActorsCount + ActorsCount + i] = new Captor(AlwaysActorsCount + ActorsCount + i, scene, sceneResource.Captors[i]);

        // Initialize actors
        foreach (BaseActor actor in Objects.Take(AlwaysActorsCount + ActorsCount).Cast<BaseActor>())
            actor.Init();
    }

    public IEnumerable<BaseActor> EnumerateAlwaysActors(bool isEnabled)
    {
        return Objects.Take(AlwaysActorsCount).Where(x => x.IsEnabled == isEnabled).Cast<BaseActor>();
    }

    public IEnumerable<BaseActor> EnumerateActors(bool isEnabled, Knot knot = null)
    {
        knot ??= CurrentKnot;
        return knot.ActorIds.Select(x => Objects[x]).Where(x => x.IsEnabled == isEnabled).Cast<BaseActor>();
    }

    public IEnumerable<BaseActor> EnumerateAllActors(bool isEnabled, Knot knot = null)
    {
        return EnumerateAlwaysActors(isEnabled).Concat(EnumerateActors(isEnabled, knot));
    }

    public IEnumerable<GameObject> EnumerateAllGameObjects(bool isEnabled, Knot knot = null)
    {
        return EnumerateAlwaysActors(isEnabled).
            Concat(EnumerateActors(isEnabled, knot)).
            Cast<GameObject>().
            Concat(EnumerateCaptors(isEnabled, knot));
    }

    public IEnumerable<GameObject> EnumerateActorsAndCaptors(bool isEnabled, Knot knot = null)
    {
        return EnumerateActors(isEnabled, knot).
            Cast<GameObject>().
            Concat(EnumerateCaptors(isEnabled, knot));
    }

    public IEnumerable<Captor> EnumerateCaptors(bool isEnabled, Knot knot = null)
    {
        knot ??= CurrentKnot;
        return knot.CaptorIds.Select(x => Objects[x]).Where(x => x.IsEnabled == isEnabled).Cast<Captor>();
    }

    public bool UpdateCurrentKnot(TgxPlayfield playfield, Vector2 camPos)
    {
        // TODO: Offset position if Mode7

        TgxGameLayer physicalLayer = playfield.PhysicalLayer;

        if (physicalLayer.PixelWidth - Engine.ScreenCamera.ScaledGameResolution.X <= camPos.X)
            camPos = new Vector2(physicalLayer.PixelWidth - Engine.ScreenCamera.ScaledGameResolution.X - 1, camPos.Y);

        if (physicalLayer.PixelHeight - Engine.ScreenCamera.ScaledGameResolution.Y <= camPos.Y)
            camPos = new Vector2(camPos.X, physicalLayer.PixelHeight - Engine.ScreenCamera.ScaledGameResolution.Y - 1);

        int knotX = (int)(camPos.X / Engine.ScreenCamera.GameResolution.X);
        int knotY = (int)(camPos.Y / Engine.ScreenCamera.GameResolution.Y);
        Knot knot = Knots[knotX + knotY * KnotsWidth];

        if (knot == CurrentKnot)
            return false;

        PreviousKnot = CurrentKnot;
        CurrentKnot = knot;

        return true;
    }

    public BaseActor SpawnActor<T>(T actorType)
        where T : Enum
    {
        return SpawnActor((int)(object)actorType);
    }

    public BaseActor SpawnActor(int actorType)
    {
        // TODO: Check IsSpawnable flag
        BaseActor actor = EnumerateAllActors(isEnabled: false).FirstOrDefault(x => x.Type == actorType);
        actor?.ProcessMessage(Message.ResurrectWakeUp);
        return actor;
    }
}