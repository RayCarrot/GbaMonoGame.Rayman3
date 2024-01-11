using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Onyx.Gba;
using Microsoft.Xna.Framework.Audio;
using SoundBank = BinarySerializer.Onyx.Gba.SoundBank;

namespace OnyxCs.Gba;

// TODO: Implement remaining sound functions. Try and handle sounds like game does. Currently some important ones
//       are missing like SetVolumeForType which fades in music during intro etc.
public static class SoundEventsManager
{
    private static readonly Dictionary<int, SoundEffect> _songs = new();
    private static SoundBank _soundBank;

    internal static readonly List<PlayingSong> _playingSongs = new();

    internal static void Load(int soundBankResourceId, Dictionary<int, string> songTable)
    {
        if (Engine.Settings.Platform == Platform.GBA)
            _soundBank = Storage.LoadResource<SoundBank>(soundBankResourceId);

        // TODO: Load music as Song?
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

    internal static void Step()
    {
        foreach (PlayingSong playingSong in _playingSongs.ToArray())
        {
            if (playingSong.SoundInstance.State == SoundState.Stopped)
            {
                _playingSongs.Remove(playingSong);

                if (playingSong.NextSoundEventId != null)
                    ProcessEvent(playingSong.NextSoundEventId.Value, playingSong.Obj);
            }
        }
    }

    private static void Stop(ushort soundEventId, ushort? nextEventId, uint fadeOut, object obj)
    {
        bool foundSong = false;

        foreach (PlayingSong playingSong in _playingSongs)
        {
            if (playingSong.EventId == soundEventId && playingSong.Obj == obj)
            {
                // TODO: Fade out
                playingSong.SoundInstance.Stop();

                if (!foundSong)
                {
                    foundSong = true;
                    playingSong.NextSoundEventId = nextEventId;
                }
            }
        }

        if (!foundSong && nextEventId != null)
        {
            ProcessEvent(nextEventId.Value, obj);
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

    private static SoundResource GetResource(ushort resourceId)
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

                return GetResource(resId.Value);

            default:
                throw new Exception($"Invalid resource type {res.Type}");
        }
    }

    public static void ProcessEvent(Enum soundEventId, object obj = null) => ProcessEvent((ushort)(object)soundEventId, obj);
    public static void ProcessEvent(ushort soundEventId, object obj = null)
    {
        switch (Engine.Settings.Platform)
        {
            case Platform.GBA:
            {
                SoundEvent evt = _soundBank.Events[soundEventId].Value;

                switch (evt.Type)
                {
                    case SoundEvent.SoundEventType.PlaySong:
                        SoundResource res = GetResource(evt.ResourceId);
                        
                        if (res == null)
                            return;

                        SoundEffect song = _songs[res.SongTableIndex];
                        SoundEffectInstance snd = song.CreateInstance();
                        _playingSongs.Add(new PlayingSong(soundEventId, obj, snd, song));
                        snd.IsLooped = res.Flag0; // TODO: Not 100% sure about this
                        snd.Play();
                        break;
                
                    case SoundEvent.SoundEventType.StopSong:
                        Stop(evt.StopEventId, null, evt.FadeOutTime, obj);
                        break;
                
                    case SoundEvent.SoundEventType.StopAndSetNext:
                        Stop(evt.StopEventId, evt.NextEventId, evt.FadeOutTime, obj);
                        break;
                }
                break;
            }
            
            case Platform.NGage:
                // TODO: Implement
                break;
            
            default:
                throw new UnsupportedPlatformException();
        }
    }

    public static bool IsPlaying(Enum soundEventId) => IsPlaying((ushort)(object)soundEventId);
    public static bool IsPlaying(ushort soundEventId)
    {
        return _playingSongs.Any(x => x.EventId == soundEventId);
    }

    public static void FUN_080ac468(Enum soundEventId, float unknown) => FUN_080ac468((ushort)(object)soundEventId, unknown);
    public static void FUN_080ac468(ushort soundEventId, float unknown)
    {
        // TODO: Implement. Sound speed or pitch?
    }

    public static void FUN_080abe44(Enum soundEventId, float fadeOut) => FUN_080abe44((ushort)(object)soundEventId, fadeOut);
    public static void FUN_080abe44(ushort soundEventId, float fadeOut)
    {
        // TODO: Implement. Implement flags in Ghidra to understand this.
    }

    public static void FUN_08001954(Enum soundEventId) => FUN_08001954((ushort)(object)soundEventId);
    public static void FUN_08001954(ushort soundEventId)
    {
        // TODO: Implement
    }

    public static void StopAll()
    {
        foreach (PlayingSong playingSong in _playingSongs)
        {
            playingSong.SoundInstance.Stop();
            playingSong.SoundInstance.Dispose();
        }

        _playingSongs.Clear();
    }

    public static void Pause()
    {
        foreach (PlayingSong playingSong in _playingSongs)
        {
            playingSong.SoundInstance.Pause();
        }
    }

    public static void Resume()
    {
        foreach (PlayingSong playingSong in _playingSongs)
        {
            playingSong.SoundInstance.Resume();
        }
    }

    internal record PlayingSong(ushort EventId, object Obj, SoundEffectInstance SoundInstance, SoundEffect SoundEffect)
    {
        public ushort? NextSoundEventId { get; set; }
    }
}