using System;
using BinarySerializer.Ubisoft.GbaEngine.Rayman3;

namespace GbaMonoGame.Rayman3;

public static class LevelMusicManager
{
    private static short OverridenSoundEvent { get; set; }
    private static byte Timer { get; set; }

    private static bool Flag_0 { get; set; } // TODO: Name
    private static bool Flag_1 { get; set; } // TODO: Name
    private static bool ShouldPlaySpecialMusic { get; set; }
    private static bool HasOverridenLevelMusic { get; set; }

    private static void PlaySpecialMusic()
    {
        if (Flag_0 || Flag_1)
            return;

        if (GameInfo.MapId is MapId.TombOfTheAncients_M1 or MapId.TombOfTheAncients_M2)
        {
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Stop__ancients);
            SoundEventsManager.ProcessEvent(Rayman3SoundEvent.Play__spiderchase);
        }
        else
        {
            SoundEventsManager.ReplaceAllSongs(GameInfo.GetSpecialLevelMusicSoundEvent(), 3);
        }

        // TODO: N-Gage has a lot of additional code here

        Flag_0 = true;
        Flag_1 = true;
    }

    public static void Init()
    {
        Timer = 0;
        Flag_0 = false;
        Flag_1 = false;
        ShouldPlaySpecialMusic = false;
        HasOverridenLevelMusic = false;
    }

    public static void Step()
    {
        if (!SoundEventsManager.IsSongPlaying(Rayman3SoundEvent.Play__win3))
        {
            if (!Flag_1)
            {
                if (!Flag_0)
                {
                    if (ShouldPlaySpecialMusic)
                        PlaySpecialMusic();
                }
                else if (!ShouldPlaySpecialMusic)
                {
                    SoundEventsManager.ReplaceAllSongs(GameInfo.GetLevelMusicSoundEvent(), 3);
                    Flag_0 = false;
                    Flag_1 = true;
                }
            }
            else
            {
                Timer--;
                if (Timer == 0)
                {
                    Timer = 120;
                    Flag_1 = false;

                    if (HasOverridenLevelMusic)
                    {
                        SoundEventsManager.ProcessEvent(OverridenSoundEvent);
                        HasOverridenLevelMusic = false;
                    }
                }
            }
        }

        ShouldPlaySpecialMusic = false;
    }

    public static void PlaySpecialMusicIfDetected(GameObject obj)
    {
        Box objBox = new(
            minX: obj.Position.X - Engine.ScreenCamera.Resolution.X,
            minY: obj.Position.Y - Engine.ScreenCamera.Resolution.Y / 2,
            maxX: obj.Position.X + Engine.ScreenCamera.Resolution.X,
            maxY: obj.Position.Y + 5);

        if (Flag_0)
        {
            objBox = new Box(
                minX: objBox.MinX - 64,
                minY: objBox.MinY - 64,
                maxX: objBox.MaxX + 64,
                maxY: objBox.MaxY + 64);
        }

        Box mainActorDetectionBox = obj.Scene.MainActor.GetDetectionBox();

        if (mainActorDetectionBox.Intersects(objBox))
            ShouldPlaySpecialMusic = true;
    }

    public static void OverrideLevelMusic(Enum soundEventId) => OverrideLevelMusic((short)(object)soundEventId);
    public static void OverrideLevelMusic(short soundEventId)
    {
        if (HasOverridenLevelMusic) 
            return;
        
        Timer = 180;
        Flag_0 = false;
        Flag_1 = true;
        HasOverridenLevelMusic = true;
        OverridenSoundEvent = SoundEventsManager.ReplaceAllSongs(soundEventId, 0);
    }
}