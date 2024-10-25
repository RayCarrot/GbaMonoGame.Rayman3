using System.Collections.Generic;
using System.Linq;

namespace GbaMonoGame.Editor;

public static class EditorData
{
    private static Dictionary<int, ActorDefinition> ActorDefinitions { get; set; }

    public static void Init(ActorDefinition[] actorDefinitions)
    {
        ActorDefinitions = actorDefinitions.ToDictionary(x => x.ActorId);
    }
}