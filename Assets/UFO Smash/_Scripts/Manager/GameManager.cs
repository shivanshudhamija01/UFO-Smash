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

    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        scoreService = ServiceLocator.Get<IScoreService>();
        animalLivesCount = maxAnimalLives;
        isGameOverSequenceRunning = false;
    }

    private void OnEnable()
    {
        eventBus.Add<Events.OnUFODestroyed>(HandleUFODestroyed);
        eventBus.Add<Events.OnAnimalTaken>(HandleAnimalTaken);
        eventBus.Add<Events.OnGameReset>(HandleGameReset);
    }

    private void OnDisable()
    {
        eventBus.Remove<Events.OnUFODestroyed>(HandleUFODestroyed);
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
            StartCoroutine(GameOverRoutine());
        }
    }
    private IEnumerator GameOverRoutine()
    {
        eventBus.Publish(new Events.DisableGameplayInput());

        yield return new WaitForSeconds(gameOverDelay);

        eventBus.Publish(new Events.OnGameOver());
    }
    private void HandleUFODestroyed(Events.OnUFODestroyed data)
    {
        int score = scoreService.GetScore();
    }
    private void HandleGameReset(Events.OnGameReset evt)
    {
        animalLivesCount = maxAnimalLives;
        isGameOverSequenceRunning = false;

    }
}