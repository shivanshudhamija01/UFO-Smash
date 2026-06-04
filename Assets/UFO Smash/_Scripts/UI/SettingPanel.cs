using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button closeButton;
    private IEventBus eventBus;
    private IAudioService audioService;

    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        audioService = ServiceLocator.Get<IAudioService>();
        closeButton.onClick.AddListener(OnCloseButtonClicked);

        // Load Saved Values
        bgmSlider.value = PlayerPrefs.GetFloat(Keys.BGM, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(Keys.SFX, 1f);

        bgmSlider.onValueChanged.AddListener(OnBgmValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxValueChanged);

    }
    private void OnCloseButtonClicked()
    {
        audioService.UISFX(SoundType.Click);
        eventBus.Publish(new Events.OnCloseButtonClicked());
        Debug.Log("Close button is clicked");
    }
    private void OnBgmValueChanged(float value)
    {
        // PlayerPrefs.SetFloat(Keys.BGM, value);
        // PlayerPrefs.Save();
        audioService.SetBGMVolume(value);
    }

    private void OnSfxValueChanged(float value)
    {
        // PlayerPrefs.SetFloat(Keys.SFX, value);
        // PlayerPrefs.Save();
        audioService.SetSFXVolume(value);
    }

}
