using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioService : IAudioService
{
    private Dictionary<SoundType, SoundInfo> audioMap = new();
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    public AudioService(AudioManager audioManager)
    {
        sfxSource = audioManager.SFXSource;
        bgmSource = audioManager.BGMSource;
        foreach (var sound in audioManager.Audios)
        {
            audioMap[sound.id] = sound;
        }
    }
    public void BGM(SoundType soundType)
    {
        if (audioMap.TryGetValue(soundType, out var sound))
        {
            bgmSource.clip = sound.audioClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }
    public void SFX(SoundType soundType)
    {
        if (audioMap.TryGetValue(soundType, out var sound))
        {
            // Here may be i need to play the volume according to the clips 
            float finalVolume = sound.volume * sfxVolume;

            sfxSource.PlayOneShot(sound.audioClip, finalVolume);
            // sfxSource.PlayOneShot(sound.audioClip, sfxSource.volume);
        }
    }
    public float GetBGMVolume()
    {
        return bgmVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        bgmSource.volume = value;
        PlayerPrefs.SetFloat(Keys.BGM, value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(Keys.SFX, value);
    }

}
