﻿using BinarySerializer.Onyx.Gba.Rayman3;

namespace OnyxCs.Gba.Rayman3;

public static class GameInfo
{
    public static MapId NextMapId { get; set; }
    public static MapId MapId { get; set; }
    public static int World { get; set; }
    public static int LoadedYellowLums { get; set; }
    public static int GreenLums { get; set; }
    public static int LastGreenLumAlive { get; set; }
    public static Vector2 CheckpointPosition { get; set; }
    public static byte field12_0xf { get; set; }
    public static CheatFlags Cheats { get; set; }

    public static LevelInfo Level => Levels[(int)MapId];
    public static LevelInfo[] Levels => Engine.Loader.Rayman3_LevelInfo;

    public static void SetNextMapId(MapId mapId)
    {
        LastGreenLumAlive = 0;
        NextMapId = mapId;
        GreenLums = 0;
        // TODO: More setup...

    }

    public static void PlayLevelMusic()
    {
        SoundManager.Play(Level.StartMusicSoundEvent);
    }

    public static void StopLevelMusic()
    {
        SoundManager.Play(Level.StopMusicSoundEvent);
    }
}