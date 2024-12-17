namespace GbaMonoGame;

// TODO: Update for N-Gage
// TODO: Implement. This handles receiving and sending packets of data between machines.
public static class RSMultiplayer
{
    public const int MaxPlayersCount = 4;

    public static bool IsActive { get; set; }
    public static MubState MubState { get; set; }
    public static int PlayersCount { get; set; }
    public static int MachineId { get; set; }
    public static bool IsMaster => MachineId == 0;
    public static bool IsSlave => MachineId != 0;

    public static void Init()
    {
        // TODO: Initialize. The game also sets communication hooks here.
        IsActive = true;
    }

    public static void Reset()
    {
        // TODO: Implement
    }

    public static void UnInit()
    {
        // TODO: Uninitialize
        IsActive = false;
    }

    public static void CheckForLostConnection()
    {
        // TODO: Implement
    }

    public static void Connect()
    {
        // TODO: Implement
    }

    public static void SendPacket(ushort[] data)
    {
        // TODO: Implement
    }

    public static ushort[] ReadPacket(int hubMachine)
    {
        // TODO: Implement
        return [0xace];
    }

    public static bool IsPacketPending(int hubMachine)
    {
        // TODO: Implement
        return true;
    }

    public static void ReleasePacket(int hubMachine)
    {
        // TODO: Implement
    }
}