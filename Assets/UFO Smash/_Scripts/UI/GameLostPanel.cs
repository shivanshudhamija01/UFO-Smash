using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class GameLostPanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI bestScore;
    private IEventBus eventBus;
    private IAudioService audioService;
    private IScoreService scoreService;
    void Awake()
    {
        eventBus = ServiceLocator.GetService<IEventBus>();
        audioService = ServiceLocator.GetService<IAudioService>();
        scoreService = ServiceLocator.GetService<IScoreService>();
        restartButton.onClick.AddListener(OnGameReset);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
    }
    void OnEnable()
    {
        UpdateScore();
    }
    void OnDisable()
    {

    }
    void OnGameReset()
    {
        audioService.UISFX(SoundType.Click);

        eventBus.Publish(new Events.OnGameReset());
        StartCoroutine(RestartGame());

    }
    void OnHomeButtonClicked()
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
    void UpdateScore()
    {
        currentScore.text = scoreService.GetScore().ToString();
        bestScore.text = scoreService.GetHighScore().ToString();
    }
}


