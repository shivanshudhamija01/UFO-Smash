using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<SoundInfo> soundInfos;

    public AudioSource BGMSource => bgmSource;
    public AudioSource SFXSource => sfxSource;
    public List<SoundInfo> Audios => soundInfos;
}
