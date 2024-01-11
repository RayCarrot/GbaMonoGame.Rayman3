using System;
using System.Collections.Generic;
using System.Linq;

namespace GbaMonoGame.Rayman3;

public static class LevelFactory
{
    public delegate Frame CreateLevel(MapId mapId);

    private static Dictionary<int, CreateLevel> LevelCreations { get; set; }

    public static void Init<T>(Dictionary<T, CreateLevel> levelCreations)
        where T : Enum
    {
        LevelCreations = levelCreations.ToDictionary(x => (int)(object)x.Key, x => x.Value);
    }

    public static void Init(Dictionary<int, CreateLevel> levelCreations)
    {
        LevelCreations = levelCreations;
    }

    public static Frame Create(MapId mapId)
    {
        if (!LevelCreations.TryGetValue((int)mapId, out CreateLevel create))
            return new DummyLevel(mapId);

        return create(mapId);
    }
}