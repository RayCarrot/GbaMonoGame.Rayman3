namespace GbaMonoGame.Editor;

public class ActorActionDefinition
{
    public required int ActionId { get; init; }
    public required string Name { get; init; }

    public int? AnimationIndex { get; init; }
    public int? BasePalette { get; init; }
}