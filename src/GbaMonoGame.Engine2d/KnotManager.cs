using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using GbaMonoGame.TgxEngine;

namespace GbaMonoGame.Engine2d;

public class KnotManager
{
    #region Constructor

    public KnotManager(Scene2DResource sceneResource)
    {
        GameObjectsCount = sceneResource.GameObjectCount;
        AlwaysActorsCount = sceneResource.AlwaysActorsCount;
        ActorsCount = sceneResource.ActorsCount;
        CaptorsCount = sceneResource.CaptorsCount;
        KnotsWidth = sceneResource.KnotsWidth;
        GameObjects = new GameObject[GameObjectsCount];
        Knots = sceneResource.Knots;

        // Create a special knot with every object which we use when loading all objects at once
        FullKnot = new Knot
        {
            ActorsCount = (byte)ActorsCount,
            CaptorsCount = (byte)CaptorsCount,
            ActorIds = Enumerable.Range(AlwaysActorsCount, ActorsCount).Select(x => (byte)x).ToArray(),
            CaptorIds = Enumerable.Range(AlwaysActorsCount + ActorsCount, CaptorsCount).Select(x => (byte)x).ToArray(),
        };
    }

    #endregion

    #region Private Properties

    private Knot FullKnot { get; }

    #endregion

    #region Public Properties

    // Total
    public int GameObjectsCount { get; }

    // Object types
    public int AlwaysActorsCount { get; }
    public int ActorsCount { get; }
    public int CaptorsCount { get; }

    public GameObject[] GameObjects { get; }
    public Knot[] Knots { get; }
    public byte KnotsWidth { get; }

    public Knot CurrentKnot { get; set; }
    public Knot PreviousKnot { get; set; }

    #endregion

    #region Public Methods

    // The game does this in the constructor, but we need the object instance to be created before doing this in case an
    // object has to access the main actor of the scene
    public void LoadGameObjects(Scene2D scene, Scene2DResource sceneResource)
    {
        // Create always actors
        for (int i = 0; i < sceneResource.AlwaysActors.Length; i++)
            GameObjects[i] = ObjectFactory.Create(i, scene, sceneResource.AlwaysActors[i]);

        // Create actors
        for (int i = 0; i < sceneResource.Actors.Length; i++)
            GameObjects[AlwaysActorsCount + i] = ObjectFactory.Create(AlwaysActorsCount + i, scene, sceneResource.Actors[i]);

        // Create captors
        for (int i = 0; i < sceneResource.Captors.Length; i++)
            GameObjects[AlwaysActorsCount + ActorsCount + i] = new Captor(AlwaysActorsCount + ActorsCount + i, scene, sceneResource.Captors[i]);

        // Initialize actors
        foreach (BaseActor actor in GameObjects.Take(AlwaysActorsCount + ActorsCount).Cast<BaseActor>())
            actor.Init();
    }

    public IEnumerable<BaseActor> EnumerateAlwaysActors(bool isEnabled)
    {
        return GameObjects.Take(AlwaysActorsCount).Where(x => x.IsEnabled == isEnabled).Cast<BaseActor>();
    }

    public IEnumerable<BaseActor> EnumerateActors(bool isEnabled, Knot knot = null)
    {
        knot ??= CurrentKnot;
        return knot.ActorIds.Select(x => GameObjects[x]).Where(x => x.IsEnabled == isEnabled).Cast<BaseActor>();
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

    public IEnumerable<Captor> EnumerateCaptors(bool isEnabled, Knot knot = null)
    {
        knot ??= CurrentKnot;
        return knot.CaptorIds.Select(x => GameObjects[x]).Where(x => x.IsEnabled == isEnabled).Cast<Captor>();
    }

    public GameObject GetGameObject(int instanceId) => GameObjects[instanceId];

    public bool UpdateCurrentKnot(TgxPlayfield playfield, Vector2 camPos)
    {
        Knot knot;

        // If the game is scaled we can't use the knots as they're pre-calculated for the original
        // screen resolution. So instead we use a knot where every object is loaded.
        if (Engine.Config.Scale != 1f) // TODO: Have this be a setting
        {
            knot = FullKnot;
        }
        else
        {
            // TODO: Offset position if Mode7

            TgxGameLayer physicalLayer = playfield.PhysicalLayer;

            if (physicalLayer.PixelWidth - playfield.Camera.Resolution.X <= camPos.X)
                camPos = new Vector2(physicalLayer.PixelWidth - playfield.Camera.Resolution.X - 1, camPos.Y);

            if (physicalLayer.PixelHeight - playfield.Camera.Resolution.Y <= camPos.Y)
                camPos = new Vector2(camPos.X, physicalLayer.PixelHeight - playfield.Camera.Resolution.Y - 1);

            int knotX = (int)(camPos.X / Engine.GameWindow.OriginalGameResolution.X);
            int knotY = (int)(camPos.Y / Engine.GameWindow.OriginalGameResolution.Y);
            knot = Knots[knotX + knotY * KnotsWidth];
        }


        if (knot == CurrentKnot)
            return false;

        PreviousKnot = CurrentKnot;
        CurrentKnot = knot;

        return true;
    }

    public T CreateProjectile<T>(Enum actorType)
        where T : BaseActor
    {
        return (T)CreateProjectile((int)(object)actorType);
    }

    public BaseActor CreateProjectile(int actorType)
    {
        // TODO: Check IsProjectile flag
        BaseActor actor = EnumerateAllActors(isEnabled: false).FirstOrDefault(x => x.Type == actorType);
        actor?.ProcessMessage(Message.ResurrectWakeUp);
        return actor;
    }

    #endregion
}