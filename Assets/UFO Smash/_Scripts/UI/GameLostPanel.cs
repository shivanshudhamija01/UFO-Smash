using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class GameLostPanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    private IEventBus eventBus;
    private IAudioService audioService;
    void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        audioService = ServiceLocator.Get<IAudioService>();
        restartButton.onClick.AddListener(OnGameReset);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
    }

    void OnGameReset()
    {
        audioService.SFX(SoundType.Click);
        eventBus.Publish(new Events.OnGameReset());
        StartCoroutine(RestartGame());

    }
    void OnHomeButtonClicked()
    {
        Time.timeScale = 1;
        audioService.SFX(SoundType.Click);
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


