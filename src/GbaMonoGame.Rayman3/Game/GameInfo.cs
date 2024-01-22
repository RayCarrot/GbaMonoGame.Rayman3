using System;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;

namespace GbaMonoGame.Rayman3;

public static class GameInfo
{
    public static MapId? NextMapId { get; set; }
    public static MapId MapId { get; set; }
    public static LevelType LevelType { get; set; }
    public static int World { get; set; }
    public static int LoadedYellowLums { get; set; }
    public static int LoadedCages { get; set; }
    public static int YellowLumsCount { get; set; }
    public static int CagesCount { get; set; }
    public static int GameCubeCollectedYellowLumsCount { get; set; } // Unused since GCN levels don't have lums
    public static int GameCubeCollectedCagesCount { get; set; } // Unused since GCN levels don't have cages
    public static int GreenLums { get; set; }
    public static int LastGreenLumAlive { get; set; }
    public static Vector2 CheckpointPosition { get; set; }
    public static int RemainingTime { get; set; }
    public static bool field7_0x7 { get; set; }
    public static byte field12_0xf { get; set; }
    public static bool field22_0x1b { get; set; }
    public static Power Powers { get; set; }
    public static CheatFlags Cheats { get; set; }

    public static int CurrentSlot { get; set; }
    public static SaveGameSlot PersistentInfo { get; set; }

    public static LevelInfo Level => Levels[(int)MapId];
    public static LevelInfo[] Levels => Engine.Loader.Rayman3_LevelInfo;

    public static void Reset()
    {
        NextMapId = null;
        MapId = MapId.WoodLight_M1;
        LoadedYellowLums = 0;
        LoadedCages = 0;
        Powers = Power.None;
        Cheats = CheatFlags.None;
        field22_0x1b = field7_0x7;
        field7_0x7 = true;
        field12_0xf = 0;
        PersistentInfo = new SaveGameSlot();
        ResetPersistentInfo();
    }

    public static void ResetPersistentInfo()
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
        string saveFile = Engine.GameInstallation.SaveFilePath;
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
            ResetPersistentInfo();
            return false;
        }
    }

    public static void Save(int saveSlot)
    {
        // TODO: Implement
    }

    public static bool GetLumStatus(int lumId)
    {
        return (PersistentInfo.Lums[lumId >> 3] & (1 << (lumId & 7))) == 0;
    }

    public static bool GetCageStatus(int cageId)
    {
        return (PersistentInfo.Cages[cageId >> 3] & (1 << (cageId & 7))) == 0;
    }

    public static void KillLum(int lumId)
    {
        PersistentInfo.Lums[lumId >> 3] = (byte)(PersistentInfo.Lums[lumId >> 3] & ~(1 << (lumId & 7)));
    }

    public static void KillCage(int cageId)
    {
        PersistentInfo.Cages[cageId >> 3] = (byte)(PersistentInfo.Cages[cageId >> 3] & ~(1 << (cageId & 7)));
    }

    public static int GetCollectedYellowLumsInLevel(MapId mapId)
    {
        if (LevelType == LevelType.GameCube)
        {
            return GameCubeCollectedYellowLumsCount;
        }
        else
        {
            int count = 0;

            for (int i = 0; i < Levels[(int)mapId].LumsCount; i++)
            {
                if (HasCollectedYellowLum(i, mapId))
                    count++;
            }

            return count;
        }
    }

    public static int GetCollectedCagesInLevel(MapId cageId)
    {
        if (LevelType == LevelType.GameCube)
        {
            return GameCubeCollectedCagesCount;
        }
        else
        {
            int count = 0;

            for (int i = 0; i < Levels[(int)cageId].CagesCount; i++)
            {
                if (HasCollectedCage(i, cageId))
                    count++;
            }

            return count;
        }
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

    public static int GetTotalCollectedCages()
    {
        int count = 0;

        for (int i = 0; i < 50; i++)
        {
            if (GetCageStatus(i))
                count++;
        }

        return count;
    }

    public static bool HasCollectedYellowLum(int lumId, MapId mapId)
    {
        return GetLumStatus(Levels[(int)mapId].GlobalLumsIndex + lumId);
    }

    public static bool HasCollectedCage(int cageId, MapId mapId)
    {
        return GetCageStatus(Levels[(int)mapId].GlobalCagesIndex + cageId);
    }

    public static void SetYellowLumAsCollected(int lumId)
    {
        if (LevelType == LevelType.GameCube)
        {
            GameCubeCollectedYellowLumsCount++;
            
            if (GameCubeCollectedYellowLumsCount == YellowLumsCount)
            {
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumTotal_Mix02);
                SoundEventsManager.FUN_08001954(Rayman3SoundEvent.Play__win2);
            }
        }
        else
        {
            KillLum(Level.GlobalLumsIndex + lumId);

            // NOTE: Game also checks to MapId is not 0xFF, but that shouldn't be possible
            if (GetCollectedYellowLumsInLevel(MapId) == YellowLumsCount && LevelType != LevelType.Race)
            {
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumTotal_Mix02);
                SoundEventsManager.FUN_08001954(Rayman3SoundEvent.Play__win2);
            }
        }
    }

    public static void SetCageAsCollected(int cageId)
    {
        if (LevelType == LevelType.GameCube)
        {
            GameCubeCollectedCagesCount++;
            if (GameCubeCollectedCagesCount == CagesCount)
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumTotal_Mix02);
        }
        else
        {
            KillCage(Level.GlobalLumsIndex + cageId);

            // NOTE: Game also checks to MapId is not 0xFF, but that shouldn't be possible
            if (GetCollectedCagesInLevel(MapId) == CagesCount)
                SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__LumTotal_Mix02);
        }
    }

    public static void SetPowerBasedOnMap(MapId mapId)
    {
        if (mapId >= MapId.WoodLight_M2)
            Powers |= Power.DoubleFist;

        if (mapId >= MapId.BossMachine)
            Powers |= Power.Grab;

        if (mapId >= MapId.EchoingCaves_M2)
            Powers |= Power.Climb;

        if (mapId >= MapId.SanctuaryOfStoneAndFire_M3)
            Powers |= Power.SuperHelico;

        if (mapId >= MapId.BossRockAndLava)
            Powers |= Power.BodyShot;

        if (mapId >= MapId.BossScaleMan)
            Powers |= Power.SuperFist;
    }

    public static MapId GetNextLevelId()
    {
        return (MapId)Level.NextLevelId;
    }

    public static void LoadLevel(MapId mapId)
    {
        if (mapId == MapId.MarshAwakening1 && PersistentInfo.LastCompletedLevel < (int)MapId.MarshAwakening1)
        {
            FrameManager.SetNextFrame(new Act2());
        }
        else if (mapId == MapId.PirateShip_M1 && PersistentInfo.LastCompletedLevel < (int)MapId.PirateShip_M1)
        {
            FrameManager.SetNextFrame(new Act5());
        }
        else
        {
            FrameManager.SetNextFrame(LevelFactory.Create(mapId));
        }
    }

    public static void LoadLastWorld()
    {
        switch ((MapId)PersistentInfo.LastPlayedLevel)
        {
            case MapId.WoodLight_M1:
            case MapId.WoodLight_M2:
            case MapId.FairyGlade_M1:
            case MapId.FairyGlade_M2:
            case MapId.MarshAwakening1:
            case MapId.BossMachine:
            case MapId.SanctuaryOfBigTree_M1:
            case MapId.SanctuaryOfBigTree_M2:
            case MapId.Bonus1:
                LoadLevel(MapId.World1);
                break;
            
            case MapId.MissileSurPattes1:
            case MapId.EchoingCaves_M1:
            case MapId.EchoingCaves_M2:
            case MapId.CavesOfBadDreams_M1:
            case MapId.CavesOfBadDreams_M2:
            case MapId.BossBadDreams:
            case MapId.MenhirHills_M1:
            case MapId.MenhirHills_M2:
            case MapId.MarshAwakening2:
            case MapId.Bonus2:
            case MapId.ChallengeLy1:
                LoadLevel(MapId.World2);
                break;
            
            case MapId.SanctuaryOfStoneAndFire_M1:
            case MapId.SanctuaryOfStoneAndFire_M2:
            case MapId.SanctuaryOfStoneAndFire_M3:
            case MapId.BeneathTheSanctuary_M1:
            case MapId.BeneathTheSanctuary_M2:
            case MapId.ThePrecipice_M1:
            case MapId.ThePrecipice_M2:
            case MapId.BossRockAndLava:
            case MapId.TheCanopy_M1:
            case MapId.TheCanopy_M2:
            case MapId.SanctuaryOfRockAndLava_M1:
            case MapId.SanctuaryOfRockAndLava_M2:
            case MapId.SanctuaryOfRockAndLava_M3:
            case MapId.Bonus3:
                LoadLevel(MapId.World3);
                break;

            case MapId.TombOfTheAncients_M1:
            case MapId.TombOfTheAncients_M2:
            case MapId.BossScaleMan:
            case MapId.IronMountains_M1:
            case MapId.IronMountains_M2:
            case MapId.MissileSurPattes2:
            case MapId.PirateShip_M1:
            case MapId.PirateShip_M2:
            case MapId.BossFinal_M1:
            case MapId.BossFinal_M2:
            case MapId.Bonus4:
            case MapId._1000Lums:
            case MapId.ChallengeLy2:
            case MapId.ChallengeLyGCN:
                LoadLevel(MapId.World4);
                break;

            case MapId.Power1:
            case MapId.Power2:
            case MapId.Power3:
            case MapId.Power4:
            case MapId.Power5:
            case MapId.Power6:
            case MapId.World1:
            case MapId.World2:
            case MapId.World3:
            case MapId.World4:
            case MapId.WorldMap:
            default:
                throw new Exception("Invalid level to load world from");
        }
    }

    public static void SetNextMapId(MapId mapId)
    {
        LastGreenLumAlive = 0;
        NextMapId = mapId;
        GreenLums = 0;
        field22_0x1b = false;
        SetPowerBasedOnMap((MapId)PersistentInfo.LastCompletedLevel);

        switch (mapId)
        {
            case MapId.WoodLight_M1:
            case MapId.WoodLight_M2:
            case MapId.FairyGlade_M1:
            case MapId.FairyGlade_M2:
            case MapId.MarshAwakening1:
            case MapId.BossMachine:
            case MapId.SanctuaryOfBigTree_M1:
            case MapId.SanctuaryOfBigTree_M2:
            case MapId.Bonus1:
            case MapId.World1:
                World = 0;
                break;

            case MapId.MissileSurPattes1:
            case MapId.EchoingCaves_M1:
            case MapId.EchoingCaves_M2:
            case MapId.CavesOfBadDreams_M1:
            case MapId.CavesOfBadDreams_M2:
            case MapId.BossBadDreams:
            case MapId.MenhirHills_M1:
            case MapId.MenhirHills_M2:
            case MapId.MarshAwakening2:
            case MapId.Bonus2:
            case MapId.ChallengeLy1:
            case MapId.World2:
                World = 1;
                break;

            case MapId.SanctuaryOfStoneAndFire_M1:
            case MapId.SanctuaryOfStoneAndFire_M2:
            case MapId.SanctuaryOfStoneAndFire_M3:
            case MapId.BeneathTheSanctuary_M1:
            case MapId.BeneathTheSanctuary_M2:
            case MapId.ThePrecipice_M1:
            case MapId.ThePrecipice_M2:
            case MapId.BossRockAndLava:
            case MapId.TheCanopy_M1:
            case MapId.TheCanopy_M2:
            case MapId.SanctuaryOfRockAndLava_M1:
            case MapId.SanctuaryOfRockAndLava_M2:
            case MapId.SanctuaryOfRockAndLava_M3:
            case MapId.Bonus3:
            case MapId.World3:
                World = 2;
                break;

            case MapId.TombOfTheAncients_M1:
            case MapId.TombOfTheAncients_M2:
            case MapId.BossScaleMan:
            case MapId.IronMountains_M1:
            case MapId.IronMountains_M2:
            case MapId.MissileSurPattes2:
            case MapId.PirateShip_M1:
            case MapId.PirateShip_M2:
            case MapId.BossFinal_M1:
            case MapId.BossFinal_M2:
            case MapId.Bonus4:
            case MapId._1000Lums:
            case MapId.ChallengeLy2:
            case MapId.ChallengeLyGCN:
            case MapId.World4:
                World = 3;
                break;

            case MapId.Power1:
            case MapId.Power2:
            case MapId.Power3:
            case MapId.Power4:
            case MapId.Power5:
            case MapId.Power6:
                World = 4;
                break;

            // TODO: Multiplayer maps are world 5

            case MapId.WorldMap:
                // Do nothing
                break;
        }
        // TODO: Implement
    }

    public static void PlayLevelMusic()
    {
        SoundEventsManager.ProcessEvent(Level.StartMusicSoundEvent);
    }

    public static void StopLevelMusic()
    {
        if (LevelType != LevelType.GameCube)
            SoundEventsManager.ProcessEvent(Level.StopMusicSoundEvent);
    }
}