using Microsoft.Xna.Framework;

namespace GbaMonoGame.Engine2d;

internal static class DebugBoxColor
{
    // Colors source: https://materialui.co/colors
    public static Color CaptorBox => new(255, 152, 0); // Orange 500
    public static Color DetectionBox => new(255, 193, 7); // Amber 500
    public static Color ActionBox => new(63, 81, 181); // Indigo 500
    public static Color AttackBox => new(244, 67, 54); // Red 500
    public static Color VulnerabilityBox => new(76, 175, 80); // Green 500
}