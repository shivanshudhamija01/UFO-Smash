using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{
    [SerializeField] private int maxAnimalLives;
    [SerializeField] private float gameOverDelay = 2f;

    private bool isGameOverSequenceRunning;
    private int animalLivesCount;

    private IEventBus eventBus;
    private IScoreService scoreService;
    private IAudioService audioService;

    private void Awake()
    {
        eventBus = ServiceLocator.GetService<IEventBus>();
        scoreService = ServiceLocator.GetService<IScoreService>();
        audioService = ServiceLocator.GetService<IAudioService>();
        animalLivesCount = maxAnimalLives;
        isGameOverSequenceRunning = false;
    }

    private void OnEnable()
    {
        eventBus.Add<Events.OnAnimalTaken>(HandleAnimalTaken);
        eventBus.Add<Events.OnGameReset>(HandleGameReset);
    }

    private void OnDisable()
    {
        eventBus.Remove<Events.OnAnimalTaken>(HandleAnimalTaken);
        eventBus.Remove<Events.OnGameReset>(HandleGameReset);
    }

    private void HandleAnimalTaken(Events.OnAnimalTaken data)
    {
        if (isGameOverSequenceRunning)
            return;

        animalLivesCount--;

        if (animalLivesCount <= 0)
        {
            isGameOverSequenceRunning = true;
            audioService.SFX(SoundType.GameOver);
            StartCoroutine(GameOverRoutine());
        }
    }
    private IEnumerator GameOverRoutine()
    {
        eventBus.Publish(new Events.DisableGameplayInput());

        yield return new WaitForSeconds(gameOverDelay);
        eventBus.Publish(new Events.OnGameOver());
        audioService.PauseGamePlayAudio();
        Time.timeScale = 0;
    }
    private void HandleGameReset(Events.OnGameReset evt)
    {
        animalLivesCount = maxAnimalLives;
        isGameOverSequenceRunning = false;

    }
}