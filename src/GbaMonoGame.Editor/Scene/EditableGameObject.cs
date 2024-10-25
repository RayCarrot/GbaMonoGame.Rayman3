namespace GbaMonoGame.Editor;

public abstract class EditableGameObject
{
    protected EditableGameObject(EditableScene2D scene, GameObjectResource gameObjectResource)
    {
        Scene = scene;
        Position = gameObjectResource.Pos.ToVector2();
    }

    public EditableScene2D Scene { get; }
    public Vector2 Position { get; set; }

    public abstract Box GetSelectionBox();
}