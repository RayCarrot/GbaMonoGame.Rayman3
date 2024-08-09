﻿using System;
using BinarySerializer.Ubisoft.GbaEngine;
using Microsoft.Xna.Framework.Audio;

namespace GbaMonoGame;

// TODO: Find better C# audio library since the MonoGame one is too limiting. We need to support loop points and ideally fade-out.

// === INFO ON SOUNDS ===
// The original sound effects in the game are .aif files. These appear directly as such in the Digiblast version, while in the N-Gage
// version they were converted to .wav files. The N-Gage version also has 4 additional sound effects not in other versions. On GBA
// the sounds are stored in the MIDI format used by the MP2K sound engine. This version has more variants of the same sound effects,
// with each played with a different pitch to add some variation.
// The sounds in this project were extracted from the GBA ROM using https://github.com/ipatix/agbplay. This allows the different variants
// of each sound to be retained. The file names were taken from the MIDI files in Rayman 4 DS. The Digiblast version also has file names,
// but only for the original .aif files and is thus missing the ones for the different sound variants.
// For the N-Gage version we read the .wav sound effects directly, while using the GBA rip for the music files (since those are stored
// as .XM MIDI files on N-Gage and Digiblast).
public abstract class SoundEventsManager
{
    #region Public Properties

    // Allow a separate GBA and N-Gage implementation due to them having entirely different sound code
    public static SoundEventsManager Current { get; private set; }

    #endregion

    #region Protected Methods

    protected abstract void RefreshEventSetImpl();
    protected abstract void SetCallBacksImpl(CallBackSet callBacks);
    protected abstract void ProcessEventImpl(short soundEventId, object obj);
    protected abstract bool IsSongPlayingImpl(short soundEventId);
    protected abstract void SetSoundPitchImpl(short soundEventId, float pitch);
    protected abstract short ReplaceAllSongsImpl(short soundEventId, float fadeOut);
    protected abstract void FinishReplacingAllSongsImpl();
    protected abstract void StopAllSongsImpl();
    protected abstract void PauseAllSongsImpl();
    protected abstract void ResumeAllSongsImpl();
    protected abstract float GetVolumeForTypeImpl(SoundType type);
    protected abstract void SetVolumeForTypeImpl(SoundType type, float newVolume);

    protected abstract void ForcePauseAllSongsImpl();
    protected abstract void ForceResumeAllSongsImpl();
    protected abstract SoundEffect GetSoundByNameImpl(string name);
    protected abstract void DrawDebugLayoutImpl();

    #endregion

    #region Public Methods

    public static void Load(SoundEventsManager manager)
    {
        SoundEngineInterface.Load();
        Current = manager;
    }

    public static void RefreshEventSet() => Current.RefreshEventSetImpl();

    public static void SetCallBacks(CallBackSet callBacks) => Current.SetCallBacksImpl(callBacks);

    public static void ProcessEvent(Enum soundEventId) => ProcessEvent(soundEventId, null);
    public static void ProcessEvent(short soundEventId) => ProcessEvent(soundEventId, null);
    public static void ProcessEvent(Enum soundEventId, object obj) => ProcessEvent((short)(object)soundEventId, obj);
    public static void ProcessEvent(short soundEventId, object obj) => Current.ProcessEventImpl(soundEventId, obj);

    public static bool IsSongPlaying(Enum soundEventId) => IsSongPlaying((short)(object)soundEventId);
    public static bool IsSongPlaying(short soundEventId) => Current.IsSongPlayingImpl(soundEventId);

    public static void SetSoundPitch(Enum soundEventId, float pitch) => SetSoundPitch((short)(object)soundEventId, pitch);
    public static void SetSoundPitch(short soundEventId, float pitch) => Current.SetSoundPitchImpl(soundEventId, pitch);

    public static short ReplaceAllSongs(Enum soundEventId, float fadeOut) => ReplaceAllSongs((short)(object)soundEventId, fadeOut);
    public static short ReplaceAllSongs(short soundEventId, float fadeOut) => Current.ReplaceAllSongsImpl(soundEventId, fadeOut);

    public static void FinishReplacingAllSongs() => Current.FinishReplacingAllSongsImpl();

    public static void StopAllSongs() => Current.StopAllSongsImpl();
    public static void PauseAllSongs() => Current.PauseAllSongsImpl();
    public static void ResumeAllSongs() => Current.ResumeAllSongsImpl();

    public static float GetVolumeForType(SoundType type) => Current.GetVolumeForTypeImpl(type);
    public static void SetVolumeForType(SoundType type, float newVolume) => Current.SetVolumeForTypeImpl(type, newVolume);

    // Custom
    public static void ForcePauseAllSongs() => Current.ForcePauseAllSongsImpl();
    public static void ForceResumeAllSongs() => Current.ForceResumeAllSongsImpl();
    public static SoundEffect GetSoundByName(string name) => Current.GetSoundByNameImpl(name);
    public static void DrawDebugLayout() => Current.DrawDebugLayoutImpl();

    #endregion
}