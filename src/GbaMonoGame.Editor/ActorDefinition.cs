using System;

namespace GbaMonoGame.Editor;

public class ActorDefinition
{
    public ActorDefinition(Enum actorId, string name, ActorActionDefinition[] actions) : this((int)(object)actorId, name, actions) { }

    public ActorDefinition(int actorId, string name, ActorActionDefinition[] actions)
    {
        ActorId = actorId;
        Name = name;
        Actions = actions;
    }

    public int ActorId { get; }
    public string Name { get; }
    public ActorActionDefinition[] Actions { get; }
}