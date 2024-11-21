namespace GbaMonoGame;

// TODO: Implement. This handles receiving and sending packets of data between machines.
public static class RSMultiplayer
{
    public const int MaxPlayersCount = 4;

    public static bool IsActive { get; set; }
    public static int PlayersCount { get; set; }
    public static int MachineId { get; set; }
    public static bool IsMaster => MachineId == 0;
    public static bool IsSlave => MachineId != 0;

    public static void Init()
    {
        // TODO: Initialize. The game also sets communication hooks here.
        IsActive = true;
    }

    public static void UnInit()
    {
        // TODO: Uninitialize
        IsActive = false;
    }
}