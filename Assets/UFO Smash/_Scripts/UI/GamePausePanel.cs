using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class GamePausePanel : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    private IEventBus eventBus;
    private IAudioService audioService;
    private void Awake()
    {
        eventBus = ServiceLocator.GetService<IEventBus>();
        audioService = ServiceLocator.GetService<IAudioService>();
        resumeButton.onClick.AddListener(OnGameResumed);
        restartButton.onClick.AddListener(OnGameReset);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
    }

    private void OnGameResumed()
    {
        Time.timeScale = 1;
        audioService.UISFX(SoundType.Click);
        audioService.ResumeGamePauseAudio();
        eventBus.Publish(new Events.OnGameResumed());
    }
    private void OnGameReset()
    {
        audioService.UISFX(SoundType.Click);
        eventBus.Publish(new Events.OnGameReset());
        StartCoroutine(RestartGame());

    }
    private void OnHomeButtonClicked()
    {
        Time.timeScale = 1;
        audioService.UISFX(SoundType.Click);
        eventBus.Publish(new Events.OnGameReset());
        eventBus.Publish(new Events.OnReturnToHome());
    }
    private IEnumerator RestartGame()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.1f);
        eventBus.Publish(new Events.OnGameRestarted());
        eventBus.Publish(new Events.OnGameStarted());
    }
}
