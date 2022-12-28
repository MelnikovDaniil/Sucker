using System.Collections.Generic;
using UnityEngine;

public class SoundManagerComponent : MonoBehaviour
{
    public void PlaySound(string name)
    {
        SoundManager.PlaySound(name);
    }

    public void PlayRandomSound(IEnumerable<string> names)
    {
        var randomTrack = names.GetRandomOrDefault();
        SoundManager.PlaySound(randomTrack);
    }

    public void PlayClip(AudioClip clip)
    {
        SoundManager.PlaySound(clip);
    }

    public void PlayRandomClip(IEnumerable<AudioClip> clips)
    {
        var randomClip = clips.GetRandomOrDefault();
        SoundManager.PlaySound(randomClip);
    }

    public void PlaySoundNotPausable(string name)
    {
        SoundManager.PlaySoundUI(name);
    }

    public void PlayRandomSoundNotPausable(IEnumerable<string> names)
    {
        var randomTrack = names.GetRandomOrDefault();
        SoundManager.PlaySoundUI(randomTrack);
    }

    public void PlayClipNotPausable(AudioClip clip)
    {
        SoundManager.PlaySoundUI(clip);
    }

    public void PlayRandomClipNotPausable(IEnumerable<AudioClip> clips)
    {
        var randomClip = clips.GetRandomOrDefault();
        SoundManager.PlaySoundUI(randomClip);
    }

    public void ChangeSoundVolume(float volume)
    {
        SoundManager.SetSoundVolume(volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        SoundManager.SetMusicVolume(volume);
    }

    public void ToggleMusicMuted()
    {
        SoundManager.SetMusicMuted(!SoundManager.GetMusicMuted());
    }

    public void ToggleSoundMuted()
    {
        SoundManager.SetSoundMuted(!SoundManager.GetSoundMuted());
    }
}
