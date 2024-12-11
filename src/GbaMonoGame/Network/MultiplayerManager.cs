namespace GbaMonoGame;

// TODO: Implement. This handles managing the RSMultiplayer and joypad.
public static class MultiplayerManager
{
    public static int MachineId { get; set; }
    public static int PlayersCount { get; set; }
    public static uint[] MachineTimers { get; set; }

    public static void Init()
    {
        // TODO: Implement
        
        MachineTimers = new uint[RSMultiplayer.MaxPlayersCount];
        MultiJoyPad.Init();
        
        MachineId = RSMultiplayer.MachineId;
        PlayersCount = RSMultiplayer.PlayersCount;
    }

    public static MubState Step()
    {
        // TODO: Implement
        return RSMultiplayer.MubState;
    }

    public static bool HasReadJoyPads()
    {
        // TODO: Implement
        return true;
    }

    public static void UpdateFromRSMultiplayer()
    {
        MachineId = RSMultiplayer.MachineId;
        PlayersCount = RSMultiplayer.PlayersCount;
    }

    public static uint GetMachineTimer()
    {
        return MachineTimers[MachineId];
    }
}