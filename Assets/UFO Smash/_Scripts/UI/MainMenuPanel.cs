using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button infoButton;
    private IEventBus eventBus;
    private IAudioService audioService;
    private IScoreService scoreService;
    private void Awake()
    {
        eventBus = ServiceLocator.GetService<IEventBus>();
        audioService = ServiceLocator.GetService<IAudioService>();
        scoreService = ServiceLocator.GetService<IScoreService>();
        playButton.onClick.AddListener(OnPlayButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        infoButton.onClick.AddListener(OnInfoButtonClicked);
    }
    void OnEnable()
    {
        scoreText.text = scoreService.GetHighScore().ToString();
    }
    private void OnPlayButtonClicked()
    {
        audioService.UISFX(SoundType.Click);
        audioService.ResumeGamePauseAudio();
        eventBus.Publish(new Events.OnGameStarted());
    }
    private void OnSettingButtonClicked()
    {
        audioService.UISFX(SoundType.Click);
        eventBus.Publish(new Events.OnSettingButtonClicked());
    }
    private void OnExitButtonClicked()
    {
        audioService.UISFX(SoundType.Click);
        Application.Quit();
    }
    private void OnInfoButtonClicked()
    {
        audioService.UISFX(SoundType.Click);
        eventBus.Publish(new Events.OnInfoButtonClicked());
    }
}
