using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame;

public static class JoyPad
{
    public static SimpleJoyPad Current { get; } = new();

    public static bool IsReplayFinished => Current.IsReplayFinished;

    public static void SetReplayData(GbaInput[] replayData) => Current.SetReplayData(replayData);

    public static void Scan() => Current.Scan();

    public static bool Check(GbaInput gbaInput) => Current.Check(gbaInput);
    public static bool CheckSingle(GbaInput gbaInput) => Current.CheckSingle(gbaInput);
    public static bool CheckSingleReleased(GbaInput gbaInput) => Current.CheckSingleReleased(gbaInput);
}