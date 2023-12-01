using System;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Onyx.Gba.Rayman3;

namespace OnyxCs.Gba.Rayman3;

public static class GameInfo
{
    static GameInfo()
    {
        field7_0x7 = true;
        PersistentInfo = new SaveGameSlot();
        Reset();
    }

    public static MapId? NextMapId { get; set; }
    public static MapId MapId { get; set; }
    public static int World { get; set; }
    public static int LoadedYellowLums { get; set; }
    public static int GreenLums { get; set; }
    public static int LastGreenLumAlive { get; set; }
    public static Vector2 CheckpointPosition { get; set; }
    public static bool field7_0x7 { get; set; }
    public static byte field12_0xf { get; set; }
    public static CheatFlags Cheats { get; set; }

    public static SaveGameSlot PersistentInfo { get; set; }

    public static LevelInfo Level => Levels[(int)MapId];
    public static LevelInfo[] Levels => Engine.Loader.Rayman3_LevelInfo;

    public static void Reset()
    {
        PersistentInfo.Lums ??= new byte[125];
        Array.Fill(PersistentInfo.Lums, (byte)0xFF);

        PersistentInfo.Cages ??= new byte[7];
        Array.Fill(PersistentInfo.Cages, (byte)0xFF);

        PersistentInfo.LastPlayedLevel = 0;
        PersistentInfo.LastCompletedLevel = 0;
        PersistentInfo.Lives = 3;
        PersistentInfo.FinishedLyChallenge1 = false;
        PersistentInfo.FinishedLyChallenge2 = false;
        PersistentInfo.FinishedLyChallengeGCN = false;
        PersistentInfo.UnlockedBonus1 = false;
        PersistentInfo.UnlockedBonus2 = false;
        PersistentInfo.UnlockedBonus3 = false;
        PersistentInfo.UnlockedBonus4 = false;
        PersistentInfo.UnlockedWorld2 = false;
        PersistentInfo.UnlockedWorld3 = false;
        PersistentInfo.UnlockedWorld4 = false;
        PersistentInfo.PlayedWorld2Unlock = false;
        PersistentInfo.PlayedWorld3Unlock = false;
        PersistentInfo.PlayedWorld4Unlock = false;
        PersistentInfo.PlayedWorld4Act = false;
        PersistentInfo.PlayedMurphyWorldHelp = false;
        PersistentInfo.UnlockedFinalBoss = false;
        PersistentInfo.UnlockedLyChallengeGCN = false;
        PersistentInfo.LastCompletedGCNBonus = 0;
    }

    public static bool Load(int saveSlot)
    {
        string saveFile = Engine.Config.SaveFile;
        using Context context = Engine.Context;

        if (!context.FileExists(saveFile))
        {
            EEPROMEncoder encoder = new(0x200);
            context.AddFile(new EncodedLinearFile(context, saveFile, encoder));
        }

        if (((PhysicalFile)context.GetRequiredFile(saveFile)).SourceFileExists)
        {
            // TODO: Try/catch?
            SaveGame saveGame = FileFactory.Read<SaveGame>(context, saveFile);
            PersistentInfo = saveGame.Slots[saveSlot];
            return true;
        }
        else
        {
            Reset();
            return false;
        }
    }

    public static bool GetLumStatus(int lumId)
    {
        return (PersistentInfo.Lums[lumId >> 3] & (1 << (lumId & 7))) == 0;
    }

    public static bool GetCageStatus(int cageId)
    {
        return (PersistentInfo.Cages[cageId >> 3] & (1 << (cageId & 7))) == 0;
    }

    public static int GetTotalCollectedYellowLums()
    {
        int count = 0;

        for (int i = 0; i < 1000; i++)
        {
            if (GetLumStatus(i))
                count++;
        }

        return count;
    }

    public static int GetTotalCollectedYellowCages()
    {
        int count = 0;

        for (int i = 0; i < 50; i++)
        {
            if (GetCageStatus(i))
                count++;
        }

        return count;
    }

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