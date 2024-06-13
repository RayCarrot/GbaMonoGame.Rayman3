using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using ImGuiNET;
using Microsoft.Xna.Framework.Audio;
using SoundBank = BinarySerializer.Ubisoft.GbaEngine.SoundBank;

namespace GbaMonoGame;

// TODO: Find better C# audio library since the MonoGame one is too limiting. We need to support loop points and ideally fade-out.
// TODO: Implement for N-Gage.
public static class SoundEventsManager
{
    #region Private Fields

    private static readonly Dictionary<int, SoundEffect> _songs = new();
    private static Dictionary<SoundType, int> _sampleSongs;
    private static SoundBank _soundBank;
    private static readonly float[] _volumePerType = Enumerable.Repeat(SoundEngineInterface.MaxVolume, 8).ToArray();
    private static readonly List<ActiveSong> _activeSongs = new(); // On GBA this is max 4 songs, but we don't need that limit
    private static SoundEffectInstance _activeSampleSong;
    private static SoundType? _activeSampleSongType;
    private static CallBackSet _callBacks;

    private static int[] RollOffTable { get; } =
    {
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
        0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x7F, 0x7F, 0x7E, 0x7E, 0x7E, 0x7D, 0x7D, 0x7C,
        0x7C, 0x7C, 0x7B, 0x7B, 0x7A, 0x7A, 0x7A, 0x79, 0x79, 0x78, 0x78, 0x78, 0x77, 0x77, 0x76, 0x76,
        0x76, 0x75, 0x75, 0x74, 0x74, 0x74, 0x73, 0x73, 0x72, 0x72, 0x72, 0x71, 0x71, 0x70, 0x70, 0x70,
        0x6F, 0x6F, 0x6E, 0x6E, 0x6E, 0x6D, 0x6D, 0x6C, 0x6C, 0x6C, 0x6B, 0x6B, 0x6A, 0x6A, 0x6A, 0x69,
        0x69, 0x68, 0x68, 0x68, 0x67, 0x67, 0x66, 0x66, 0x66, 0x65, 0x65, 0x64, 0x64, 0x64, 0x63, 0x63,
        0x62, 0x62, 0x62, 0x61, 0x61, 0x60, 0x60, 0x60, 0x5F, 0x5F, 0x5E, 0x5E, 0x5E, 0x5D, 0x5D, 0x5C,
        0x5C, 0x5C, 0x5B, 0x5B, 0x5A, 0x5A, 0x5A, 0x59, 0x59, 0x58, 0x58, 0x58, 0x57, 0x57, 0x56, 0x56,
        0x56, 0x55, 0x55, 0x54, 0x54, 0x54, 0x53, 0x53, 0x52, 0x52, 0x52, 0x51, 0x51, 0x50, 0x50, 0x50,
        0x4F, 0x4F, 0x4E, 0x4E, 0x4E, 0x4D, 0x4D, 0x4C, 0x4C, 0x4C, 0x4B, 0x4B, 0x4A, 0x4A, 0x4A, 0x49,
        0x49, 0x48, 0x48, 0x48, 0x47, 0x47, 0x46, 0x46, 0x46, 0x45, 0x45, 0x44, 0x44, 0x44, 0x43, 0x43,
        0x42, 0x42, 0x42, 0x41, 0x41, 0x40, 0x40, 0x40, 0x3F, 0x3F, 0x3E, 0x3E, 0x3E, 0x3D, 0x3D, 0x3C,
        0x3C, 0x3C, 0x3B, 0x3B, 0x3A, 0x3A, 0x3A, 0x39, 0x39, 0x38, 0x38, 0x38, 0x37, 0x37, 0x36, 0x36,
        0x36, 0x35, 0x35, 0x34, 0x34, 0x34, 0x33, 0x33, 0x32, 0x32, 0x32, 0x31, 0x31, 0x30, 0x30, 0x30,
        0x2F, 0x2F, 0x2E, 0x2E, 0x2E, 0x2D, 0x2D, 0x2C, 0x2C, 0x2C, 0x2B, 0x2B, 0x2A, 0x2A, 0x2A, 0x29,
        0x29, 0x28, 0x28, 0x28, 0x27, 0x27, 0x26, 0x26, 0x26, 0x25, 0x25, 0x24, 0x24, 0x24, 0x23, 0x23,
        0x22, 0x22, 0x22, 0x21, 0x21, 0x20, 0x20, 0x20, 0x1F, 0x1F, 0x1E, 0x1E, 0x1E, 0x1D, 0x1D, 0x1C,
        0x1C, 0x1C, 0x1B, 0x1B, 0x1A, 0x1A, 0x1A, 0x19, 0x19, 0x18, 0x18, 0x18, 0x17, 0x17, 0x16, 0x16,
        0x16, 0x15, 0x15, 0x14, 0x14, 0x14, 0x13, 0x13, 0x12, 0x12, 0x12, 0x11, 0x11, 0x10, 0x10, 0x10,
        0x0F, 0x0F, 0x0E, 0x0E, 0x0E, 0x0D, 0x0D, 0x0C, 0x0C, 0x0C, 0x0B, 0x0B, 0x0A, 0x0A, 0x0A, 0x09,
        0x09, 0x08, 0x08, 0x08, 0x07, 0x07, 0x06, 0x06, 0x06, 0x05, 0x05, 0x04, 0x04, 0x04, 0x03, 0x00,
    };

    #endregion

    #region Private Methods

    private static SoundEvent GetEventFromId(short soundEventId)
    {
        return _soundBank.Events[soundEventId];
    }

    private static SoundResource GetSoundResource(ushort resourceId)
    {
        SoundResource res = _soundBank.Resources[resourceId];

        switch (res.Type)
        {
            case SoundResource.ResourceType.Song:
                return res;

            case SoundResource.ResourceType.Switch:
                throw new NotImplementedException(); // Unused in Rayman 3

            case SoundResource.ResourceType.Random:
                ushort? resId = GetRandomResourceId(res);

                if (resId == null)
                    return null;

                return GetSoundResource(resId.Value);

            default:
                throw new Exception($"Invalid resource type {res.Type}");
        }
    }

    private static ushort? GetRandomResourceId(SoundResource res)
    {
        // TODO: Use Random.Shared or game's random implementation? It matters less here than in the game code though.
        int rand = Random.Shared.Next(100);

        for (int i = 0; i < res.ResourceIdsCount; i++)
        {
            if (rand < res.ResourceIdConditions[i])
                return res.ResourceIds[i];
        }

        return null;
    }

    private static void CreateSong(short soundEventId, SoundResource res, SoundEvent evt, object obj)
    {
        // NOTE: On GBA only 4 songs can play at once. It checks if there's an available one, or one with lower priority. We however don't need that.

        SoundEffect sndEffect = _songs[res.SongTableIndex];
        SoundEffectInstance sndEffectInstance = sndEffect.CreateInstance();

        ActiveSong song = new()
        {
            Obj = obj,
            EventId = soundEventId,
            NextSoundEventId = -1,
            Priority = evt.Priority,
            SoundType = evt.SoundType,
            Volume = -1,
            Pan = -1,
            IsPlaying = true,
            IsRollOffEnabled = evt.EnableRollOff,
            IsPanEnabled = evt.EnablePan,
            IsFadingOut = false,
            field2_0x3 = false,
            Loop = res.Loop,
            IsMusic = res.IsMusic,
            SoundEffect = sndEffect,
            SoundInstance = sndEffectInstance
        };

        sndEffectInstance.IsLooped = res.Loop;
        _activeSongs.Add(song);

        UpdateVolumeAndPan(song);
        sndEffectInstance.Play();
    }

    private static void ReplaceSong(short soundEventId, short nextEventId, float fadeOut, object obj)
    {
        bool foundSong = false;

        foreach (ActiveSong song in _activeSongs)
        {
            if (song.IsPlaying && !song.IsFadingOut && song.EventId == soundEventId && song.Obj == obj)
            {
                if (song.Volume != SoundEngineInterface.MaxVolume)
                    fadeOut = 0;

                song.field2_0x3 = false;
                song.Loop = false;
                song.SoundInstance.IsLooped = false;

                if (fadeOut == 0)
                {
                    song.SoundInstance.Stop();
                }
                else
                {
                    // TODO: Fade out
                    Logger.NotImplemented("Not implemented fading out song");
                    song.SoundInstance.Stop();
                }

                song.IsFadingOut = true;

                if (!foundSong)
                {
                    foundSong = true;
                    song.NextSoundEventId = nextEventId;
                }
            }
        }

        if (!foundSong && nextEventId != -1)
            ProcessEvent(nextEventId, obj);
    }

    private static void CalculateRollOffAndPan(Vector2 mikePos, out float rollOffLvl, out float dx, object obj)
    {
        Vector2 objPos = _callBacks.GetObjectPosition(obj);

        Vector2 dist = mikePos - objPos;
        Vector2 absDist = new(Math.Abs(dist.X), Math.Abs(dist.Y));

        float largestDist = absDist.Y < absDist.X ? absDist.X : absDist.Y;
        float rollOffIndex = largestDist + absDist.Y + absDist.X / 2;

        if (rollOffIndex > RollOffTable.Length - 1)
            rollOffIndex = RollOffTable.Length - 1;

        rollOffLvl = RollOffTable[(int)rollOffIndex];
        dx = Math.Clamp(-dist.X / 2, SoundEngineInterface.MinPan, SoundEngineInterface.MaxPan);
    }

    private static void UpdateVolumeAndPan(ActiveSong song)
    {
        float vol;
        float pan;

        if (song.Obj == null && (song.IsRollOffEnabled || song.IsPanEnabled))
            throw new Exception("Song has roll-off or pan enabled, but no object is set!");

        if (song.IsRollOffEnabled || song.IsPanEnabled)
        {
            Vector2 mikePos = _callBacks.GetMikePosition(song.Obj);
            CalculateRollOffAndPan(mikePos, out vol, out pan, song.Obj);

            if (!song.IsRollOffEnabled)
                vol = SoundEngineInterface.MaxVolume;
            if (!song.IsPanEnabled)
                pan = 0;
        }
        else
        {
            vol = SoundEngineInterface.MaxVolume;
            pan = 0;
        }

        vol *= GetVolumeForType(song.SoundType) / SoundEngineInterface.MaxVolume;

        if (song.SoundType == SoundType.Sfx)
            vol *= Engine.Config.SfxVolume;
        else if (song.SoundType == SoundType.Music)
            vol *= Engine.Config.MusicVolume;

        if (song.Volume != vol || song.Pan != pan)
        {
            song.SoundInstance.Volume = vol / SoundEngineInterface.MaxVolume;
            // TODO: This currently doesn't work - issue in MonoGame? Pan is unused in Rayman 3 though, so we can ignore it.
            song.SoundInstance.Pan = pan switch
            {
                > 0 => pan / SoundEngineInterface.MaxPan,
                < 0 => -pan / SoundEngineInterface.MinPan,
                _ => 0
            };

            song.Volume = vol;
            song.Pan = pan;
        }
    }

    #endregion

    #region Internal Methods

    internal static void Load(int soundBankResourceId, Dictionary<int, string> songTable, Dictionary<SoundType, int> sampleSongs)
    {
        if (Engine.Settings.Platform == Platform.GBA)
            _soundBank = Storage.LoadResource<SoundBank>(soundBankResourceId);
        
        // TODO: Load N-Gage sound tables

        Dictionary<string, SoundEffect> loadedSounds = new();
        foreach (var song in songTable)
        {
            if (song.Value == null)
                continue;

            if (loadedSounds.TryGetValue(song.Value, out SoundEffect snd))
            {
                _songs[song.Key] = snd;
            }
            else
            {
                snd = SoundEffect.FromFile($"{song.Value}.wav");
                snd.Name = song.Value;
                loadedSounds[song.Value] = snd;
                _songs[song.Key] = snd;
            }
        }

        _sampleSongs = sampleSongs;
    }

    internal static void DrawDebugLayout()
    {
        if (ImGui.BeginTable("_songs", 5))
        {
            ImGui.TableSetupColumn("Event", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("State", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Duration", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Next");
            ImGui.TableHeadersRow();

            foreach (ActiveSong playingSong in _activeSongs)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.EventId}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundEffect.Name}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundInstance.State}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundEffect.Duration.TotalSeconds:F}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.NextSoundEventId}");
            }

            ImGui.EndTable();
        }
    }

    #endregion

    #region Public Methods

    public static void SetCallBacks(CallBackSet callBacks)
    {
        _callBacks = callBacks;
    }

    public static void RefreshEventSet()
    {
        foreach (ActiveSong song in _activeSongs.ToArray())
        {
            if (!song.IsPlaying) 
                continue;
            
            if (song.SoundInstance.State == SoundState.Stopped && (!song.field2_0x3 || !song.Loop))
            {
                song.IsPlaying = false;

                if (song.NextSoundEventId != -1)
                    ProcessEvent(song.NextSoundEventId, song.Obj);
            }
            else if (!song.IsFadingOut)
            {
                UpdateVolumeAndPan(song);
            }

            if (!song.IsPlaying)
            {
                song.SoundInstance.Dispose();
                _activeSongs.Remove(song);
            }
        }
    }

    public static void ProcessEvent(Enum soundEventId) => ProcessEvent(soundEventId, null);
    public static void ProcessEvent(short soundEventId) => ProcessEvent(soundEventId, null);
    public static void ProcessEvent(Enum soundEventId, object obj) => ProcessEvent((short)(object)soundEventId, obj);
    public static void ProcessEvent(short soundEventId, object obj)
    {
        switch (Engine.Settings.Platform)
        {
            case Platform.GBA:
                SoundEvent evt = GetEventFromId(soundEventId);

                switch (evt.Type)
                {
                    case SoundEvent.SoundEventType.PlaySong:
                        SoundResource res = GetSoundResource(evt.ResourceId);

                        if (res != null)
                            CreateSong(soundEventId, res, evt, obj);
                        break;

                    case SoundEvent.SoundEventType.StopSong:
                        ReplaceSong(evt.StopEventId, -1, evt.FadeOutTime, obj);
                        break;

                    case SoundEvent.SoundEventType.StopAndSetNext:
                        ReplaceSong(evt.StopEventId, evt.NextEventId, evt.FadeOutTime, obj);
                        break;
                }
                break;

            case Platform.NGage:
                // TODO: Implement
                break;

            default:
                throw new UnsupportedPlatformException();
        }
    }

    public static void SetVolumeForType(SoundType type, float newVolume)
    {
        // Only implemented on GBA
        if (Engine.Settings.Platform != Platform.GBA)
            return;

        if ((byte)type > 7)
            throw new ArgumentOutOfRangeException(nameof(type), type, "Type must be a value between 0-7");

        if (newVolume is < 0 or > SoundEngineInterface.MaxVolume)
            throw new ArgumentOutOfRangeException(nameof(newVolume), newVolume, "Volume must be a value between 0-128");

        _volumePerType[(byte)type] = newVolume;
    }

    public static float GetVolumeForType(SoundType type)
    {
        // Only implemented on GBA
        if (Engine.Settings.Platform != Platform.GBA)
            return SoundEngineInterface.MaxVolume;

        if ((byte)type > 7)
            throw new ArgumentOutOfRangeException(nameof(type), type, "Type must be a value between 0-7");

        return _volumePerType[(byte)type];
    }

    public static bool IsSongPlaying(Enum soundEventId) => IsSongPlaying((short)(object)soundEventId);
    public static bool IsSongPlaying(short soundEventId)
    {
        return _activeSongs.Any(x => x.IsPlaying && x.EventId == soundEventId);
    }

    public static void SetSoundPitch(Enum soundEventId, float pitch) => SetSoundPitch((short)(object)soundEventId, pitch);
    public static void SetSoundPitch(short soundEventId, float pitch)
    {
        // TODO: Implement
    }

    public static short ReplaceAllSongs(Enum soundEventId, float fadeOut) => ReplaceAllSongs((short)(object)soundEventId, fadeOut);
    public static short ReplaceAllSongs(short soundEventId, float fadeOut)
    {
        bool firstSong = true;
        short firstEventId = -1;

        foreach (ActiveSong song in _activeSongs)
        {
            if (song.IsPlaying && song.Priority == 100)
            {
                if (firstSong)
                {
                    if (!song.IsFadingOut)
                    {
                        ReplaceSong(song.EventId, soundEventId, fadeOut, null);
                        firstEventId = song.EventId;
                    }
                    else
                    {
                        firstEventId = song.NextSoundEventId;
                        song.NextSoundEventId = soundEventId;
                    }

                    firstSong = false;
                }
                else
                {
                    ReplaceSong(song.EventId, -1, fadeOut, null);
                }
            }
        }

        return firstEventId;
    }

    public static void FinishReplacingAllSongs()
    {
        foreach (ActiveSong song in _activeSongs.ToArray())
        {
            if (song.IsPlaying && song.IsFadingOut && song.NextSoundEventId != -1)
            {
                song.SoundInstance.Stop();
                song.SoundInstance.Dispose();
                _activeSongs.Remove(song);

                ProcessEvent(song.NextSoundEventId, song.Obj);
            }
        }
    }

    public static void StopAllSongs()
    {
        foreach (ActiveSong playingSong in _activeSongs)
        {
            playingSong.SoundInstance.Stop();
            playingSong.SoundInstance.Dispose();
        }

        _activeSongs.Clear();
    }

    public static void PauseAllSongs()
    {
        foreach (ActiveSong playingSong in _activeSongs)
        {
            playingSong.SoundInstance.Pause();
        }
    }

    public static void ResumeAllSongs()
    {
        foreach (ActiveSong playingSong in _activeSongs)
        {
            UpdateVolumeAndPan(playingSong);
            playingSong.SoundInstance.Resume();
        }
    }

    public static void PlaySampleSong(SoundType type, bool restart, float volume)
    {
        if (!restart && _activeSampleSongType == type)
        {
            _activeSampleSong.Volume = volume;
        }
        else
        {
            StopSampleSongs();
            _activeSampleSong = _songs[_sampleSongs[type]].CreateInstance();
            _activeSampleSong.Volume = volume;
            _activeSampleSong.Play();
            _activeSampleSongType = type;
        }
    }

    public static void StopSampleSongs()
    {
        if (_activeSampleSong != null)
        {
            _activeSampleSong.Stop();
            _activeSampleSong.Dispose();
            _activeSampleSong = null;
            _activeSampleSongType = null;
        }
    }

    public static void StopSampleSong(SoundType type)
    {
        if (_activeSampleSong != null && _activeSampleSongType == type)
        {
            _activeSampleSong.Stop();
            _activeSampleSong.Dispose();
            _activeSampleSong = null;
            _activeSampleSongType = null;
        }
    }

    #endregion

    #region Data Types

    private class ActiveSong
    {
        public object Obj { get; init; }
        public short EventId { get; init; }
        public short NextSoundEventId { get; set; }
        public int Priority { get; init; }
        public SoundType SoundType { get; init; }

        public float Volume { get; set; }
        public float Pan { get; set; }

        // Flags
        public bool IsPlaying { get; set; }
        public bool IsRollOffEnabled { get; init; }
        public bool IsPanEnabled { get; init; }
        public bool IsFadingOut { get; set; }

        // Music player
        public bool field2_0x3 { get; set; } // Always false?
        public bool Loop { get; set; }
        public bool IsMusic { get; init; }

        // MonoGame
        public SoundEffectInstance SoundInstance { get; init; }
        public SoundEffect SoundEffect { get; init; }
    }

    public class CallBackSet
    {
        public CallBackSet(Func<object, Vector2> getObjectPosition, Func<object, Vector2> getMikePosition, Func<int> getSwitchIndex)
        {
            GetObjectPosition = getObjectPosition;
            GetMikePosition = getMikePosition;
            GetSwitchIndex = getSwitchIndex;
        }

        public Func<object, Vector2> GetObjectPosition { get; }
        public Func<object, Vector2> GetMikePosition { get; }
        public Func<int> GetSwitchIndex { get; } // Unused in Rayman 3
    }

    #endregion
}