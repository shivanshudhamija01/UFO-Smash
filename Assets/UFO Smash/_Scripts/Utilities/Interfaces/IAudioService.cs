public interface IAudioService
{
    void BGM(SoundType soundType);
    void SFX(SoundType soundType);
    void UISFX(SoundType soundType);
    void SetBGMVolume(float value);
    void SetSFXVolume(float value);
    float GetBGMVolume();
    float GetSFXVolume();
    void PauseGamePlayAudio();
    void ResumeGamePauseAudio();
}
