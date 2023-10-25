using Microsoft.Xna.Framework;

namespace OnyxCs.Gba;

public class DebugBox
{
    public DebugBox(string name, Rectangle rectangle, Vector2 position, Color color)
    {
        rectangle.Offset(position);

        Name = name;
        Rectangle = rectangle;
        Color = color;
    }

    public string Name { get; }
    public Rectangle Rectangle { get; }
    public Color Color { get; }
}