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
    private static SoundBank _soundBank;
    private static readonly float[] _volumePerType = Enumerable.Repeat(SoundEngineInterface.MaxVolume, 8).ToArray();
    private static readonly List<ActiveSong> _activeSongs = new(); // On GBA this is max 4 songs, but we don't need that limit

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

    private static void CalculateRollOffAndPan(Vector2 mikePos, Vector2 objPos, out float rollOffLvl, out float dx)
    {
        throw new NotImplementedException();
    }

    private static void UpdateVolumeAndPan(ActiveSong song)
    {
        float vol;
        float pan;

        if (song.Obj == null && (song.IsRollOffEnabled || song.IsPanEnabled))
            throw new Exception("Song has roll-off or pan enabled, but no object is set!");

        if (song.IsRollOffEnabled || song.IsPanEnabled)
        {
            // TODO: Pass in mike pos and obj pos - the game gets it from callbacks
            CalculateRollOffAndPan(Vector2.Zero, Vector2.Zero, out vol, out pan);
        }
        else
        {
            vol = SoundEngineInterface.MaxVolume;
            pan = 0;
        }

        vol *= _volumePerType[(int)song.SoundType] / SoundEngineInterface.MaxVolume;

        if (song.Volume != vol || song.Pan != pan)
        {
            song.SoundInstance.Volume = vol / SoundEngineInterface.MaxVolume;
            // TODO: Set pan on sound instance
            
            song.Volume = vol;
            song.Pan = pan;
        }
    }

    #endregion

    #region Internal Methods

    internal static void Load(int soundBankResourceId, Dictionary<int, string> songTable)
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
            playingSong.SoundInstance.Resume();
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

    #endregion
}