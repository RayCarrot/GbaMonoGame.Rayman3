namespace GbaMonoGame;

public static class JoyPad
{
    public static SimpleJoyPad Current { get; } = new();

    public static bool Check(GbaInput gbaInput) => Current.Check(gbaInput);
    public static bool CheckSingle(GbaInput gbaInput) => Current.CheckSingle(gbaInput);
    public static bool CheckSingleReleased(GbaInput gbaInput) => Current.CheckSingleReleased(gbaInput);
}