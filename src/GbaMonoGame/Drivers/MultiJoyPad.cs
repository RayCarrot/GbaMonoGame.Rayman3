using System;
using BinarySerializer.Ubisoft.GbaEngine;

namespace GbaMonoGame;

public static class MultiJoyPad
{
    static MultiJoyPad()
    {
        JoyPads = new SimpleJoyPad[RSMultiplayer.MaxPlayersCount][];
        for (int i = 0; i < JoyPads.Length; i++)
            JoyPads[i] = new SimpleJoyPad[BufferedFramesCount];

        ValidFlags = new bool[RSMultiplayer.MaxPlayersCount][];
        for (int i = 0; i < ValidFlags.Length; i++)
            ValidFlags[i] = new bool[BufferedFramesCount];

        FirstReadFlags = new bool[RSMultiplayer.MaxPlayersCount];
    }

    private const int BufferedFramesCount = 4;

    private static SimpleJoyPad[][] JoyPads { get; }
    private static bool[][] ValidFlags { get; }
    private static bool[] FirstReadFlags { get; }

    public static void Init()
    {
        for (int player = 0; player < RSMultiplayer.MaxPlayersCount; player++)
        {
            FirstReadFlags[player] = false;

            for (int frame = 0; frame < BufferedFramesCount; frame++)
            {
                JoyPads[player][frame] = new SimpleJoyPad();
                ValidFlags[player][frame] = false;
            }
        }
    }

    public static void Read(int machineId, uint machineTimer, GbaInput input)
    {
        if (machineId is < 0 or >= RSMultiplayer.MaxPlayersCount)
            throw new Exception("Invalid machine id");

        uint frame = machineTimer % BufferedFramesCount;
        uint prevFrame = (machineTimer - 1) % BufferedFramesCount;

        if (!FirstReadFlags[machineId])
        {
            JoyPads[machineId][frame].KeyTriggers = GbaInput.None;
            FirstReadFlags[machineId] = true;
        }
        else
        {
            JoyPads[machineId][frame].KeyTriggers = input ^ JoyPads[machineId][prevFrame].KeyStatus;
        }

        JoyPads[machineId][frame].KeyStatus = input;
        ValidFlags[machineId][frame] = true;
    }

    public static SimpleJoyPad GetJoyPad(int machineId)
    {
        if (machineId is < 0 or >= RSMultiplayer.MaxPlayersCount)
            throw new Exception("Invalid machine id");

        return JoyPads[machineId][MultiplayerManager.GetMachineTimer() % BufferedFramesCount];
    }

    public static bool IsButtonPressed(int machineId, GbaInput gbaInput)
    {
        if (RSMultiplayer.IsActive)
            return GetJoyPad(machineId).IsButtonPressed(gbaInput);
        else
            return JoyPad.IsButtonPressed(gbaInput);
    }

    public static bool IsButtonReleased(int machineId, GbaInput gbaInput)
    {
        if (RSMultiplayer.IsActive)
            return GetJoyPad(machineId).IsButtonReleased(gbaInput);
        else
            return JoyPad.IsButtonReleased(gbaInput);
    }

    public static bool IsButtonJustPressed(int machineId, GbaInput gbaInput)
    {
        if (RSMultiplayer.IsActive)
            return GetJoyPad(machineId).IsButtonJustPressed(gbaInput);
        else
            return JoyPad.IsButtonJustPressed(gbaInput);
    }

    public static bool IsButtonJustReleased(int machineId, GbaInput gbaInput)
    {
        if (RSMultiplayer.IsActive)
            return GetJoyPad(machineId).IsButtonJustReleased(gbaInput);
        else
            return JoyPad.IsButtonJustReleased(gbaInput);
    }
}