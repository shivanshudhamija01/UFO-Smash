using UnityEngine;

public class ScoreService : IScoreService
{
    private int score;
    private IEventBus eventBus;
    public ScoreService()
    {
        score = 0;
        eventBus = ServiceLocator.Get<IEventBus>();
        eventBus.Add<Events.OnGameReset>(ResetScore);
    }
    public int GetScore()
    {
        return score;
    }

    public void AddScore(int amount)
    {
        score += amount;
    }
    private void ResetScore(Events.OnGameReset evt)
    {
        score = 0;
    }
}
