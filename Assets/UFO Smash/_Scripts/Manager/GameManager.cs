using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int maxAnimalLives;
    private int animalLivesCount;

    private IEventBus eventBus;
    private IScoreService scoreService;

    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        scoreService = ServiceLocator.Get<IScoreService>();
        animalLivesCount = maxAnimalLives;
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
        animalLivesCount--;
        if (animalLivesCount <= 0)
        {
            Debug.Log("Game is over ");
            // Here may be i need to add something like that , as if the game is over, then instantly not set the time.timescale to zero , instead fire an event to pause the game , and then pop up the game lost panel 
            eventBus.Publish(new Events.PauseGame());

            eventBus.Publish(new Events.OnGameOver());
            Time.timeScale = 0;
        }
    }

    private void HandleUFODestroyed(Events.OnUFODestroyed data)
    {
        int score = scoreService.GetScore();
    }
    private void HandleGameReset(Events.OnGameReset evt)
    {
        animalLivesCount = maxAnimalLives;
    }
}