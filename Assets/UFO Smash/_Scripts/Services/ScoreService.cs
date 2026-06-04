using UnityEngine;

public class ScoreService : IScoreService
{
    private int score;
    private int highScore;
    private IEventBus eventBus;
    public ScoreService()
    {
        score = 0;
        eventBus = ServiceLocator.GetService<IEventBus>();
        eventBus.Add<Events.OnGameReset>(ResetScore);
        highScore = PlayerPrefs.GetInt(Keys.HIGH_SCORE, 0);
    }
    public int GetScore()
    {
        return score;
    }
    public int GetHighScore() => highScore;

    public void AddScore(int amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;

            PlayerPrefs.SetInt(
                Keys.HIGH_SCORE,
                highScore);

            PlayerPrefs.Save();
        }

    }
    private void ResetScore(Events.OnGameReset evt)
    {
        score = 0;
    }
}
