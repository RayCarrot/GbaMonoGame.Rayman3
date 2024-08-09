using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer.Ubisoft.GbaEngine;
using ImGuiNET;
using Microsoft.Xna.Framework.Audio;

namespace GbaMonoGame;

public class NGageSoundEventsManager : SoundEventsManager
{
    #region Constructor

    public NGageSoundEventsManager(Dictionary<int, string> songTable, NGageSoundEvent[] soundEvents)
    {
        // Load the sound resources
        Dictionary<int, SoundEffect> loadedSounds = new();
        foreach (NGageSoundEvent evt in soundEvents)
        {
            if (!evt.IsValid)
                continue;

            if (loadedSounds.TryGetValue(evt.SoundResourceId, out SoundEffect snd))
            {
                SoundResources[evt.SoundResourceId] = snd;
            }
            else
            {
                // Load music from extracted files
                if (evt.IsMusic)
                {
                    snd = SoundEffect.FromFile($"{songTable[evt.SoundResourceId]}.wav");
                }
                // Load sound effects from game data since it's already .wav data there and the N-Gage version has a few exclusive sounds
                else
                {
                    using Stream sndStream = Storage.LoadResourceStream(evt.SoundResourceId);
                    snd = SoundEffect.FromStream(sndStream);
                }

                snd.Name = songTable[evt.SoundResourceId];
                loadedSounds[evt.SoundResourceId] = snd;
                SoundResources[evt.SoundResourceId] = snd;
            }
        }
    }

    #endregion

    #region Private Properties

    private Dictionary<int, SoundEffect> SoundResources { get; } = new();
    private ActiveSong ActiveMusic { get; set; }
    private Dictionary<int, ActiveSong> ActiveSoundEffects { get; } = new(); // On N-Gage this is max 64 songs, but we don't need that limit

    #endregion

    #region Private Methods

    private void CreateSong(NGageSoundEvent evt)
    {
        SoundEffect sndEffect = SoundResources[evt.SoundResourceId];
        SoundEffectInstance sndEffectInstance = sndEffect.CreateInstance();

        ActiveSong song = new()
        {
            SoundResourceId = evt.SoundResourceId,
            Volume = (float)evt.Volume / 7,
            IsMusic = evt.IsMusic,
            SoundEffect = sndEffect,
            SoundInstance = sndEffectInstance
        };

        sndEffectInstance.IsLooped = evt.Loop;

        // Only one music track can play at a time
        if (evt.IsMusic)
        {
            ActiveMusic?.SoundInstance.Dispose();
            ActiveMusic = song;
        }
        // Only one sound effect of the same type can play at a time
        else
        {
            if (ActiveSoundEffects.TryGetValue(evt.SoundResourceId, out ActiveSong existingSong))
                existingSong.SoundInstance.Dispose();

            ActiveSoundEffects[evt.SoundResourceId] = song;
        }

        UpdateVolume(song);
        sndEffectInstance.Play();
    }

    private void StopSong(NGageSoundEvent evt)
    {
        if (evt.IsMusic)
        {
            if (ActiveMusic != null && ActiveMusic.SoundResourceId == evt.SoundResourceId)
            {
                ActiveMusic.SoundInstance.Dispose();
                ActiveMusic = null;
            }
        }
        else
        {
            if (ActiveSoundEffects.TryGetValue(evt.SoundResourceId, out ActiveSong sfx))
            {
                sfx.SoundInstance.Dispose();
                ActiveSoundEffects.Remove(evt.SoundResourceId);
            }
        }
    }

    private void UpdateVolume(ActiveSong song)
    {
        float vol = song.Volume;

        if (song.IsMusic)
            vol *= Engine.Config.MusicVolume;
        else
            vol *= Engine.Config.SfxVolume;

        song.SoundInstance.Volume = vol;
    }

    #endregion

    #region Protected Methods

    protected override void RefreshEventSetImpl()
    {
        if (ActiveMusic != null)
        {
            UpdateVolume(ActiveMusic);

            if (ActiveMusic.SoundInstance.State == SoundState.Stopped && !ActiveMusic.SoundInstance.IsLooped)
            {
                ActiveMusic.SoundInstance.Dispose();
                ActiveMusic = null;
            }
        }

        foreach (ActiveSong sfx in ActiveSoundEffects.Values.ToArray())
        {
            UpdateVolume(sfx);

            if (sfx.SoundInstance.State == SoundState.Stopped && !sfx.SoundInstance.IsLooped)
            {
                sfx.SoundInstance.Dispose();
                ActiveSoundEffects.Remove(sfx.SoundResourceId);
            }
        }
    }

    protected override void SetCallBacksImpl(CallBackSet callBacks) { }

    protected override void ProcessEventImpl(short soundEventId, object obj)
    {
        NGageSoundEvent evt = Engine.Loader.NGage_SoundEvents[soundEventId];

        if (evt.PlaySong)
            CreateSong(evt);
        else
            StopSong(evt);
    }

    protected override bool IsSongPlayingImpl(short soundEventId)
    {
        NGageSoundEvent evt = Engine.Loader.NGage_SoundEvents[soundEventId];

        if (evt.IsMusic)
            return ActiveMusic != null && ActiveMusic.SoundResourceId == evt.SoundResourceId;
        else
            return ActiveSoundEffects.ContainsKey(evt.SoundResourceId);
    }

    protected override void SetSoundPitchImpl(short soundEventId, float pitch) { }
    
    protected override short ReplaceAllSongsImpl(short soundEventId, float fadeOut)
    {
        ProcessEventImpl(soundEventId, null);
        return 0;
    }

    protected override void FinishReplacingAllSongsImpl() { }

    protected override void StopAllSongsImpl() { }

    protected override void PauseAllSongsImpl() { }
    
    protected override void ResumeAllSongsImpl() { }

    protected override float GetVolumeForTypeImpl(SoundType type) => SoundEngineInterface.MaxVolume;

    protected override void SetVolumeForTypeImpl(SoundType type, float newVolume) { }

    protected override void ForcePauseAllSongsImpl()
    {
        ActiveMusic?.SoundInstance.Pause();

        foreach (ActiveSong sfx in ActiveSoundEffects.Values)
            sfx.SoundInstance.Pause();
    }

    protected override void ForceResumeAllSongsImpl()
    {
        ActiveMusic?.SoundInstance.Resume();

        foreach (ActiveSong sfx in ActiveSoundEffects.Values)
            sfx.SoundInstance.Resume();
    }

    protected override SoundEffect GetSoundByNameImpl(string name)
    {
        return SoundResources.Values.First(x => x.Name == name);
    }

    protected override void DrawDebugLayoutImpl()
    {
        if (ImGui.BeginTable("_songs", 4))
        {
            ImGui.TableSetupColumn("Resource", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("State", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Duration", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableHeadersRow();

            if (ActiveMusic != null)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{ActiveMusic.SoundResourceId}");

                ImGui.TableNextColumn();
                ImGui.Text($"{ActiveMusic.SoundEffect.Name}");

                ImGui.TableNextColumn();
                ImGui.Text($"{ActiveMusic.SoundInstance.State}");

                ImGui.TableNextColumn();
                ImGui.Text($"{ActiveMusic.SoundEffect.Duration.TotalSeconds:F}");
            }

            foreach (ActiveSong playingSong in ActiveSoundEffects.Values)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundResourceId}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundEffect.Name}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundInstance.State}");

                ImGui.TableNextColumn();
                ImGui.Text($"{playingSong.SoundEffect.Duration.TotalSeconds:F}");
            }

            ImGui.EndTable();
        }
    }

    #endregion

    #region Data Types

    private class ActiveSong
    {
        public int SoundResourceId { get; init; }
        public float Volume { get; init; }
        public bool IsMusic { get; init; }

        // MonoGame
        public SoundEffectInstance SoundInstance { get; init; }
        public SoundEffect SoundEffect { get; init; }
    }

    #endregion
}