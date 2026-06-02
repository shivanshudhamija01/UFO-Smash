using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    private IEventBus eventBus;
    private IAudioService audioService;
    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        audioService = ServiceLocator.Get<IAudioService>();
        playButton.onClick.AddListener(OnPlayButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        audioService.SFX(SoundType.Click);
        eventBus.Publish(new Events.OnGameStarted());
    }
    private void OnSettingButtonClicked()
    {
        audioService.SFX(SoundType.Click);
        eventBus.Publish(new Events.OnSettingButtonClicked());
    }
    private void OnExitButtonClicked()
    {
        audioService.SFX(SoundType.Click);
        Application.Quit();
    }
}
